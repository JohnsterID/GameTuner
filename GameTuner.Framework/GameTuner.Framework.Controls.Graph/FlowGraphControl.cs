using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GameTuner.Framework.Graph;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework.Controls.Graph
{
	public class FlowGraphControl : WorkspaceControl
	{
		private enum DragMode
		{
			Idle,
			Node,
			RubberBox,
			NewLink,
			MoveLink,
			Slider
		}

		private struct Connection
		{
			public IGraphNode Node;

			public IGraphSocket Socket;

			private static Connection empty = default(Connection);

			public static Connection Empty
			{
				get
				{
					return empty;
				}
			}

			public Connection(IGraphSocket socket)
			{
				Socket = socket;
				Node = Socket.Owner;
			}
		}

		private class NodeTag
		{
			public List<RectangleF> Entries { get; private set; }

			public bool HasCaption { get; set; }

			public bool ShowValue { get; set; }

			public RectangleF Bounds { get; set; }

			public RectangleF Append { get; set; }

			public Vec2 Location { get; set; }

			public Vec2 Anchor { get; set; }

			public RectangleF ValueRect { get; set; }

			public NodeTag()
			{
				Append = RectangleF.Empty;
				Bounds = RectangleF.Empty;
				ValueRect = RectangleF.Empty;
				Location = Vec2.Empty;
				Anchor = Vec2.Empty;
				Entries = new List<RectangleF>();
			}
		}

		private class SocketTag
		{
			public RectangleF Bounds { get; set; }

			public RectangleF LinkBounds { get; set; }

			public PointF LabelPoint { get; set; }

			public PointF AttachPoint { get; set; }

			public SocketTag()
			{
				Bounds = RectangleF.Empty;
				LabelPoint = PointF.Empty;
				AttachPoint = PointF.Empty;
				LinkBounds = RectangleF.Empty;
			}
		}

		public delegate void FlowGraphHandler(object sender, FlowGraphEventArgs e);

		private const int CaptionHeight = 17;

		private const int PadX = 5;

		private const int PadY = 5;

		private const int NipWidth = 6;

		private const int NipHeight = 9;

		private const int ShadowOffset = 5;

		private const int RectCorner = 4;

		private const int ConstantWidth = 16;

		private const float Saturate = 0.125f;

		private IGraph graph;

		private bool selectWhileUpdate;

		private readonly SizeF MinNodeSize = new SizeF(128f, 32f);

		private DragMode dragMode;

		private bool dragCtrl;

		private bool dragMoved;

		private bool ctrlPressed;

		private bool allowEdit;

		private Point2 dragPos;

		private Point2 dragCurrent;

		private PointF dragAttach;

		private RectangleF dragRect;

		private Connection dragSave;

		private object dragItem;

		private IGraphNode hotNode;

		private Vec2 hotNodePoint;

		private object focusItem;

		private IGraphSocket focusSocket;

		private IGraphSocket movedSocket;

		private BlockCurvePopup bcPopup;

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

		[Category("Graph")]
		[DefaultValue(true)]
		public bool SnapToGrid { get; set; }

		[Category("Appearance")]
		public Color SelectedColor { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public ListEvent<object> SelectedItems { get; private set; }

		private ListEvent<object> RubberItems { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
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

		public event FlowGraphHandler GraphContextMenu;

		public event FlowGraphHandler NewSocketConnection;

		public event FlowGraphHandler MovedSocketConnection;

		public event FlowGraphHandler RemoveSocketConnection;

		public event EventHandler SelectedItemChanged;

		public event EventHandler ModifiedAction;

		public event EventHandler MoveNodeBegin;

		public event EventHandler MoveNodeEnd;

		public event CancelEventHandler SelectedItemsDelete;

		public FlowGraphControl()
		{
			InitializeComponent();
			SelectedColor = Color.FromArgb(255, 193, 42);
			SnapToGrid = true;
			hotNodePoint = Vec2.Empty;
			dragSave = Connection.Empty;
			bcPopup = new BlockCurvePopup();
			bcPopup.BeginModifiedAction += bcPopup_BeginModifiedAction;
			bcPopup.EndModifiedAction += bcPopup_EndModifiedAction;
			RubberItems = new ListEvent<object>();
			SelectedItems = new ListEvent<object>();
			SelectedItems.ItemCountChanged += SelectedItems_ItemCountChanged;
			base.InvokedEndUpdate += FlowGraphControl_InvokedEndUpdate;
		}

		public void RemoveSelectedItems()
		{
			if (SelectedItems.Count <= 0)
			{
				return;
			}
			bool flag = false;
			BeginUpdate();
			CancelEventHandler selectedItemsDelete = this.SelectedItemsDelete;
			if (selectedItemsDelete != null)
			{
				CancelEventArgs e = new CancelEventArgs(false);
				selectedItemsDelete(this, e);
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
				foreach (IGraphNode selectedItem in SelectedItems)
				{
					FlowGraphNodeStyleAttribute attribute = ReflectionHelper.GetAttribute<FlowGraphNodeStyleAttribute>(selectedItem);
					if (attribute == null || (attribute.Style & FlowGraphNodeStyle.NoDelete) == 0)
					{
						Graph.RemoveLinks(selectedItem);
						Graph.Nodes.Remove(selectedItem);
					}
				}
				SelectedItems.Clear();
			}
			EndUpdate();
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
				SizeF sizeF = CalcNodeSize(g, node);
				empty2.Width = sizeF.Width;
				empty2.Height = sizeF.Height;
				empty2.X = empty.X - sizeF.Width * 0.5f;
				empty2.Y = empty.Y - sizeF.Height * 0.5f;
				nodeTag.Bounds = empty2;
				nodeTag.Location = empty;
				nodeTag.HasCaption = ShowCaption(node);
				nodeTag.ShowValue = ShowValue(node);
				if (nodeTag.ShowValue)
				{
					RectangleF valueRect = empty2;
					valueRect.X = empty2.X + 5f;
					valueRect.Y = empty2.Bottom - 2.5f - (float)base.FontHeight;
					valueRect.Height = base.FontHeight;
					nodeTag.ValueRect = valueRect;
				}
				IFlowGraphDynamicEntries flowGraphDynamicEntries = node as IFlowGraphDynamicEntries;
				if (flowGraphDynamicEntries != null)
				{
					nodeTag.Entries.Clear();
					int entryCount = flowGraphDynamicEntries.EntryCount;
					float num = empty2.Y + (float)base.FontHeight + 5f;
					DynamicEntriesAttribute attribute = ReflectionHelper.GetAttribute<DynamicEntriesAttribute>(node);
					if (attribute != null)
					{
						num += (float)(attribute.StartIndex * base.FontHeight);
					}
					int num2 = 0;
					while (num2 < entryCount)
					{
						nodeTag.Entries.Add(new RectangleF(empty2.X, num, empty2.Width, base.FontHeight));
						num2++;
						num += (float)base.FontHeight;
					}
					nodeTag.Append = new RectangleF(empty2.X, num, GameTuner.Framework.Properties.Resources.graph_append.Width, GameTuner.Framework.Properties.Resources.graph_append.Height);
				}
				IFlowGraphNode flowGraphNode = (IFlowGraphNode)node;
				CalcSocketRects(g, node, flowGraphNode.InputSockets, FlowGraphSocketType.Input);
				CalcSocketRects(g, node, flowGraphNode.OutputSockets, FlowGraphSocketType.Output);
				rectangleF = (rectangleF.HasValue ? new RectangleF?(RectangleF.Union(rectangleF.Value, empty2)) : new RectangleF?(empty2));
			}
			NodeBounds = (rectangleF.HasValue ? rectangleF.Value : RectangleF.Empty);
		}

		private void CalcSocketRects(Graphics g, IGraphNode node, GraphSocketCollection sockets, FlowGraphSocketType type)
		{
			NodeTag nodeTag = (NodeTag)node.Tag;
			Vec2 vec = new Vec2(nodeTag.Bounds.X, nodeTag.Bounds.Y);
			int num = GameTuner.Framework.Properties.Resources.graph_link.Width;
			int num2 = GameTuner.Framework.Properties.Resources.graph_link.Height;
			if (type == FlowGraphSocketType.Output)
			{
				float num3 = nodeTag.Bounds.Height;
				if (nodeTag.HasCaption)
				{
					vec.Y += 17f;
					num3 -= 17f;
				}
				float num4 = ((sockets.Count > 1) ? ((float)num2 * 0.5f) : 0f);
				vec.Y += num3 * 0.5f - (float)(sockets.Count * num2) * 0.5f - num4;
			}
			else if (nodeTag.HasCaption)
			{
				vec.Y += 17f;
			}
			RectangleF empty = RectangleF.Empty;
			foreach (IGraphSocket socket in sockets)
			{
				SocketTag socketTag = socket.Tag as SocketTag;
				if (socketTag == null)
				{
					socketTag = (SocketTag)(socket.Tag = new SocketTag());
				}
				if (type == FlowGraphSocketType.Input)
				{
					empty.X = nodeTag.Bounds.X + 2f;
					empty.Y = vec.Y + 2f;
					empty.Width = 6f;
					empty.Height = 9f;
					vec.Y += base.FontHeight;
					socketTag.AttachPoint = new PointF(empty.X - 14f, empty.Y + empty.Height * 0.5f);
					RectangleF linkBounds = empty;
					linkBounds.Offset(-15f, -3f);
					socketTag.LinkBounds = linkBounds;
				}
				else
				{
					empty.X = nodeTag.Bounds.Right;
					empty.Y = vec.Y;
					empty.Width = num;
					empty.Height = num2;
					vec.Y += num2;
					socketTag.AttachPoint = new PointF(empty.Right - 2f, empty.Y + empty.Height * 0.5f);
				}
				socketTag.Bounds = empty;
				socketTag.LabelPoint = new PointF(empty.Right + 4f, empty.Y + empty.Height * 0.5f);
			}
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

		private void FlowGraphControl_InvokedEndUpdate(object sender, EventArgs e)
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

		private bool ShowCaption(IGraphNode node)
		{
			FlowGraphNodeStyleAttribute attribute = ReflectionHelper.GetAttribute<FlowGraphNodeStyleAttribute>(node);
			if (attribute != null)
			{
				return (attribute.Style & FlowGraphNodeStyle.Constant) == 0;
			}
			return true;
		}

		private bool IsConstant(IGraphNode node)
		{
			FlowGraphNodeStyleAttribute attribute = ReflectionHelper.GetAttribute<FlowGraphNodeStyleAttribute>(node);
			if (attribute != null)
			{
				return (attribute.Style & FlowGraphNodeStyle.Constant) != 0;
			}
			return false;
		}

		private bool ShowValue(IGraphNode node)
		{
			if (node != null)
			{
				if (!(node is IDisplayValue))
				{
					return node is IFlowGraphCustomValue;
				}
				return true;
			}
			return false;
		}

		public bool IsBlockCurve(IGraphNode node)
		{
			if (node != null)
			{
				return node is IFlowGraphBlockCurve;
			}
			return false;
		}

		public bool IsDynamicEntry(IGraphNode node)
		{
			if (node != null)
			{
				return node is IFlowGraphDynamicEntries;
			}
			return false;
		}

		private int HitRemove(IGraphNode node, int x, int y)
		{
			return HitRemove(node, x, y, true);
		}

		private int HitRemove(IGraphNode node, int x, int y, bool buttonOnly)
		{
			if (AllowEdit && IsDynamicEntry(node))
			{
				Vec2 point = new Vec2(x, y);
				ClientToWorld(ref point);
				NodeTag nodeTag = (NodeTag)node.Tag;
				int num = 0;
				foreach (RectangleF entry in nodeTag.Entries)
				{
					if (entry.Contains(point.X, point.Y) && (!buttonOnly || point.X >= entry.Right - (float)GameTuner.Framework.Properties.Resources.graph_remove.Width))
					{
						return num;
					}
					num++;
				}
			}
			return -1;
		}

		private bool HitAppend(IGraphNode node, int x, int y)
		{
			if (AllowEdit && IsDynamicEntry(node))
			{
				Vec2 point = new Vec2(x, y);
				ClientToWorld(ref point);
				return ((NodeTag)node.Tag).Append.Contains(point.X, point.Y);
			}
			return false;
		}

		private bool HitBlockCurve(IGraphNode node, int x, int y)
		{
			if (AllowEdit && IsBlockCurve(node))
			{
				Vec2 point = new Vec2(x, y);
				ClientToWorld(ref point);
				return GetBlockCurveRect(node).Contains(point.X, point.Y);
			}
			return false;
		}

		private RectangleF GetBlockCurveRect(IGraphNode node)
		{
			RectangleF result = RectangleF.Empty;
			if (IsBlockCurve(node))
			{
				result = ((NodeTag)node.Tag).Bounds;
				int num = 69;
				result.X += result.Width * 0.5f - (float)num * 0.5f;
				result.Y = result.Bottom - 29f;
				result.Width = num;
				result.Height = 24f;
			}
			return result;
		}

		private SizeF CalcNodeSize(Graphics g, IGraphNode node)
		{
			IFlowGraphNode flowGraphNode = (IFlowGraphNode)node;
			bool flag = ShowCaption(node);
			bool flag2 = ShowValue(node);
			int num = (flag ? 17 : 0);
			int fontHeight = base.FontHeight;
			SizeF empty = SizeF.Empty;
			int num2 = Math.Max(flowGraphNode.InputSockets.Count, flowGraphNode.OutputSockets.Count);
			empty.Width = MinNodeSize.Width;
			empty.Height = Math.Max(MinNodeSize.Height, num2 * fontHeight + num + 5);
			if (IsBlockCurve(node))
			{
				empty.Height += 32f;
			}
			if (flowGraphNode.InputSockets.Count != 0 && flag2)
			{
				empty.Height += (float)fontHeight - 2.5f;
			}
			IFlowGraphDynamicEntries flowGraphDynamicEntries = node as IFlowGraphDynamicEntries;
			if (flowGraphDynamicEntries != null)
			{
				float num3 = (float)(flowGraphDynamicEntries.EntryCount * fontHeight) + (float)fontHeight * 2f + 5f + (flag2 ? ((float)fontHeight) : 0f);
				DynamicEntriesAttribute attribute = ReflectionHelper.GetAttribute<DynamicEntriesAttribute>(node);
				if (attribute != null)
				{
					num3 += (float)(attribute.StartIndex * fontHeight);
				}
				empty.Height = Math.Max(empty.Height, num3);
			}
			return empty;
		}

		protected override void PaintWorkspace(Graphics g, Rectangle rect)
		{
			Vec2 center = base.Center;
			CalcNodesRects(g, center);
			PaintRubberBox(g);
			if (Graph != null)
			{
				BeginTranslate(g);
				PaintShadows(g);
				PaintSelection(g);
				PaintSockets(g);
				PaintNodes(g);
				EndTranslate(g);
			}
		}

		private void PaintRubberBox(Graphics g)
		{
			if (dragMode != DragMode.RubberBox || dragRect.IsEmpty)
			{
				return;
			}
			using (GraphicsPath path = DrawingHelper.CreateRoundRect(dragRect.ToRect(), 4))
			{
				using (Pen pen = new Pen(SelectedColor))
				{
					pen.DashStyle = DashStyle.Dash;
					g.DrawPath(pen, path);
				}
				using (Brush brush = new SolidBrush(Color.FromArgb(32, SelectedColor)))
				{
					g.FillPath(brush, path);
				}
			}
		}

		private void PaintShadows(Graphics g)
		{
			using (Brush brush = new SolidBrush(Color.FromArgb(16, 0, 0, 0)))
			{
				foreach (IGraphNode node in Graph.Nodes)
				{
					NodeTag nodeTag = (NodeTag)node.Tag;
					RectangleF bounds = nodeTag.Bounds;
					bounds.Offset(5f, 5f);
					using (GraphicsPath path = DrawingHelper.CreateRoundRect(bounds.ToRect(), 4))
					{
						g.FillPath(brush, path);
					}
				}
			}
		}

		private void PaintNodeBackground(Graphics g, IGraphNode node)
		{
			NodeTag nodeTag = (NodeTag)node.Tag;
			RectangleF bounds = nodeTag.Bounds;
			Color color = (AllowEdit ? ReflectionHelper.GetColor(node) : ReflectionHelper.GetColor(node).Desaturate(0.125f));
			using (Brush brush = new SolidBrush(color))
			{
				using (GraphicsPath path = DrawingHelper.CreateRoundRect(bounds.ToRect(), 4))
				{
					g.FillPath(Brushes.White, path);
				}
				if (ShowCaption(node))
				{
					bounds.Height = 17f;
					using (GraphicsPath path2 = DrawingHelper.CreateRoundRect(bounds.ToRect(), 4, RectangleCorners.TopLeft | RectangleCorners.TopRight))
					{
						g.FillPath(brush, path2);
						return;
					}
				}
				bounds.Width = 16f;
				using (GraphicsPath path3 = DrawingHelper.CreateRoundRect(bounds.ToRect(), 4, RectangleCorners.TopLeft | RectangleCorners.BottomLeft))
				{
					g.FillPath(brush, path3);
				}
			}
		}

		private void PaintNodes(Graphics g)
		{
			foreach (IGraphNode node in Graph.Nodes)
			{
				NodeTag nodeTag = (NodeTag)node.Tag;
				PaintNodeBackground(g, node);
				bool flag = ShowCaption(node);
				PointF empty = PointF.Empty;
				empty.X = nodeTag.Bounds.X + (float)((!flag) ? 16 : 0) + 5f;
				empty.Y = nodeTag.Bounds.Y + (flag ? 17f : nodeTag.Bounds.Height) * 0.5f;
				using (StringFormat stringFormat = new StringFormat())
				{
					stringFormat.LineAlignment = StringAlignment.Center;
					stringFormat.Alignment = StringAlignment.Near;
					string s = node.ToString();
					if (IsConstant(node) && node is IDisplayValue)
					{
						s = ((IDisplayValue)node).DisplayValue;
					}
					g.DrawString(s, base.ScaledFont, flag ? Brushes.White : Brushes.Black, empty, stringFormat);
					PaintNodeLabels(g, node, stringFormat);
				}
			}
		}

		private void PaintNodeLabels(Graphics g, IGraphNode node, StringFormat sf)
		{
			IFlowGraphNode flowGraphNode = (IFlowGraphNode)node;
			NodeTag nodeTag = (NodeTag)node.Tag;
			g.SetClip(nodeTag.Bounds);
			IFlowGraphBlockCurve flowGraphBlockCurve = node as IFlowGraphBlockCurve;
			if (flowGraphBlockCurve != null)
			{
				Image bc_fill = GameTuner.Framework.Properties.Resources.bc_fill;
				RectangleF blockCurveRect = GetBlockCurveRect(node);
				RectangleF empty = RectangleF.Empty;
				float num = 0f;
				float num2 = blockCurveRect.X;
				for (int i = 0; i < flowGraphBlockCurve.NumBlocks; i++)
				{
					BlockCurveInfo block = flowGraphBlockCurve.GetBlock(i);
					num += block.Dist;
					float num3 = block.Dist * 23f / 0.33f;
					empty.X = num2;
					empty.Y = blockCurveRect.Y;
					empty.Width = num3;
					empty.Height = 23f;
					InterpolationMode interpolationMode = g.InterpolationMode;
					g.InterpolationMode = InterpolationMode.HighQualityBilinear;
					g.DrawImage(bc_fill, empty);
					PaintBlock(g, block, empty.X, empty.Y, empty.Width, empty.Height);
					g.InterpolationMode = interpolationMode;
					num2 += num3;
				}
			}
			IFlowGraphDynamicEntries flowGraphDynamicEntries = node as IFlowGraphDynamicEntries;
			if (flowGraphDynamicEntries != null)
			{
				int entryCount = flowGraphDynamicEntries.EntryCount;
				if (node == hotNode && AllowEdit)
				{
					RectangleF rectangleF = nodeTag.Entries[flowGraphDynamicEntries.EntryCount - 1];
					rectangleF.Y += base.FontHeight;
					DrawingHelper.DrawImage(g, GameTuner.Framework.Properties.Resources.graph_append, rectangleF.X, rectangleF.Y + rectangleF.Height * 0.5f - (float)GameTuner.Framework.Properties.Resources.graph_append.Height * 0.5f);
					for (int j = 0; j < entryCount; j++)
					{
						if (nodeTag.Entries[j].Contains(hotNodePoint.X, hotNodePoint.Y))
						{
							rectangleF = nodeTag.Entries[j];
							Rectangle rect = rectangleF.ToRect();
							rect.Inflate(-1, 0);
							using (Brush brush = new SolidBrush(Color.FromArgb(160, SelectedColor)))
							{
								g.FillRectangle(brush, rect);
							}
							DrawingHelper.DrawImage(g, GameTuner.Framework.Properties.Resources.graph_remove, rectangleF.Right - (float)GameTuner.Framework.Properties.Resources.graph_remove.Width, rectangleF.Y + rectangleF.Height * 0.5f - (float)GameTuner.Framework.Properties.Resources.graph_remove.Height * 0.5f);
							break;
						}
					}
				}
				DynamicEntriesAttribute attribute = ReflectionHelper.GetAttribute<DynamicEntriesAttribute>(node);
				if (attribute == null || (attribute.Style & DynamicEntriesStyle.AsInputs) == 0)
				{
					for (int k = 0; k < entryCount; k++)
					{
						g.DrawString(flowGraphDynamicEntries.GetEntryLabel(k), base.ScaledFont, Brushes.Black, nodeTag.Entries[k], sf);
					}
				}
			}
			foreach (IGraphSocket inputSocket in flowGraphNode.InputSockets)
			{
				IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)inputSocket;
				SocketTag socketTag = (SocketTag)inputSocket.Tag;
				g.DrawString(flowGraphSocket.Name, base.ScaledFont, Brushes.Black, socketTag.LabelPoint, sf);
				Color color = (AllowEdit ? ReflectionHelper.GetColor(flowGraphSocket.SocketType) : ReflectionHelper.GetColor(flowGraphSocket.SocketType).Desaturate(0.125f));
				using (Brush brush2 = new SolidBrush(color))
				{
					g.FillRectangle(brush2, socketTag.Bounds);
					g.DrawRectangle(Pens.LightGray, socketTag.Bounds.X, socketTag.Bounds.Y, socketTag.Bounds.Width, socketTag.Bounds.Height);
				}
			}
			IFlowGraphCustomValue flowGraphCustomValue = node as IFlowGraphCustomValue;
			if (flowGraphCustomValue != null)
			{
				flowGraphCustomValue.PaintCustomValue(g, nodeTag.ValueRect, base.ScaledFont, sf);
			}
			else if (!IsConstant(node))
			{
				IDisplayValue displayValue = node as IDisplayValue;
				if (displayValue != null)
				{
					g.DrawString(displayValue.DisplayValue, base.ScaledFont, Brushes.Black, nodeTag.ValueRect, sf);
				}
			}
			g.ResetClip();
			foreach (IGraphSocket outputSocket in flowGraphNode.OutputSockets)
			{
				IFlowGraphSocket flowGraphSocket2 = (IFlowGraphSocket)outputSocket;
				SocketTag socketTag2 = (SocketTag)outputSocket.Tag;
				Color color2 = (AllowEdit ? ReflectionHelper.GetColor(flowGraphSocket2.SocketType) : ReflectionHelper.GetColor(flowGraphSocket2.SocketType).Desaturate(0.125f));
				DrawingHelper.DrawImage(g, GameTuner.Framework.Properties.Resources.graph_link, socketTag2.Bounds.X, socketTag2.Bounds.Y, color2);
			}
		}

		public static void PaintBlock(Graphics g, BlockCurveInfo block, float x, float y, float w, float h)
		{
			if (block.Type != BlockCurveType.None && !(w <= float.Epsilon))
			{
				Image[] array = new Image[4]
				{
					GameTuner.Framework.Properties.Resources.bc_line,
					GameTuner.Framework.Properties.Resources.bc_slash,
					GameTuner.Framework.Properties.Resources.bc_bend,
					GameTuner.Framework.Properties.Resources.bc_ess
				};
				Image image = array[(int)block.Type];
				RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;
				w = ((w == -1f) ? ((float)image.Width) : w);
				h = ((h == -1f) ? ((float)image.Height) : h);
				if (block.FlipX != 0 && block.FlipY != 0)
				{
					rotateFlipType = RotateFlipType.Rotate180FlipNone;
				}
				else if (block.FlipX != 0)
				{
					rotateFlipType = RotateFlipType.RotateNoneFlipX;
				}
				else if (block.FlipY != 0)
				{
					rotateFlipType = RotateFlipType.Rotate180FlipX;
				}
				image.RotateFlip(rotateFlipType);
				Rectangle destRect = new Rectangle((int)x, (int)y, (int)w, (int)h);
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
				if (rotateFlipType != RotateFlipType.RotateNoneFlipNone)
				{
					image.RotateFlip(rotateFlipType);
				}
			}
		}

		private void PaintSockets(Graphics g)
		{
			foreach (IGraphNode node in Graph.Nodes)
			{
				IFlowGraphNode flowGraphNode = (IFlowGraphNode)node;
				foreach (IGraphSocket inputSocket in flowGraphNode.InputSockets)
				{
					IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)inputSocket;
					if (flowGraphSocket.OutputSocket != null)
					{
						PaintLine(g, flowGraphSocket.OutputSocket, inputSocket);
					}
				}
			}
			if (dragMode != DragMode.NewLink && dragMode != DragMode.MoveLink)
			{
				return;
			}
			bool flag = false;
			Vec2 point = new Vec2(dragCurrent.X, dragCurrent.Y);
			ClientToWorld(ref point);
			FlowGraphSocketType type;
			IGraphSocket graphSocket = FindSocket(point.X, point.Y, out type);
			IFlowGraphSocket flowGraphSocket2 = (IFlowGraphSocket)graphSocket;
			if (flowGraphSocket2 != null && type == FlowGraphSocketType.Input && flowGraphSocket2.SocketType == ((IFlowGraphSocket)focusSocket).SocketType && !IsParentNode(focusSocket.Owner, graphSocket.Owner))
			{
				PaintLine(g, focusSocket, graphSocket);
				flag = true;
			}
			Color color = ReflectionHelper.GetColor(((IFlowGraphSocket)focusSocket).SocketType);
			if (!flag)
			{
				using (Pen pen = new Pen(color, 1f))
				{
					PointF attachPoint = ((SocketTag)focusSocket.Tag).AttachPoint;
					PointF toPointF = point.ToPointF;
					pen.DashStyle = DashStyle.Dash;
					DrawingHelper.DrawBezier(g, pen, attachPoint, toPointF);
				}
			}
			IFlowGraphSocket flowGraphSocket3 = (IFlowGraphSocket)focusSocket;
			using (Brush brush = new SolidBrush(color))
			{
				using (StringFormat stringFormat = new StringFormat())
				{
					PointF attachPoint2 = ((SocketTag)focusSocket.Tag).AttachPoint;
					stringFormat.LineAlignment = StringAlignment.Center;
					SizeF sizeF = g.MeasureString(flowGraphSocket3.Name, base.ScaledFont, attachPoint2, stringFormat);
					using (Brush brush2 = new SolidBrush(Color.FromArgb(32, 0, 0, 0)))
					{
						g.FillRectangle(brush2, attachPoint2.X, attachPoint2.Y - sizeF.Height * 0.5f, sizeF.Width, sizeF.Height);
					}
					g.DrawString(flowGraphSocket3.Name, base.ScaledFont, brush, attachPoint2, stringFormat);
				}
			}
		}

		private void PaintSelection(Graphics g)
		{
			foreach (IGraphNode node in Graph.Nodes)
			{
				if (IsRubberSelected(node))
				{
					PaintSelection(g, node);
				}
			}
		}

		private void PaintSelection(Graphics g, object o)
		{
			IGraphNode graphNode = o as IGraphNode;
			if (graphNode == null)
			{
				return;
			}
			using (Pen pen = new Pen(SelectedColor, 3f))
			{
				NodeTag nodeTag = (NodeTag)graphNode.Tag;
				RectangleF bounds = nodeTag.Bounds;
				bounds.Inflate(5f, 5f);
				using (GraphicsPath path = DrawingHelper.CreateRoundRect(bounds.ToRect(), 4))
				{
					g.DrawPath(pen, path);
				}
			}
		}

		private bool IsRubberSelected(object item)
		{
			bool flag = false;
			if (dragMode == DragMode.RubberBox)
			{
				bool flag2 = IsSelected(item);
				bool flag3 = IsSelected(item, RubberItems);
				if (dragCtrl)
				{
					return (flag2 && !flag3) || (flag3 && !flag2);
				}
				return flag2 || flag3;
			}
			return IsSelected(item);
		}

		public bool IsSelected(object item)
		{
			return IsSelected(item, SelectedItems);
		}

		private bool IsSelected(object item, ListEvent<object> selectList)
		{
			return selectList.Contains(item);
		}

		private void PaintLine(Graphics g, IGraphSocket output, IGraphSocket input)
		{
			IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)output;
			Color color = ReflectionHelper.GetColor(flowGraphSocket.SocketType);
			PointF attachPoint = ((SocketTag)output.Tag).AttachPoint;
			PointF attachPoint2 = ((SocketTag)input.Tag).AttachPoint;
			PointF location = ((SocketTag)input.Tag).LinkBounds.Location;
			if (!AllowEdit)
			{
				color = color.Desaturate(0.125f);
			}
			using (Pen pen = new Pen(color, 3f))
			{
				DrawingHelper.DrawBezier(g, pen, attachPoint, attachPoint2);
				DrawingHelper.DrawImage(g, GameTuner.Framework.Properties.Resources.graph_link, location.X, location.Y, color);
			}
		}

		public void CenterView()
		{
			base.Origin = new Vec2(NodeBounds.X + NodeBounds.Width * 0.5f, NodeBounds.Y + NodeBounds.Height * 0.5f);
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
				if (nodeTag != null && nodeTag.Bounds.Contains(pt.ToPointF))
				{
					return graphNode;
				}
			}
			return null;
		}

		private IGraphSocket FindSocket(float x, float y)
		{
			FlowGraphSocketType type;
			return FindSocket(x, y, out type);
		}

		private IGraphSocket FindSocket(float x, float y, out FlowGraphSocketType type)
		{
			foreach (IGraphNode node in Graph.Nodes)
			{
				IFlowGraphNode flowGraphNode = (IFlowGraphNode)node;
				foreach (IGraphSocket inputSocket in flowGraphNode.InputSockets)
				{
					if (inputSocket.Tag != null)
					{
						RectangleF bounds = ((SocketTag)inputSocket.Tag).Bounds;
						bounds.Inflate(10f, 0f);
						IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)inputSocket;
						if (flowGraphSocket.OutputSocket != null)
						{
							int num = GameTuner.Framework.Properties.Resources.graph_link.Width;
							bounds.X -= num;
							bounds.Width += num;
						}
						if (bounds.Contains(x, y))
						{
							type = FlowGraphSocketType.Input;
							return inputSocket;
						}
					}
				}
				foreach (IGraphSocket outputSocket in flowGraphNode.OutputSockets)
				{
					if (outputSocket.Tag != null && ((SocketTag)outputSocket.Tag).Bounds.Contains(x, y))
					{
						type = FlowGraphSocketType.Output;
						return outputSocket;
					}
				}
			}
			type = FlowGraphSocketType.None;
			return null;
		}

		protected override bool HandleMouseDown(object sender, MouseEventArgs e)
		{
			if (base.HandleMouseDown(sender, e))
			{
				return true;
			}
			bool result = false;
			if (dragMode == DragMode.Idle)
			{
				switch (e.Button)
				{
				case MouseButtons.Left:
					result = OnLeftButtonDown(e);
					break;
				case MouseButtons.Right:
					result = OnRightButtonDown(e);
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
			switch (dragMode)
			{
			case DragMode.Node:
				OnDragNode(e);
				result = true;
				break;
			case DragMode.RubberBox:
				OnDragRubberBox(e);
				result = true;
				break;
			case DragMode.NewLink:
			case DragMode.MoveLink:
				OnDragNewLink(e);
				result = true;
				break;
			}
			return result;
		}

		protected override bool HandleSetCursor(object sender, MouseEventArgs e)
		{
			if (base.HandleSetCursor(sender, e))
			{
				return true;
			}
			if (Graph == null)
			{
				return false;
			}
			Vec2 point = new Vec2(e.X, e.Y);
			ClientToWorld(ref point);
			hotNodePoint = point;
			IGraphNode graphNode = (IGraphNode)Find(e.X, e.Y);
			if (graphNode != hotNode || graphNode != null)
			{
				hotNode = graphNode;
				Invalidate();
			}
			if (HitBlockCurve(graphNode, e.X, e.Y) || HitAppend(graphNode, e.X, e.Y) || HitRemove(graphNode, e.X, e.Y) != -1)
			{
				Cursor = CustomCursors.Get(CustomCursor.FingerPoint);
				return true;
			}
			FlowGraphSocketType type;
			IGraphSocket graphSocket = FindSocket(point.X, point.Y, out type);
			IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)graphSocket;
			if (AllowEdit && graphSocket != null && (type == FlowGraphSocketType.Output || flowGraphSocket.OutputSocket != null))
			{
				Cursor = CustomCursors.Get(CustomCursor.FingerPoint);
				return true;
			}
			if (dragMode == DragMode.Idle && graphNode != null && AllowEdit && HitRemove(graphNode, e.X, e.Y, false) == -1)
			{
				Cursor = CustomCursors.Get(CustomCursor.HandDrag);
				return true;
			}
			return false;
		}

		private bool Intersect(IGraphNode node, RectangleF rect)
		{
			NodeTag nodeTag = node.Tag as NodeTag;
			return nodeTag.Bounds.IntersectsWith(rect);
		}

		public void SelectItem(object item, bool select)
		{
			SelectItem(item, select, SelectedItems);
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

		private void bcPopup_BeginModifiedAction(object sender, EventArgs e)
		{
			EventHandler modifiedAction = this.ModifiedAction;
			if (modifiedAction != null)
			{
				modifiedAction(this, EventArgs.Empty);
			}
		}

		private void bcPopup_EndModifiedAction(object sender, EventArgs e)
		{
		}

		private void OnDragNode(MouseEventArgs e)
		{
			if (!dragMoved)
			{
				dragMoved = true;
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
			OffsetSelectedNodes(e.X - dragPos.X, e.Y - dragPos.Y);
			dragPos.X = e.X;
			dragPos.Y = e.Y;
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
			}
		}

		private void OnDragRubberBox(MouseEventArgs e)
		{
			dragRect = dragRect.SetEdges(e.X, e.Y, dragPos.X, dragPos.Y);
			RectangleF rect = dragRect;
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

		protected override bool HandleMouseUp(object sender, MouseEventArgs e)
		{
			if (base.HandleMouseUp(sender, e))
			{
				return true;
			}
			bool result = false;
			if (e.Button == MouseButtons.Right)
			{
				FlowGraphHandler graphContextMenu = this.GraphContextMenu;
				if (graphContextMenu != null)
				{
					FlowGraphEventArgs e2 = new FlowGraphEventArgs(focusItem, e);
					graphContextMenu(this, e2);
				}
			}
			if (e.Button == MouseButtons.Left)
			{
				switch (dragMode)
				{
				case DragMode.RubberBox:
					OnRubberBoxEnd(e);
					result = true;
					break;
				case DragMode.Node:
					OnDragNodeEnd(e);
					result = true;
					break;
				case DragMode.MoveLink:
					OnDragMoveLinkEnd(e);
					result = true;
					break;
				case DragMode.NewLink:
					OnDragNewLinkEnd(e);
					result = true;
					break;
				}
			}
			return result;
		}

		private void OnDragNewLink(MouseEventArgs e)
		{
			dragCurrent.X = e.X;
			dragCurrent.Y = e.Y;
			dragMoved = true;
			Invalidate();
		}

		private void OnDragMoveLinkEnd(MouseEventArgs e)
		{
			dragCurrent.X = e.X;
			dragCurrent.Y = e.Y;
			Vec2 point = new Vec2(dragCurrent.X, dragCurrent.Y);
			ClientToWorld(ref point);
			FlowGraphSocketType type;
			IGraphSocket graphSocket = FindSocket(point.X, point.Y, out type);
			IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)graphSocket;
			if (flowGraphSocket != null && type == FlowGraphSocketType.Input && flowGraphSocket.SocketType == ((IFlowGraphSocket)focusSocket).SocketType)
			{
				if (!IsParentNode(focusSocket.Owner, graphSocket.Owner))
				{
					FlowGraphHandler movedSocketConnection = this.MovedSocketConnection;
					if (movedSocketConnection != null)
					{
						FlowGraphEventArgs e2 = new FlowGraphEventArgs(graphSocket, e);
						e2.Target = focusSocket;
						e2.Original = movedSocket;
						movedSocketConnection(this, e2);
					}
					flowGraphSocket.Connect(focusSocket.Owner, focusSocket);
				}
			}
			else
			{
				FlowGraphHandler removeSocketConnection = this.RemoveSocketConnection;
				if (removeSocketConnection != null)
				{
					removeSocketConnection(this, new FlowGraphEventArgs(dragSave.Socket, e));
				}
			}
			dragMode = DragMode.Idle;
			base.Capture = false;
			Invalidate();
		}

		private void OnDragNewLinkEnd(MouseEventArgs e)
		{
			dragCurrent.X = e.X;
			dragCurrent.Y = e.Y;
			Vec2 point = new Vec2(dragCurrent.X, dragCurrent.Y);
			ClientToWorld(ref point);
			FlowGraphSocketType type;
			IGraphSocket graphSocket = FindSocket(point.X, point.Y, out type);
			IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)graphSocket;
			if (flowGraphSocket != null && type == FlowGraphSocketType.Input && flowGraphSocket.SocketType == ((IFlowGraphSocket)focusSocket).SocketType && !IsParentNode(focusSocket.Owner, graphSocket.Owner))
			{
				EventHandler modifiedAction = this.ModifiedAction;
				if (modifiedAction != null)
				{
					modifiedAction(this, EventArgs.Empty);
				}
				FlowGraphHandler newSocketConnection = this.NewSocketConnection;
				if (newSocketConnection != null)
				{
					FlowGraphEventArgs e2 = new FlowGraphEventArgs(graphSocket, e);
					e2.Target = focusSocket;
					newSocketConnection(this, e2);
				}
				flowGraphSocket.Connect(focusSocket.Owner, focusSocket);
			}
			dragMode = DragMode.Idle;
			base.Capture = false;
			Invalidate();
		}

		private bool IsParentNode(IGraphNode node, IGraphNode parent)
		{
			if (node == parent)
			{
				return true;
			}
			IFlowGraphNode flowGraphNode = (IFlowGraphNode)node;
			foreach (IGraphSocket inputSocket in flowGraphNode.InputSockets)
			{
				IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)inputSocket;
				if (flowGraphSocket.OutputSocket != null && IsParentNode(flowGraphSocket.Node, parent))
				{
					return true;
				}
			}
			return false;
		}

		private bool OnRightButtonDown(MouseEventArgs e)
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
			return true;
		}

		private bool OnLeftButtonDown(MouseEventArgs e)
		{
			dragCtrl = KeyHelper.CtrlPressed;
			Vec2 point = new Vec2(e.X, e.Y);
			ClientToWorld(ref point);
			focusItem = Find(e.X, e.Y);
			FlowGraphSocketType type;
			focusSocket = FindSocket(point.X, point.Y, out type);
			IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)focusSocket;
			BeginUpdate();
			if (focusItem != null || focusSocket != null)
			{
				if (focusItem != null)
				{
					if (dragCtrl)
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
				}
				if (AllowEdit)
				{
					if (HitAppend(focusItem as IGraphNode, e.X, e.Y))
					{
						EventHandler modifiedAction = this.ModifiedAction;
						if (modifiedAction != null)
						{
							modifiedAction(this, EventArgs.Empty);
						}
						IFlowGraphDynamicEntries flowGraphDynamicEntries = (IFlowGraphDynamicEntries)focusItem;
						flowGraphDynamicEntries.InsertEntry(flowGraphDynamicEntries.EntryCount);
						EndUpdate();
						return true;
					}
					if (HitRemove(focusItem as IGraphNode, e.X, e.Y, false) != -1)
					{
						int num = HitRemove(focusItem as IGraphNode, e.X, e.Y);
						if (num != -1)
						{
							IFlowGraphDynamicEntries flowGraphDynamicEntries2 = (IFlowGraphDynamicEntries)focusItem;
							if (flowGraphDynamicEntries2.EntryCount > 1)
							{
								EventHandler modifiedAction2 = this.ModifiedAction;
								if (modifiedAction2 != null)
								{
									modifiedAction2(this, EventArgs.Empty);
								}
								flowGraphDynamicEntries2.RemoveEntry(num);
							}
						}
						EndUpdate();
						return true;
					}
					if (HitBlockCurve(focusItem as IGraphNode, e.X, e.Y))
					{
						EndUpdate();
						bcPopup.Location = Cursor.Position;
						bcPopup.BlockCurve = focusItem as IFlowGraphBlockCurve;
						bcPopup.ShowIt();
						return true;
					}
					if (flowGraphSocket != null)
					{
						switch (type)
						{
						case FlowGraphSocketType.Output:
						{
							SocketTag socketTag2 = (SocketTag)focusSocket.Tag;
							base.Capture = true;
							dragItem = focusSocket;
							dragAttach = socketTag2.AttachPoint;
							dragPos.X = e.X;
							dragPos.Y = e.Y;
							dragCurrent = dragPos;
							dragMode = DragMode.NewLink;
							dragMoved = false;
							break;
						}
						case FlowGraphSocketType.Input:
							if (flowGraphSocket.Node != null)
							{
								dragSave.Socket = focusSocket;
								dragSave.Node = focusSocket.Owner;
								base.Capture = true;
								SocketTag socketTag = (SocketTag)flowGraphSocket.OutputSocket.Tag;
								dragItem = flowGraphSocket.OutputSocket;
								dragAttach = socketTag.AttachPoint;
								dragPos.X = e.X;
								dragPos.Y = e.Y;
								dragCurrent = dragPos;
								dragMode = DragMode.MoveLink;
								dragMoved = false;
								movedSocket = focusSocket;
								focusItem = flowGraphSocket.Node;
								focusSocket = flowGraphSocket.OutputSocket;
								EventHandler modifiedAction3 = this.ModifiedAction;
								if (modifiedAction3 != null)
								{
									modifiedAction3(this, EventArgs.Empty);
								}
								flowGraphSocket.Disconnect();
							}
							break;
						}
					}
					else
					{
						base.Capture = true;
						dragItem = focusItem;
						dragPos.X = e.X;
						dragPos.Y = e.Y;
						dragCurrent = dragPos;
						dragMode = DragMode.Node;
						dragMoved = false;
					}
				}
			}
			else
			{
				if (!dragCtrl)
				{
					SelectedItems.Clear();
				}
				base.Capture = true;
				dragPos.X = e.X;
				dragPos.Y = e.Y;
				dragMode = DragMode.RubberBox;
				dragRect = RectangleF.Empty;
			}
			EndUpdate();
			return base.Capture;
		}

		private void OnDragNodeEnd(MouseEventArgs e)
		{
			EventHandler moveNodeEnd = this.MoveNodeEnd;
			if (moveNodeEnd != null)
			{
				moveNodeEnd(this, EventArgs.Empty);
			}
			dragMode = DragMode.Idle;
			base.Capture = false;
			Invalidate();
		}

		private void OnRubberBoxEnd(MouseEventArgs e)
		{
			base.Capture = false;
			BeginUpdate();
			if (!dragCtrl)
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
			dragMode = DragMode.Idle;
			EndUpdate();
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
			base.Name = "FlowGraphControl";
			base.Size = new System.Drawing.Size(564, 357);
			base.ResumeLayout(false);
		}
	}
}
