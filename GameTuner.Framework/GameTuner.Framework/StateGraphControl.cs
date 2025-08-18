using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using GameTuner.Framework.Graph;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework
{
	public class StateGraphControl : WorkspaceControl
	{
		private enum DragMode
		{
			Idle,
			Node,
			RubberBox,
			Overview,
			NodeNewLink,
			Nub
		}

		private class NodeTag : IBoundsProvider, IRadiusProvider, ILocationProvider
		{
			public Vec2 Anchor { get; set; }

			public RectangleF Bounds { get; set; }

			public float Radius { get; set; }

			public Vec2 Location { get; set; }

			public RectangleF DataBounds { get; set; }

			public NodeTag()
			{
				Radius = 36f;
				Location = Vec2.Empty;
				Anchor = Vec2.Empty;
				Bounds = RectangleF.Empty;
			}
		}

		private class SocketTag : ISegmentProvider
		{
			public Vec2 SegA { get; set; }

			public Vec2 SegB { get; set; }

			public int Index { get; set; }

			public SocketTag()
			{
				SegA = Vec2.Empty;
				SegB = Vec2.Empty;
			}
		}

		private class NubTag : IRadiusProvider, ILocationProvider
		{
			public Vec2 Anchor { get; set; }

			public float Radius { get; set; }

			public Vec2 Location { get; set; }

			public RectangleF Bounds
			{
				get
				{
					return new RectangleF(Location.X - Radius, Location.Y - Radius, Radius * 2f, Radius * 2f);
				}
			}

			public NubTag()
			{
				Radius = 9f;
				Location = Vec2.Empty;
				Anchor = Vec2.Empty;
			}
		}

		private class DragSettings
		{
			public DragMode Mode;

			public Point2 Pos;

			public Point2 Current;

			public bool Moved;

			public RectangleF Rect;

			public object Item;

			public bool Ctrl;

			public bool Shift;

			public DragSettings()
			{
				Reset();
			}

			public void Reset()
			{
				Ctrl = false;
				Shift = false;
				Item = null;
				Mode = DragMode.Idle;
				Pos = Point2.Empty;
				Current = Point2.Empty;
				Moved = false;
				Rect = RectangleF.Empty;
			}
		}

		public delegate void PaintNodeHandler(object sender, StateGraphPaintEventArgs e);

		public delegate void StateGraphHandler(object sender, StateGraphEventArgs e);

		private const byte isolateAlpha = 16;

		private const float nodeWidth = 4f;

		private const float labelWidth = 3f;

		private const float nubWidth = 2f;

		private const int PadY = 5;

		private const float Saturate = 0.125f;

		private DragSettings drag = new DragSettings();

		private Point2 mousePoint = Point2.Empty;

		private bool ctrlPressed;

		private bool shiftPressed;

		private bool selectWhileUpdate;

		private bool showNubLabels;

		private bool showNodeLabels;

		private bool showIsolatedStates;

		private bool allowEdit;

		private bool showEval;

		private IGraph graph;

		private object focusItem;

		private readonly ColorMatrix SaturateMatrix = DrawingHelper.MakeSaturation(0.125f);

		private readonly ColorMatrix SaturateMatrixA = DrawingHelper.MakeSaturation(0.125f, 0.0627451f);

		private Pen nodePen;

		private Pen dashPen;

		private SolidBrush nodeBrush;

		private SolidBrush selectBrush;

		private Pen socketPen;

		private Pen socketPenSel;

		private AdjustableArrowCap socketEndCap;

		private PaintNodeHandler paintNodeHandler;

		private IContainer components;

		[Category("Graph")]
		[DefaultValue(false)]
		public bool AllowEdit
		{
			get
			{
				return allowEdit;
			}
			set
			{
				allowEdit = value;
				Invalidate();
			}
		}

		[DefaultValue(true)]
		[Category("Graph")]
		public bool SnapToGrid { get; set; }

		[Category("Graph")]
		[DefaultValue(false)]
		public bool MultiSockets { get; set; }

		[Category("Appearance")]
		public Color NodeColor { get; set; }

		[Category("Appearance")]
		public Color DefaultNodeColor { get; set; }

		[Category("Appearance")]
		public Color SocketColor { get; set; }

		[Category("Appearance")]
		public Color SelectedColor { get; set; }

		[Category("Appearance")]
		public Color NubColor { get; set; }

		[Category("Appearance")]
		[DefaultValue(false)]
		public bool ShowEval
		{
			get
			{
				return showEval;
			}
			set
			{
				showEval = value;
				Invalidate();
			}
		}

		[DefaultValue(true)]
		[Category("Appearance")]
		public bool ShowNubLabels
		{
			get
			{
				return showNubLabels;
			}
			set
			{
				showNubLabels = value;
				Invalidate();
			}
		}

		[DefaultValue(true)]
		[Category("Appearance")]
		public bool ShowNodeLabels
		{
			get
			{
				return showNodeLabels;
			}
			set
			{
				showNodeLabels = value;
				Invalidate();
			}
		}

		[DefaultValue(false)]
		[Category("Appearance")]
		public bool ShowIsolatedStates
		{
			get
			{
				return showIsolatedStates;
			}
			set
			{
				showIsolatedStates = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RectangleF NodeBounds { get; private set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public IGraph Graph
		{
			get
			{
				return graph;
			}
			set
			{
				graph = value;
				SelectedItems.Clear();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public PaintNodeHandler PaintNode
		{
			get
			{
				return paintNodeHandler;
			}
			set
			{
				paintNodeHandler = value ?? new PaintNodeHandler(DefaultPaintHandler);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ListEvent<object> SelectedItems { get; private set; }

		private ListEvent<object> RubberItems { get; set; }

		public event EventHandler SelectedItemChanged;

		public event EventHandler MoveNodeBegin;

		public event EventHandler MoveNodeEnd;

		public event EventHandler MoveNubBegin;

		public event EventHandler MoveNubEnd;

		public event EventHandler ModifiedAction;

		public event StateGraphHandler NodesLinking;

		public event StateGraphHandler NodesLinked;

		public event StateGraphHandler GraphContextMenu;

		public event StateGraphHandler SelectedItemDelete;

		public event StateGraphHandler DataProviderClicked;

		public StateGraphControl()
		{
			InitializeComponent();
			NodeColor = Color.FromArgb(107, 155, 233);
			NubColor = Color.FromArgb(170, 214, 119);
			SocketColor = Color.FromArgb(170, 214, 119);
			SelectedColor = Color.FromArgb(255, 193, 42);
			DefaultNodeColor = Color.FromArgb(197, 112, 180);
			ShowNubLabels = true;
			ShowNodeLabels = true;
			SnapToGrid = true;
			PaintNode = null;
			NodeBounds = RectangleF.Empty;
			nodePen = new Pen(NodeColor, 4f);
			nodeBrush = new SolidBrush(Color.White);
			selectBrush = new SolidBrush(SelectedColor);
			dashPen = new Pen(SelectedColor);
			dashPen.DashStyle = DashStyle.Dash;
			socketEndCap = new AdjustableArrowCap(5f, 5f);
			socketPen = new Pen(SocketColor, 4f);
			socketPen.CustomEndCap = socketEndCap;
			socketPen.Width = 4f;
			socketPen.Color = SocketColor;
			socketPen.CustomEndCap = socketEndCap;
			socketPenSel = (Pen)socketPen.Clone();
			socketPenSel.Width = 8f;
			socketPenSel.Color = SelectedColor;
			socketPenSel.EndCap = LineCap.Flat;
			RubberItems = new ListEvent<object>();
			SelectedItems = new ListEvent<object>();
			SelectedItems.ItemCountChanged += SelectedItems_ItemCountChanged;
			base.InvokedEndUpdate += StateGraphControl_InvokedEndUpdate;
		}

		private void SelectedItems_ItemCountChanged(object sender, ListEvent<object>.ListEventArgs e)
		{
			if (!base.Updating)
			{
				Invalidate();
			}
			else
			{
				selectWhileUpdate = true;
			}
		}

		private void StateGraphControl_InvokedEndUpdate(object sender, EventArgs e)
		{
			if (selectWhileUpdate)
			{
				EventHandler selectedItemChanged = this.SelectedItemChanged;
				if (selectedItemChanged != null)
				{
					selectedItemChanged(this, EventArgs.Empty);
				}
				selectWhileUpdate = false;
			}
		}

		protected override void PaintWorkspace(Graphics g, Rectangle rect)
		{
			Vec2 center = base.Center;
			CalcNodesRects(g, center);
			PaintRubberBox(g);
			if (Graph == null)
			{
				return;
			}
			BeginTranslate(g);
			PaintShadows(g);
			PaintSelection(g);
			PaintSockets(g);
			PaintNubs(g);
			PaintNodes(g);
			if (ShowEval)
			{
				using (StringFormat stringFormat = new StringFormat())
				{
					stringFormat.LineAlignment = StringAlignment.Center;
					stringFormat.Alignment = StringAlignment.Center;
					foreach (IGraphNode node in Graph.Nodes)
					{
						int num = 0;
						bool flag = node.Sockets.Find((IGraphSocket a) => ((IStateGraphSocket)a).Order != 0) == null;
						foreach (IGraphSocket socket in node.Sockets)
						{
							int isolatedAlpha = GetIsolatedAlpha(socket);
							IStateGraphSocket stateGraphSocket = (IStateGraphSocket)socket;
							SocketTag socketTag = socket.Tag as SocketTag;
							Vec2 vec = (socketTag.SegA + socketTag.SegB) * 0.5f;
							float num2 = (float)base.ScaledFont.Height / base.Zoom;
							RectangleF rectangleF = RectangleF.FromLTRB(vec.X - num2, vec.Y - num2, vec.X + num2, vec.Y + num2);
							int num3 = (flag ? num : stateGraphSocket.Order);
							using (Brush brush = new SolidBrush(Color.FromArgb(isolatedAlpha, Color.Tomato)))
							{
								g.FillEllipse(brush, rectangleF);
							}
							using (Pen pen = new Pen(Color.FromArgb(isolatedAlpha, SocketColor)))
							{
								g.DrawEllipse(pen, rectangleF);
							}
							using (Brush brush2 = new SolidBrush(Color.FromArgb(isolatedAlpha, Color.White)))
							{
								g.DrawString(num3.ToString(), base.ScaledFont, brush2, rectangleF, stringFormat);
							}
							num++;
						}
					}
				}
			}
			EndTranslate(g);
		}

		private void PaintRubberBox(Graphics g)
		{
			if (drag.Mode == DragMode.RubberBox && !drag.Rect.IsEmpty)
			{
				using (GraphicsPath path = DrawingHelper.CreateRoundRect(drag.Rect.ToRect(), 4))
				{
					selectBrush.Color = Color.FromArgb(32, SelectedColor);
					g.DrawPath(dashPen, path);
					g.FillPath(selectBrush, path);
				}
			}
		}

		private void CalcNodesRects(Graphics g, Vec2 point)
		{
			if (Graph == null)
			{
				NodeBounds = RectangleF.Empty;
				return;
			}
			Vec2 empty = Vec2.Empty;
			RectangleF empty2 = RectangleF.Empty;
			RectangleF? rectangleF = null;
			foreach (IGraphNode node in Graph.Nodes)
			{
				NodeTag nodeTag = node.Tag as NodeTag;
				if (nodeTag == null)
				{
					nodeTag = (NodeTag)(node.Tag = new NodeTag());
				}
				empty = node.Location;
				empty2.X = empty.X - nodeTag.Radius;
				empty2.Y = empty.Y - nodeTag.Radius;
				float num = (empty2.Height = nodeTag.Radius * 2f);
				empty2.Width = num;
				nodeTag.Bounds = empty2;
				nodeTag.Location = empty;
				RectangleF empty3 = RectangleF.Empty;
				IGraphDataProvider graphDataProvider = node as IGraphDataProvider;
				if (graphDataProvider != null && graphDataProvider.GraphDataImage != null)
				{
					float num3 = graphDataProvider.GraphDataImage.Width;
					float num4 = graphDataProvider.GraphDataImage.Height;
					empty3.X = empty.X - num3 / 2f;
					empty3.Y = empty.Y + nodeTag.Radius - num4 - 5f;
					empty3.Width = num3;
					empty3.Height = num4;
				}
				nodeTag.DataBounds = empty3;
				rectangleF = (rectangleF.HasValue ? new RectangleF?(RectangleF.Union(rectangleF.Value, empty2)) : new RectangleF?(empty2));
			}
			foreach (IGraphNode node2 in Graph.Nodes)
			{
				CalcSocketRects(g, node2, point);
			}
			NodeBounds = (rectangleF.HasValue ? rectangleF.Value : RectangleF.Empty);
		}

		public void CenterView()
		{
			base.Origin = new Vec2(NodeBounds.X + NodeBounds.Width * 0.5f, NodeBounds.Y + NodeBounds.Height * 0.5f);
		}

		private void CalcSocketRects(Graphics g, IGraphNode node, Vec2 point)
		{
			List<IGraphSocket> list = new List<IGraphSocket>();
			foreach (IGraphNode node2 in Graph.Nodes)
			{
				if (node2 == node)
				{
					continue;
				}
				list.Clear();
				CollectSocketConnections(node, node2, list);
				CollectSocketConnections(node2, node, list);
				if (list.Count <= 0)
				{
					continue;
				}
				list.Sort(SortSockets);
				foreach (IGraphSocket socket in node.Sockets)
				{
					SocketTag socketTag = socket.Tag as SocketTag;
					if (socketTag == null)
					{
						socketTag = (SocketTag)(socket.Tag = new SocketTag());
					}
					socketTag.Index = list.IndexOf(socket);
					if (socketTag.Index != -1)
					{
						ApplySocketSegment(node, node2, socketTag, list.Count);
					}
					CalcNubRects(g, socket);
				}
			}
		}

		private void CalcNubRects(Graphics g, IGraphSocket socket)
		{
			SocketTag socketTag = (SocketTag)socket.Tag;
			foreach (IGraphNub nub in socket.Nubs)
			{
				NubTag nubTag = nub.Tag as NubTag;
				if (nubTag == null)
				{
					nubTag = (NubTag)(nub.Tag = new NubTag());
				}
				nubTag.Location = Vec2.Lerp(socketTag.SegA, socketTag.SegB, nub.Order);
			}
		}

		private void ApplySocketSegment(IGraphNode a, IGraphNode b, SocketTag segment, int count)
		{
			NodeTag nodeTag = (NodeTag)a.Tag;
			NodeTag nodeTag2 = (NodeTag)b.Tag;
			Vec2 location = nodeTag.Location;
			Vec2 location2 = nodeTag2.Location;
			float num = (float)Math.Atan2(location2.Y - location.Y, location2.X - location.X);
			float num2 = MathEx.Deg2Rad(45f);
			float num3 = (0f - (float)(count - 1) * num2) / 2f + num2 * (float)segment.Index;
			float s;
			float c;
			MathEx.SinCos(num - num3, out s, out c);
			segment.SegA = new Vec2(location.X + c * nodeTag.Radius * 0.95f, location.Y + s * nodeTag.Radius);
			MathEx.SinCos(num + (float)Math.PI + num3, out s, out c);
			segment.SegB = new Vec2(location2.X + c * nodeTag2.Radius * 0.95f, location2.Y + s * nodeTag2.Radius);
		}

		private int SortSockets(IGraphSocket a, IGraphSocket b)
		{
			int num = Math.Sign(a.Owner.ID - b.Owner.ID);
			if (num == 0)
			{
				num = Math.Sign(a.ID - b.ID);
			}
			return num;
		}

		private void CollectSocketConnections(IGraphNode src, IGraphNode dst, List<IGraphSocket> sockets)
		{
			foreach (IGraphSocket socket in src.Sockets)
			{
				IStateGraphSocket stateGraphSocket = (IStateGraphSocket)socket;
				if (stateGraphSocket.Node == dst)
				{
					sockets.Add(socket);
				}
			}
		}

		public object Find(int x, int y)
		{
			if (Graph != null && !base.Updating)
			{
				Vec2 point = new Vec2(x, y);
				ClientToWorld(ref point);
				IGraphNode result;
				if ((result = FindNode(point)) != null)
				{
					return result;
				}
				IGraphNub result2;
				if ((result2 = FindNub(point)) != null)
				{
					return result2;
				}
				IGraphSocket result3;
				if ((result3 = FindSocket(point)) != null)
				{
					return result3;
				}
			}
			return null;
		}

		private IGraphSocket FindSocket(Vec2 pt)
		{
			GraphNodeCollection nodes = Graph.Nodes;
			int count = nodes.Count;
			for (int num = count - 1; num >= 0; num--)
			{
				IGraphNode graphNode = nodes[num];
				foreach (IGraphSocket socket in graphNode.Sockets)
				{
					SocketTag socketTag = (SocketTag)socket.Tag;
					if (socketTag != null && Vec2.DistancePointLine(pt, socketTag.SegA, socketTag.SegB) <= 4f)
					{
						return socket;
					}
				}
			}
			return null;
		}

		private IGraphNub FindNub(Vec2 pt)
		{
			GraphNodeCollection nodes = Graph.Nodes;
			int count = nodes.Count;
			for (int num = count - 1; num >= 0; num--)
			{
				IGraphNode graphNode = nodes[num];
				foreach (IGraphSocket socket in graphNode.Sockets)
				{
					foreach (IGraphNub nub in socket.Nubs)
					{
						NubTag nubTag = (NubTag)nub.Tag;
						if (nubTag != null && (pt - nubTag.Location).Length <= nubTag.Radius + 2f)
						{
							return nub;
						}
					}
				}
			}
			return null;
		}

		private IGraphNode FindNode(Vec2 pt)
		{
			GraphNodeCollection nodes = Graph.Nodes;
			int count = nodes.Count;
			for (int num = count - 1; num >= 0; num--)
			{
				IGraphNode graphNode = nodes[num];
				NodeTag nodeTag = (NodeTag)graphNode.Tag;
				if (nodeTag != null && (pt - graphNode.Location).Length <= nodeTag.Radius + 4f)
				{
					return graphNode;
				}
			}
			return null;
		}

		public Vec2 GetLocation(object o)
		{
			IGraphNub graphNub = o as IGraphNub;
			if (graphNub != null)
			{
				return ((NubTag)graphNub.Tag).Location;
			}
			IGraphSocket graphSocket = o as IGraphSocket;
			if (graphSocket != null)
			{
				return ((SocketTag)graphSocket.Tag).SegA + (((SocketTag)graphSocket.Tag).SegB - ((SocketTag)graphSocket.Tag).SegA) / 2f;
			}
			IGraphNode graphNode = o as IGraphNode;
			if (graphNode != null)
			{
				return graphNode.Location;
			}
			return new Vec2(0f, 0f);
		}

		public void RemoveSelectedItems()
		{
			if (SelectedItems.Count <= 0)
			{
				return;
			}
			bool flag = false;
			BeginUpdate();
			SelectedItems.Sort(SelectionSort);
			StateGraphHandler selectedItemDelete = this.SelectedItemDelete;
			if (selectedItemDelete != null)
			{
				StateGraphEventArgs e = new StateGraphEventArgs();
				selectedItemDelete(this, e);
				flag = e.Cancel;
			}
			if (!flag)
			{
				if (SelectedItems.Count > 0)
				{
					EventHandler modifiedAction = this.ModifiedAction;
					if (modifiedAction != null)
					{
						modifiedAction(this, EventArgs.Empty);
					}
				}
				foreach (object selectedItem in SelectedItems)
				{
					RemoveItem(selectedItem);
				}
				SelectedItems.Clear();
			}
			EndUpdate();
		}

		private void RemoveItem(object o)
		{
			IGraphNub graphNub = o as IGraphNub;
			if (graphNub != null)
			{
				graphNub.Owner.Nubs.Remove(graphNub);
				return;
			}
			IGraphSocket graphSocket = o as IGraphSocket;
			if (graphSocket != null)
			{
				graphSocket.Owner.Sockets.Remove(graphSocket);
				return;
			}
			IGraphNode graphNode = o as IGraphNode;
			if (graphNode != null)
			{
				graphNode.Owner.RemoveLinks(graphNode);
				graphNode.Owner.Nodes.Remove(graphNode);
			}
		}

		public static int GetTypeOrder(object a)
		{
			if (a is IGraphNub)
			{
				return 0;
			}
			if (a is IGraphSocket)
			{
				return 1;
			}
			if (a is IGraphNode)
			{
				return 2;
			}
			return 3;
		}

		public static int SelectionSort(object a, object b)
		{
			int num = GetTypeOrder(a) - GetTypeOrder(b);
			if (num == 0)
			{
				num = ((IUniqueID)a).ID - ((IUniqueID)b).ID;
			}
			return num;
		}

		private void PaintShadows(Graphics g)
		{
			foreach (IGraphNode node in Graph.Nodes)
			{
				NodeTag nodeTag = (NodeTag)node.Tag;
				nodeBrush.Color = Color.FromArgb(Math.Min(48, GetIsolatedAlpha(node)), Color.Black);
				RectangleF bounds = nodeTag.Bounds;
				bounds.Offset(6f, 6f);
				g.FillEllipse(nodeBrush, bounds);
			}
		}

		private void PaintSelection(Graphics g)
		{
			if (Graph == null)
			{
				return;
			}
			foreach (IGraphNode node in Graph.Nodes)
			{
				if (IsRubberSelected(node))
				{
					PaintSelection(g, node);
				}
				foreach (IGraphSocket socket in node.Sockets)
				{
					if (IsRubberSelected(socket))
					{
						PaintSelection(g, socket);
					}
					foreach (IGraphNub nub in socket.Nubs)
					{
						if (IsRubberSelected(nub))
						{
							PaintSelection(g, nub);
						}
					}
				}
			}
		}

		private void PaintSelection(Graphics g, object o)
		{
			IGraphNode graphNode = o as IGraphNode;
			if (graphNode != null)
			{
				NodeTag nodeTag = (NodeTag)graphNode.Tag;
				nodePen.Color = Color.FromArgb(GetIsolatedAlpha(graphNode), SelectedColor);
				nodePen.Width = 4f;
				RectangleF bounds = nodeTag.Bounds;
				float num = nodePen.Width * 1.5f;
				bounds.Inflate(num, num);
				g.DrawEllipse(nodePen, bounds);
			}
			IGraphSocket graphSocket = o as IGraphSocket;
			if (graphSocket != null)
			{
				SocketTag socketTag = (SocketTag)graphSocket.Tag;
				g.DrawLine(socketPenSel, socketTag.SegA.ToPointF, socketTag.SegB.ToPointF);
			}
			IGraphNub graphNub = o as IGraphNub;
			if (graphNub != null)
			{
				nodePen.Color = Color.FromArgb(GetIsolatedAlpha(graphNub), SelectedColor);
				nodePen.Width = 2f;
				NubTag nubTag = (NubTag)graphNub.Tag;
				RectangleF bounds2 = nubTag.Bounds;
				switch (graphNub.NubType)
				{
				case NubType.Condition:
				{
					float num2 = nodePen.Width * 1.5f;
					bounds2.Inflate(num2, num2);
					g.DrawEllipse(nodePen, bounds2);
					break;
				}
				case NubType.Event:
					DrawingHelper.DrawImageCentered(g, GameTuner.Framework.Properties.Resources.star_man_select, nubTag.Location.X, nubTag.Location.Y);
					break;
				case NubType.Operator:
					DrawingHelper.DrawImageCentered(g, GameTuner.Framework.Properties.Resources.or_man_select, nubTag.Location.X, nubTag.Location.Y);
					break;
				}
			}
		}

		public bool ValidLink(IGraphNode src, IGraphNode dst)
		{
			if (src == null || src == dst)
			{
				return false;
			}
			if (!MultiSockets && (HasLink(src, dst) || HasLink(dst, src)))
			{
				return false;
			}
			return true;
		}

		public bool HasLink(IGraphNode src, IGraphNode dst)
		{
			if (src != null)
			{
				foreach (IGraphSocket socket in src.Sockets)
				{
					IStateGraphSocket stateGraphSocket = (IStateGraphSocket)socket;
					if (stateGraphSocket.Node == dst)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void PaintNubs(Graphics g)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.LineAlignment = StringAlignment.Center;
			foreach (IGraphNode node in Graph.Nodes)
			{
				foreach (IGraphSocket socket in node.Sockets)
				{
					foreach (IGraphNub nub in socket.Nubs)
					{
						PaintNub(g, nub, stringFormat);
					}
				}
			}
		}

		private void PaintNub(Graphics g, IGraphNub nub, StringFormat sf)
		{
			NubTag nubTag = (NubTag)nub.Tag;
			RectangleF bounds = nubTag.Bounds;
			int isolatedAlpha = GetIsolatedAlpha(nub);
			nodeBrush.Color = Color.FromArgb(isolatedAlpha, Color.White);
			ColorMatrix m = ((isolatedAlpha == 255) ? SaturateMatrix : SaturateMatrixA);
			switch (nub.NubType)
			{
			case NubType.Condition:
				g.FillEllipse(nodeBrush, bounds);
				nodePen.Width = 2f;
				nodePen.Color = Color.FromArgb(isolatedAlpha, NubColor).Desaturate(AllowEdit ? 1f : 0.125f);
				g.DrawEllipse(nodePen, bounds);
				break;
			case NubType.Event:
				if (AllowEdit)
				{
					DrawingHelper.DrawImageCentered(g, GameTuner.Framework.Properties.Resources.star_man, nubTag.Location.X, nubTag.Location.Y, Color.FromArgb(isolatedAlpha, nodeBrush.Color));
				}
				else
				{
					DrawingHelper.DrawImageCentered(g, GameTuner.Framework.Properties.Resources.star_man, nubTag.Location.X, nubTag.Location.Y, m);
				}
				break;
			case NubType.Operator:
				if (AllowEdit)
				{
					DrawingHelper.DrawImageCentered(g, GameTuner.Framework.Properties.Resources.or_man, nubTag.Location.X, nubTag.Location.Y, Color.FromArgb(isolatedAlpha, nodeBrush.Color));
				}
				else
				{
					DrawingHelper.DrawImageCentered(g, GameTuner.Framework.Properties.Resources.or_man, nubTag.Location.X, nubTag.Location.Y, m);
				}
				break;
			}
			if (ShowNubLabels)
			{
				g.DrawString(nub.Text, base.ScaledFont, nodeBrush, nubTag.Location.X + nubTag.Radius + 2f, nubTag.Location.Y, sf);
			}
		}

		private void PaintSockets(Graphics g)
		{
			foreach (IGraphNode node in Graph.Nodes)
			{
				PaintSockets(g, node);
			}
			if (drag.Mode == DragMode.NodeNewLink)
			{
				Vec2 point = drag.Current.ToVec2;
				ClientToWorld(ref point);
				NodeTag nodeTag = (NodeTag)((IGraphNode)drag.Item).Tag;
				IGraphNode graphNode = FindNode(point);
				if (graphNode != null && graphNode != drag.Item && ValidLink((IGraphNode)drag.Item, graphNode))
				{
					NodeTag nodeTag2 = (NodeTag)graphNode.Tag;
					float rad = (float)Math.Atan2(nodeTag.Location.Y - nodeTag2.Location.Y, nodeTag.Location.X - nodeTag2.Location.X);
					float s;
					float c;
					MathEx.SinCos(rad, out s, out c);
					PointF toPointF = nodeTag2.Location.ToPointF;
					toPointF.X += c * nodeTag2.Radius;
					toPointF.Y += s * nodeTag2.Radius;
					g.DrawLine(socketPen, nodeTag.Location.ToPointF, toPointF);
				}
				else
				{
					socketPen.DashStyle = DashStyle.Dash;
					socketPen.Width = 1f;
					g.DrawLine(socketPen, nodeTag.Location.ToPoint, point.ToPoint);
					socketPen.DashStyle = DashStyle.Solid;
					socketPen.Width = 4f;
				}
			}
		}

		private int GetIsolatedAlpha(object item)
		{
			int num = ((drag.Mode == DragMode.RubberBox) ? (SelectedItems.Count + RubberItems.Count) : SelectedItems.Count);
			if (!ShowIsolatedStates || num <= 0 || IsNextToSelected(item))
			{
				return 255;
			}
			return 16;
		}

		private Color GetIsolatedColor(object item, Color color)
		{
			return Color.FromArgb(GetIsolatedAlpha(item), color).Desaturate(AllowEdit ? 1f : 0.125f);
		}

		private void PaintSockets(Graphics g, IGraphNode node)
		{
			Color color = socketPen.Color;
			foreach (IGraphSocket socket in node.Sockets)
			{
				socketPen.Color = GetIsolatedColor(socket, color);
				SocketTag socketTag = socket.Tag as SocketTag;
				g.DrawLine(socketPen, socketTag.SegA.ToPointF, socketTag.SegB.ToPointF);
			}
			socketPen.Color = color;
		}

		private void PaintNodes(Graphics g)
		{
			IStateGraph stateGraph = Graph as IStateGraph;
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			SolidBrush solidBrush = new SolidBrush(Color.White);
			SolidBrush solidBrush2 = new SolidBrush(Color.Black);
			StateGraphPaintEventArgs e = new StateGraphPaintEventArgs(g, stringFormat, Font, nodePen, SelectedColor, Color.Black, NodeColor.Desaturate(AllowEdit ? 1f : 0.125f), DefaultNodeColor.Desaturate(AllowEdit ? 1f : 0.125f), Color.White);
			e.EdgeWidth = 4f;
			e.LabelWidth = 3f;
			e.FillBrush = solidBrush;
			e.TextBrush = solidBrush2;
			e.TextLabelBrush = nodeBrush;
			foreach (IGraphNode node in Graph.Nodes)
			{
				e.Alpha = GetIsolatedAlpha(node);
				nodeBrush.Color = Color.FromArgb(Math.Min(128, e.Alpha), Color.White);
				solidBrush.Color = Color.FromArgb(e.Alpha, Color.White);
				solidBrush2.Color = Color.FromArgb(e.Alpha, Color.Black);
				e.Pen.Color = Color.FromArgb(e.Alpha, (stateGraph.DefaultState == node) ? e.DefaultEdgeColor : e.EdgeColor).Desaturate(AllowEdit ? 1f : 0.125f);
				e.Node = node;
				PaintNode(this, e);
			}
		}

		public void DefaultPaintHandler(object sender, StateGraphPaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			IGraph owner = e.Node.Owner;
			PointF empty = PointF.Empty;
			NodeTag nodeTag = (NodeTag)e.Node.Tag;
			empty.X = nodeTag.Location.X;
			empty.Y = nodeTag.Location.Y;
			SizeF sizeF = graphics.MeasureString(e.Node.Text, e.Font, empty, e.StringFormat);
			RectangleF empty2 = RectangleF.Empty;
			empty2.X = empty.X - sizeF.Width * 0.5f;
			empty2.Y = empty.Y - sizeF.Height * 0.5f;
			empty2.Width = sizeF.Width;
			empty2.Height = sizeF.Height;
			if (ShowNodeLabels)
			{
				e.Pen.Width = e.LabelWidth;
				graphics.DrawRectangle(e.Pen, empty2.X, empty2.Y, empty2.Width, empty2.Height);
			}
			e.Pen.Width = e.EdgeWidth;
			graphics.FillEllipse(e.FillBrush, nodeTag.Bounds);
			graphics.DrawEllipse(e.Pen, nodeTag.Bounds);
			IGraphDataProvider graphDataProvider = e.Node as IGraphDataProvider;
			if (graphDataProvider != null && graphDataProvider.GraphDataImage != null)
			{
				DrawingHelper.DrawImage(graphics, graphDataProvider.GraphDataImage, nodeTag.DataBounds.X, nodeTag.DataBounds.Y, Color.FromArgb(e.Alpha, Color.White));
			}
			if (ShowNodeLabels)
			{
				graphics.FillRectangle(e.TextLabelBrush, empty2);
				graphics.DrawString(e.Node.Text, e.Font, e.TextBrush, empty.X, empty.Y, e.StringFormat);
				IGraphCaptionProvider graphCaptionProvider = e.Node as IGraphCaptionProvider;
				if (graphCaptionProvider != null && !string.IsNullOrEmpty(graphCaptionProvider.GraphCaption))
				{
					empty.Y -= nodeTag.Radius + 14f;
					string graphCaption = graphCaptionProvider.GraphCaption;
					sizeF = graphics.MeasureString(graphCaption, base.ScaledFont, empty, e.StringFormat);
					empty2 = RectangleF.Empty;
					empty2.X = empty.X - sizeF.Width * 0.5f;
					empty2.Y = empty.Y - sizeF.Height * 0.5f;
					empty2.Width = sizeF.Width;
					empty2.Height = sizeF.Height;
					graphics.FillRectangle(e.TextLabelBrush, empty2);
					graphics.DrawString(graphCaption, base.ScaledFont, e.TextBrush, empty.X, empty.Y, e.StringFormat);
				}
			}
		}

		private void OnRightButtonDown(MouseEventArgs e)
		{
			focusItem = Find(e.X, e.Y);
			ctrlPressed = KeyHelper.CtrlPressed;
			BeginUpdate();
			if (focusItem != null)
			{
				if (!ctrlPressed && !IsSelected(focusItem))
				{
					SelectedItems.Clear();
				}
				SelectItem(focusItem, true);
			}
			else if (!ctrlPressed)
			{
				SelectedItems.Clear();
			}
			EndUpdate();
		}

		private void OnLeftButtonDown(MouseEventArgs e)
		{
			ctrlPressed = KeyHelper.CtrlPressed;
			shiftPressed = KeyHelper.ShiftPressed;
			drag.Ctrl = ctrlPressed;
			drag.Shift = shiftPressed;
			focusItem = Find(e.X, e.Y);
			if (focusItem != null && focusItem is IGraphDataProvider)
			{
				IGraphNode graphNode = (IGraphNode)focusItem;
				NodeTag nodeTag = (NodeTag)graphNode.Tag;
				Vec2 point = new Vec2(e.X, e.Y);
				ClientToWorld(ref point);
				if (nodeTag.DataBounds.Contains(point.ToPointF))
				{
					StateGraphHandler dataProviderClicked = this.DataProviderClicked;
					if (dataProviderClicked != null)
					{
						dataProviderClicked(this, new StateGraphEventArgs(focusItem, e));
					}
					return;
				}
			}
			BeginUpdate();
			if (focusItem != null)
			{
				if (ctrlPressed)
				{
					SelectItem(focusItem, !IsSelected(focusItem));
				}
				else
				{
					if (!IsSelected(focusItem))
					{
						SelectedItems.Clear();
					}
					SelectItem(focusItem, true);
				}
				if (AllowEdit)
				{
					if (focusItem is IGraphNode)
					{
						base.Capture = true;
						drag.Item = focusItem;
						drag.Pos.X = e.X;
						drag.Pos.Y = e.Y;
						drag.Current = drag.Pos;
						drag.Mode = ((!shiftPressed) ? DragMode.Node : DragMode.NodeNewLink);
						drag.Moved = false;
					}
					else if (focusItem is IGraphNub)
					{
						base.Capture = true;
						drag.Item = focusItem;
						drag.Pos.X = e.X;
						drag.Pos.Y = e.Y;
						drag.Current = drag.Pos;
						drag.Mode = DragMode.Nub;
						drag.Moved = false;
					}
				}
			}
			else
			{
				if (!ctrlPressed)
				{
					SelectedItems.Clear();
				}
				base.Capture = true;
				drag.Pos.X = e.X;
				drag.Pos.Y = e.Y;
				drag.Mode = DragMode.RubberBox;
				drag.Rect = RectangleF.Empty;
			}
			EndUpdate();
		}

		public void SelectItem(object item, bool select)
		{
			SelectItem(item, select, SelectedItems);
		}

		public bool IsSelected(object item)
		{
			return IsSelected(item, SelectedItems);
		}

		private bool IsSelected(object item, ListEvent<object> selectList)
		{
			return selectList.Contains(item);
		}

		private bool IsRubberSelected(object item)
		{
			bool flag = false;
			if (drag.Mode == DragMode.RubberBox)
			{
				bool flag2 = IsSelected(item);
				bool flag3 = IsSelected(item, RubberItems);
				if (drag.Ctrl)
				{
					return (flag2 && !flag3) || (flag3 && !flag2);
				}
				return flag2 || flag3;
			}
			return IsSelected(item);
		}

		private void SelectItem(object item, bool select, ListEvent<object> selectList)
		{
			if (select)
			{
				if (!IsSelected(item))
				{
					selectList.Add(item);
				}
			}
			else if (IsSelected(item))
			{
				selectList.Remove(item);
			}
		}

		protected override bool HandleMouseDown(object sender, MouseEventArgs e)
		{
			if (base.HandleMouseDown(sender, e))
			{
				return true;
			}
			bool result = false;
			if (drag.Mode == DragMode.Idle)
			{
				switch (e.Button)
				{
				case MouseButtons.Left:
					OnLeftButtonDown(e);
					result = true;
					break;
				case MouseButtons.Right:
					OnRightButtonDown(e);
					result = true;
					break;
				}
			}
			return result;
		}

		protected override bool HandleMouseMove(object sender, MouseEventArgs e)
		{
			if (base.HandleMouseMove(sender, e))
			{
				return true;
			}
			bool result = false;
			mousePoint.X = e.X;
			mousePoint.Y = e.Y;
			switch (drag.Mode)
			{
			case DragMode.Node:
				OnDragNode(e);
				result = true;
				break;
			case DragMode.Nub:
				OnDragNub(e);
				result = true;
				break;
			case DragMode.NodeNewLink:
				OnDragNewLink(e);
				result = true;
				break;
			case DragMode.RubberBox:
				OnDragRubberBox(e);
				result = true;
				break;
			}
			return result;
		}

		private void OnDragNub(MouseEventArgs e)
		{
			if (!drag.Moved)
			{
				drag.Moved = true;
				EventHandler modifiedAction = this.ModifiedAction;
				if (modifiedAction != null)
				{
					modifiedAction(this, EventArgs.Empty);
				}
				if (!IsSelected(focusItem))
				{
					SelectItem(focusItem, true);
				}
				InitSelectionAnchor();
				EventHandler moveNubBegin = this.MoveNubBegin;
				if (moveNubBegin != null)
				{
					moveNubBegin(this, EventArgs.Empty);
				}
			}
			OffsetSelectedNubs(e.X - drag.Pos.X, e.Y - drag.Pos.Y);
			drag.Pos.X = e.X;
			drag.Pos.Y = e.Y;
		}

		private void OnDragNewLink(MouseEventArgs e)
		{
			drag.Moved = true;
			drag.Current = mousePoint;
			Invalidate();
		}

		private void OnDragNode(MouseEventArgs e)
		{
			if (!drag.Moved)
			{
				drag.Moved = true;
				EventHandler modifiedAction = this.ModifiedAction;
				if (modifiedAction != null)
				{
					modifiedAction(this, EventArgs.Empty);
				}
				if (!IsSelected(focusItem))
				{
					SelectItem(focusItem, true);
				}
				InitSelectionAnchor();
				EventHandler moveNodeBegin = this.MoveNodeBegin;
				if (moveNodeBegin != null)
				{
					moveNodeBegin(this, EventArgs.Empty);
				}
			}
			OffsetSelectedNodes(e.X - drag.Pos.X, e.Y - drag.Pos.Y);
			drag.Pos.X = e.X;
			drag.Pos.Y = e.Y;
		}

		private void InitSelectionAnchor()
		{
			foreach (object selectedItem in SelectedItems)
			{
				if (selectedItem is IGraphNode)
				{
					IGraphNode graphNode = (IGraphNode)selectedItem;
					NodeTag nodeTag = (NodeTag)graphNode.Tag;
					nodeTag.Anchor = graphNode.Location;
				}
				if (selectedItem is IGraphNub)
				{
					IGraphNub graphNub = (IGraphNub)selectedItem;
					NubTag nubTag = (NubTag)graphNub.Tag;
					nubTag.Anchor = nubTag.Location;
				}
			}
		}

		private void OffsetSelectedNubs(int dx, int dy)
		{
			Vec2 vec = new Vec2(dx, dy);
			vec /= base.Zoom;
			foreach (object selectedItem in SelectedItems)
			{
				if (selectedItem is IGraphNub)
				{
					IGraphNub graphNub = (IGraphNub)selectedItem;
					NubTag nubTag = (NubTag)graphNub.Tag;
					SocketTag socketTag = (SocketTag)graphNub.Owner.Tag;
					nubTag.Anchor += vec;
					Vec2 point = Vec2.NearestPoint(nubTag.Anchor, socketTag.SegA, socketTag.SegB);
					graphNub.Order = Math.Max(Math.Min(Vec2.FloatPointLine(point, socketTag.SegA, socketTag.SegB), 1f), 0f);
				}
			}
			Invalidate();
			Update();
		}

		public float FindOrder(IGraphSocket socket, Vec2 point)
		{
			SocketTag socketTag = (SocketTag)socket.Tag;
			Vec2 point2 = Vec2.NearestPoint(point, socketTag.SegA, socketTag.SegB);
			return Vec2.FloatPointLine(point2, socketTag.SegA, socketTag.SegB);
		}

		private void OffsetSelectedNodes(int dx, int dy)
		{
			Vec2 vec = new Vec2(dx, dy);
			vec /= base.Zoom;
			foreach (object selectedItem in SelectedItems)
			{
				if (selectedItem is IGraphNode)
				{
					IGraphNode graphNode = (IGraphNode)selectedItem;
					NodeTag nodeTag = (NodeTag)graphNode.Tag;
					nodeTag.Anchor += vec;
					graphNode.Location = GetSnapPoint(nodeTag.Anchor);
				}
			}
			Invalidate();
			Update();
		}

		private Vec2 GetSnapPoint(Vec2 point)
		{
			Vec2 result = point;
			if (SnapToGrid)
			{
				result.X = (int)point.X / base.GridSpacing.Width * base.GridSpacing.Width;
				result.Y = (int)point.Y / base.GridSpacing.Height * base.GridSpacing.Height;
			}
			return result;
		}

		private void OnDragRubberBox(MouseEventArgs e)
		{
			drag.Rect = drag.Rect.SetEdges(e.X, e.Y, drag.Pos.X, drag.Pos.Y);
			RectangleF rect = drag.Rect;
			ClientToWorld(ref rect);
			RubberItems.Clear();
			if (Graph != null)
			{
				foreach (IGraphNode node in Graph.Nodes)
				{
					if (Intersect(node, rect))
					{
						SelectItem(node, true, RubberItems);
					}
				}
			}
			Invalidate();
		}

		private bool Intersect(IGraphNode node, RectangleF rect)
		{
			NodeTag nodeTag = node.Tag as NodeTag;
			return nodeTag.Bounds.IntersectsWith(rect);
		}

		protected override bool HandleSetCursor(object sender, MouseEventArgs e)
		{
			if (base.HandleSetCursor(sender, e))
			{
				return true;
			}
			object obj = Find(e.X, e.Y);
			if (obj != null)
			{
				Cursor = CustomCursors.Get(CustomCursor.FingerPoint);
				return true;
			}
			return false;
		}

		protected override bool HandleMouseUp(object sender, MouseEventArgs e)
		{
			if (base.HandleMouseUp(sender, e))
			{
				return true;
			}
			bool result = false;
			if (e.Button == MouseButtons.Right)
			{
				StateGraphHandler graphContextMenu = this.GraphContextMenu;
				if (graphContextMenu != null)
				{
					StateGraphEventArgs e2 = new StateGraphEventArgs(focusItem, e);
					graphContextMenu(this, e2);
				}
			}
			if (e.Button == MouseButtons.Left)
			{
				switch (drag.Mode)
				{
				case DragMode.Nub:
					OnDragNubEnd(e);
					result = true;
					break;
				case DragMode.Node:
					OnDragNodeEnd(e);
					result = true;
					break;
				case DragMode.NodeNewLink:
					OnDragNewLinkEnd(e);
					result = true;
					break;
				case DragMode.RubberBox:
					OnRubberBoxEnd(e);
					result = true;
					break;
				}
			}
			return result;
		}

		private void OnRubberBoxEnd(MouseEventArgs e)
		{
			base.Capture = false;
			BeginUpdate();
			if (!drag.Ctrl)
			{
				SelectedItems.Clear();
				SelectedItems.InsertRange(0, RubberItems);
			}
			else
			{
				foreach (object rubberItem in RubberItems)
				{
					if (!IsSelected(rubberItem))
					{
						SelectedItems.Add(rubberItem);
					}
				}
			}
			RubberItems.Clear();
			drag.Mode = DragMode.Idle;
			EndUpdate();
		}

		private void OnDragNubEnd(MouseEventArgs e)
		{
			List<IGraphSocket> list = new List<IGraphSocket>();
			foreach (object selectedItem in SelectedItems)
			{
				IGraphNub graphNub = selectedItem as IGraphNub;
				if (graphNub != null && !list.Contains(graphNub.Owner))
				{
					list.Add(graphNub.Owner);
				}
			}
			foreach (IGraphSocket item in list)
			{
				item.Nubs.Sort((IGraphNub a, IGraphNub b) => Math.Sign(a.Order - b.Order));
			}
			EventHandler moveNubEnd = this.MoveNubEnd;
			if (moveNubEnd != null)
			{
				moveNubEnd(this, EventArgs.Empty);
			}
			drag.Mode = DragMode.Idle;
			base.Capture = false;
			Invalidate();
		}

		private void OnDragNewLinkEnd(MouseEventArgs e)
		{
			Vec2 point = drag.Current.ToVec2;
			ClientToWorld(ref point);
			IGraphNode graphNode = FindNode(point);
			if (graphNode != null && graphNode != drag.Item && ValidLink((IGraphNode)drag.Item, graphNode))
			{
				StateGraphHandler nodesLinking = this.NodesLinking;
				if (nodesLinking != null)
				{
					StateGraphEventArgs e2 = new StateGraphEventArgs((IGraphNode)drag.Item, graphNode);
					nodesLinking(this, e2);
					if (e2.Cancel)
					{
						return;
					}
				}
				EventHandler modifiedAction = this.ModifiedAction;
				if (modifiedAction != null)
				{
					modifiedAction(this, EventArgs.Empty);
				}
				IGraphNode graphNode2 = (IGraphNode)drag.Item;
				IStateGraphNode stateGraphNode = (IStateGraphNode)graphNode2;
				IGraphSocket graphSocket = stateGraphNode.CreateSocket();
				graphNode2.Sockets.Add(graphSocket);
				IStateGraphSocket stateGraphSocket = (IStateGraphSocket)graphSocket;
				stateGraphSocket.Connect(graphNode);
				nodesLinking = this.NodesLinked;
				if (nodesLinking != null)
				{
					StateGraphEventArgs e3 = new StateGraphEventArgs(graphSocket, e);
					nodesLinking(this, e3);
				}
			}
			drag.Mode = DragMode.Idle;
			base.Capture = false;
			Invalidate();
		}

		private void OnDragNodeEnd(MouseEventArgs e)
		{
			EventHandler moveNodeEnd = this.MoveNodeEnd;
			if (moveNodeEnd != null)
			{
				moveNodeEnd(this, EventArgs.Empty);
			}
			drag.Mode = DragMode.Idle;
			base.Capture = false;
			Invalidate();
		}

		public bool IsNextToSelected(object item)
		{
			if (IsRubberSelected(item))
			{
				return true;
			}
			if (item is IGraphNode)
			{
				IGraphNode graphNode = (IGraphNode)item;
				foreach (IGraphSocket socket in graphNode.Sockets)
				{
					if (IsRubberSelected(socket))
					{
						return true;
					}
					IGraphNode node = ((IStateGraphSocket)socket).Node;
					if (node != null && IsRubberSelected(node))
					{
						return true;
					}
					foreach (IGraphNub nub in socket.Nubs)
					{
						if (IsRubberSelected(nub))
						{
							return true;
						}
					}
				}
				foreach (IGraphNode node4 in graphNode.Owner.Nodes)
				{
					foreach (IGraphSocket socket2 in node4.Sockets)
					{
						IGraphNode node2 = ((IStateGraphSocket)socket2).Node;
						if (node2 != graphNode)
						{
							continue;
						}
						if (IsRubberSelected(node4) || IsRubberSelected(socket2))
						{
							return true;
						}
						foreach (IGraphNub nub2 in socket2.Nubs)
						{
							if (IsRubberSelected(nub2))
							{
								return true;
							}
						}
					}
				}
			}
			else if (item is IGraphSocket)
			{
				IGraphSocket graphSocket = (IGraphSocket)item;
				foreach (IGraphNub nub3 in graphSocket.Nubs)
				{
					if (IsRubberSelected(nub3))
					{
						return true;
					}
				}
				if (IsRubberSelected(graphSocket.Owner))
				{
					return true;
				}
				IGraphNode node3 = ((IStateGraphSocket)item).Node;
				if (node3 != null && IsRubberSelected(node3))
				{
					return true;
				}
			}
			else if (item is IGraphNub && IsNextToSelected(((IGraphNub)item).Owner))
			{
				return true;
			}
			return false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.DoubleBuffered = true;
			base.Name = "StateGraphControl";
			base.Size = new System.Drawing.Size(242, 226);
			base.ResumeLayout(false);
		}
	}
}
