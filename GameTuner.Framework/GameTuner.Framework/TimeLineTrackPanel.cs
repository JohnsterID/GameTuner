using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.Scrollables;

namespace GameTuner.Framework
{
	public class TimeLineTrackPanel : UserControl
	{
		private enum DragMode
		{
			Idle,
			Pan,
			Shuttle,
			Key
		}

		public delegate void ContextMenuKeyHandler(object sender, ScrollableTree.TreeNode node, EventArgs e);

		private DragMode drag;

		private Point drag_pt;

		private bool drag_moved;

		private ScrollableItemPaintEventArgs paint_args;

		private TimeLineHitArgs timeline_args;

		private ScrollableTree.TreeNode drag_node;

		private IContainer components;

		public Color BackColor0 { get; set; }

		public Color BackColor1 { get; set; }

		public bool LockKeys { get; set; }

		[Browsable(false)]
		public ListEvent<IKey> SelectedKeys { get; private set; }

		private TimeLineControl TimeLineControl { get; set; }

		private TimeRulerControl Ruler
		{
			get
			{
				return TimeLineControl.TimeRuler;
			}
		}

		private ScrollableTree Tree
		{
			get
			{
				return TimeLineControl.ScrollableTree;
			}
		}

		public event EventHandler MovingKeyBegin;

		public event EventHandler MovingKey;

		public event EventHandler MovingKeyEnd;

		public event EventHandler ModifiedAction;

		public event ContextMenuKeyHandler ContextMenuKey;

		public event EventHandler<ScrollableItemPaintEventArgs> PostPaintTracks;

		public TimeLineTrackPanel(TimeLineControl timeLineControl, Panel panel)
		{
			TimeLineControl = timeLineControl;
			TimeLineControl.TimeRuler.ScaleChanged += TimeRuler_ScaleChanged;
			InitializeComponent();
			SelectedKeys = new ListEvent<IKey>();
			paint_args = new ScrollableItemPaintEventArgs();
			timeline_args = new TimeLineHitArgs(timeLineControl, Rectangle.Empty);
			drag = DragMode.Idle;
			drag_moved = false;
			Dock = DockStyle.Fill;
			BackColor = panel.BackColor;
			BackColor0 = Color.FromArgb(170, 170, 170);
			BackColor1 = Color.FromArgb(181, 181, 181);
			Ruler.CurrentTimeChanged += RefreshDisplay;
			Ruler.OriginChanged += RefreshDisplay;
			Ruler.RangeChanged += RefreshDisplay;
			Tree.DisplayListChanged += RefreshDisplay;
			Tree.VerticalScroll.ValueChanged += RefreshDisplay;
			base.MouseWheel += TimeLineTrackPanel_MouseWheel;
		}

		private void TimeLineTrackPanel_MouseWheel(object sender, MouseEventArgs e)
		{
			if (KeyHelper.CtrlPressed)
			{
				int num = (TimeLineControl.TrackBar.Maximum - TimeLineControl.TrackBar.Minimum) / 20;
				TimeLineControl.SetMajorScale(Ruler.MajorScale + (float)((e.Delta < 0) ? num : (-num)));
			}
			else
			{
				Tree.PerformVScroll((e.Delta < 0) ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement, 0);
			}
		}

		private void TimeRuler_ScaleChanged(object sender, EventArgs e)
		{
			Invalidate();
			Update();
		}

		private void RefreshDisplay(object sender, EventArgs e)
		{
			Invalidate();
			Update();
		}

		private void TimeLineTrackPanel_Paint(object sender, PaintEventArgs e)
		{
			PaintTracks(e.Graphics);
		}

		protected virtual void PaintTracks(Graphics g)
		{
			int value = Tree.VerticalScroll.Value;
			int num = value + base.ClientSize.Height;
			Rectangle rectangle = new Rectangle(0, 0, base.ClientSize.Width, 0);
			Brush[] array = new Brush[2]
			{
				new SolidBrush(BackColor0),
				new SolidBrush(BackColor1)
			};
			int num2 = 0;
			int num3 = Ruler.TimeToX(0f);
			g.DrawLine(Pens.Gray, num3, 0, num3, base.ClientSize.Height);
			foreach (ScrollableTree.DisplayTreeNode displayNode in Tree.DisplayNodes)
			{
				if (displayNode.Origin + displayNode.Node.Item.ItemHeight >= value || displayNode.Origin < num)
				{
					rectangle.Y = displayNode.Origin - value;
					rectangle.Height = displayNode.Node.Item.ItemHeight;
					g.FillRectangle(array[num2], rectangle);
					g.DrawLine(Pens.Gray, num3, rectangle.Y, num3, rectangle.Bottom);
					num2 = 1 - num2;
					PaintTrack(g, displayNode, rectangle);
				}
			}
			using (Pen pen = new Pen(Color.FromArgb(64, Color.Gray)))
			{
				float num4 = Ruler.Origin + Ruler.TimeSpan;
				for (float num5 = (float)Math.Truncate(Ruler.Origin); num5 < num4; num5 += 1f)
				{
					num3 = Ruler.TimeToX(num5);
					g.DrawLine(pen, num3, 0, num3, base.ClientSize.Height);
				}
			}
			EventHandler<ScrollableItemPaintEventArgs> postPaintTracks = this.PostPaintTracks;
			if (postPaintTracks != null)
			{
				paint_args.Graphics = g;
				paint_args.Bounds = base.ClientRectangle;
				paint_args.State = ScrollableItemState.Normal;
				paint_args.Level = 0;
				paint_args.Style = ScrollableItemStyle.Normal;
				paint_args.Interacting = drag == DragMode.Key && drag_moved;
				postPaintTracks(this, paint_args);
			}
			if (Ruler.TrackRangeVisible)
			{
				using (Pen pen2 = new Pen(Ruler.RangeColor))
				{
					num3 = Ruler.TimeToX(Ruler.RangeStart);
					g.DrawLine(pen2, num3, 0, num3, base.ClientSize.Height);
					num3 = Ruler.TimeToX(Ruler.RangeStart + Ruler.RangeDuration);
					g.DrawLine(pen2, num3, 0, num3, base.ClientSize.Height);
				}
			}
			if (Ruler.ShuttleVisible)
			{
				num3 = Ruler.TimeToX(Ruler.CurrentTime);
				using (Pen pen3 = new Pen(Ruler.ShuttleColor))
				{
					g.DrawLine(pen3, num3, 0, num3, base.ClientSize.Height);
				}
			}
		}

		protected virtual void PaintTrack(Graphics g, ScrollableTree.DisplayTreeNode node, Rectangle r)
		{
			IScrollableItemTrack scrollableItemTrack = node.Node.Item as IScrollableItemTrack;
			if (scrollableItemTrack != null)
			{
				ScrollableItemState scrollableItemState = (Tree.SelectedNodes.Contains(node.Node) ? ScrollableItemState.Selected : ScrollableItemState.Normal);
				paint_args.Graphics = g;
				paint_args.Bounds = r;
				paint_args.State = scrollableItemState;
				paint_args.Level = node.Level;
				paint_args.Style = ScrollableItemStyle.Normal;
				paint_args.Interacting = drag == DragMode.Key && drag_moved;
				scrollableItemTrack.PaintTrack(TimeLineControl, paint_args);
			}
			else
			{
				g.DrawLine(Pens.LightGray, r.X, r.Bottom, r.Right, r.Bottom);
			}
		}

		private void TimeLineTrackPanel_MouseDown(object sender, MouseEventArgs e)
		{
			bool ctrlPressed = KeyHelper.CtrlPressed;
			if (e.Button == MouseButtons.Middle)
			{
				drag = DragMode.Pan;
				drag_pt.X = e.X;
				base.Capture = true;
				Cursor = CustomCursors.Get(CustomCursor.HandDrag);
			}
			else if (e.Button == MouseButtons.Left)
			{
				IKey key = FindKey(e.X, e.Y, ref drag_node);
				if (!ctrlPressed || key == null)
				{
					SelectedKeys.Clear();
				}
				if (key != null)
				{
					bool flag = SelectedKeys.Contains(key);
					if (ctrlPressed)
					{
						if (flag)
						{
							SelectedKeys.Remove(key);
						}
						else
						{
							SelectedKeys.Add(key);
						}
					}
					else if (!flag)
					{
						SelectedKeys.Add(key);
					}
				}
				Tree.BeginUpdate();
				Tree.SelectedNodes.Clear();
				Tree.SelectedNodes.Add(drag_node);
				Tree.EndUpdate();
				if (TimeLineControl.AllowEdit && !LockKeys)
				{
					EventHandler movingKeyBegin = this.MovingKeyBegin;
					if (movingKeyBegin != null)
					{
						movingKeyBegin(this, EventArgs.Empty);
					}
					drag = DragMode.Key;
					drag_moved = false;
					drag_pt.X = e.X;
					drag_pt.Y = e.Y;
					base.Capture = true;
				}
				Invalidate();
			}
			else if (e.Button == MouseButtons.Right)
			{
				IKey key2 = FindKey(e.X, e.Y, ref drag_node);
				if (key2 == null || (!SelectedKeys.Contains(key2) && !ctrlPressed))
				{
					SelectedKeys.Clear();
				}
				if (key2 != null && !SelectedKeys.Contains(key2))
				{
					SelectedKeys.Add(key2);
				}
				Tree.BeginUpdate();
				Tree.SelectedNodes.Clear();
				Tree.SelectedNodes.Add(drag_node);
				Tree.EndUpdate();
				ContextMenuKeyHandler contextMenuKey = this.ContextMenuKey;
				if (contextMenuKey != null)
				{
					contextMenuKey(this, drag_node, EventArgs.Empty);
				}
				Invalidate();
			}
		}

		private IKey FindKey(int x, int y, ref ScrollableTree.TreeNode drag_node)
		{
			ScrollableTree.TreeNode treeNode = Tree.FindNode(0, y);
			if (treeNode != null)
			{
				drag_node = treeNode;
				if (treeNode.Item is IScrollableItemKeys)
				{
					timeline_args.Bounds = Tree.GetBounds(treeNode);
					return ((IScrollableItemKeys)treeNode.Item).FindKey(x, y, timeline_args);
				}
				return null;
			}
			drag_node = null;
			return null;
		}

		private void TimeLineTrackPanel_MouseMove(object sender, MouseEventArgs e)
		{
			switch (drag)
			{
			case DragMode.Pan:
				Ruler.Origin = Math.Max(0f, Ruler.Origin - (float)(e.X - drag_pt.X) / Ruler.MajorScale);
				drag_pt.X = e.X;
				break;
			case DragMode.Idle:
				Cursor = ((FindKey(e.X, e.Y, ref drag_node) != null) ? Cursors.Hand : Cursors.Default);
				break;
			case DragMode.Key:
				if (SelectedKeys.Count <= 0 || (drag_pt.X == e.X && drag_pt.Y == e.Y))
				{
					break;
				}
				if (!drag_moved)
				{
					EventHandler modifiedAction = this.ModifiedAction;
					if (modifiedAction != null)
					{
						modifiedAction(this, EventArgs.Empty);
					}
					EventHandler movingKey = this.MovingKey;
					if (movingKey != null)
					{
						movingKey(this, EventArgs.Empty);
					}
					drag_moved = true;
				}
				((IScrollableItemKeys)drag_node.Item).MoveKey(TimeLineControl, SelectedKeys[0], e.X - drag_pt.X);
				drag_pt.X = e.X;
				drag_pt.Y = e.Y;
				Invalidate();
				Update();
				break;
			case DragMode.Shuttle:
				break;
			}
		}

		private void TimeLineTrackPanel_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle && drag == DragMode.Pan)
			{
				drag = DragMode.Idle;
				base.Capture = false;
				Cursor = Cursors.Default;
			}
			else if (e.Button == MouseButtons.Left && drag == DragMode.Key)
			{
				EventHandler movingKeyEnd = this.MovingKeyEnd;
				if (movingKeyEnd != null)
				{
					movingKeyEnd(this, EventArgs.Empty);
				}
				drag = DragMode.Idle;
				base.Capture = false;
				Cursor = Cursors.Default;
				Invalidate();
			}
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
			this.BackColor = System.Drawing.Color.DarkGray;
			this.DoubleBuffered = true;
			base.Name = "TimeLineTrackPanel";
			base.Size = new System.Drawing.Size(351, 179);
			base.Paint += new System.Windows.Forms.PaintEventHandler(TimeLineTrackPanel_Paint);
			base.MouseMove += new System.Windows.Forms.MouseEventHandler(TimeLineTrackPanel_MouseMove);
			base.MouseDown += new System.Windows.Forms.MouseEventHandler(TimeLineTrackPanel_MouseDown);
			base.MouseUp += new System.Windows.Forms.MouseEventHandler(TimeLineTrackPanel_MouseUp);
			base.ResumeLayout(false);
		}
	}
}
