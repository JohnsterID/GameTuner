using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.Controls.Graph;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework.Graph
{
	public class BlockCurvePopup : Form
	{
		private enum ButtonType
		{
			Type,
			Close,
			Reset,
			VFlip,
			HFlip,
			None
		}

		private Rectangle blockRect;

		private int selected;

		private int drag;

		private bool modified;

		private int[] pts = new int[3];

		private BlockCurveInfo[] blocks;

		private IFlowGraphBlockCurve blockCurve;

		private Point dragPt;

		private IContainer components;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IFlowGraphBlockCurve BlockCurve
		{
			get
			{
				return blockCurve;
			}
			set
			{
				blockCurve = value;
				if (blockCurve != null)
				{
					for (int i = 0; i < 3; i++)
					{
						blocks[i] = new BlockCurveInfo(blockCurve.GetBlock(i));
					}
				}
				selected = -1;
				Invalidate();
			}
		}

		public event EventHandler BeginModifiedAction;

		public event EventHandler EndModifiedAction;

		public BlockCurvePopup()
		{
			InitializeComponent();
			dragPt = Point.Empty;
			blockRect = Rectangle.Empty;
			blocks = new BlockCurveInfo[3];
			selected = -1;
			drag = -1;
			base.LostFocus += BlockCurvePopup_LostFocus;
		}

		private void ResetCurve()
		{
			if (blockCurve != null)
			{
				for (int i = 0; i < 3; i++)
				{
					blocks[i] = new BlockCurveInfo(blockCurve.GetBlock(i));
				}
			}
			Invalidate();
		}

		private void BlockCurveControl_Paint(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			DrawingHelper.DrawImage(graphics, GameTuner.Framework.Properties.Resources.bc_reset, 179f, 100f);
			DrawingHelper.DrawImage(graphics, GameTuner.Framework.Properties.Resources.bc_close, 203f, 100f);
			DrawingHelper.DrawImage(graphics, GameTuner.Framework.Properties.Resources.bc_type, 5f, 100f);
			DrawingHelper.DrawImage(graphics, GameTuner.Framework.Properties.Resources.bc_vflip, 26f, 100f);
			DrawingHelper.DrawImage(graphics, GameTuner.Framework.Properties.Resources.bc_hflip, 47f, 100f);
			DrawCurve(graphics, base.ClientRectangle);
		}

		public void ShowIt()
		{
			Show();
			base.WindowState = FormWindowState.Normal;
			Activate();
			Focus();
		}

		private void HideIt()
		{
			if (BlockCurve != null)
			{
				if (modified)
				{
					EventHandler beginModifiedAction = this.BeginModifiedAction;
					if (beginModifiedAction != null)
					{
						beginModifiedAction(this, EventArgs.Empty);
					}
					for (int i = 0; i < 3; i++)
					{
						BlockCurve.GetBlock(i).Set(blocks[i]);
					}
					beginModifiedAction = this.EndModifiedAction;
					if (beginModifiedAction != null)
					{
						beginModifiedAction(this, EventArgs.Empty);
					}
				}
				modified = false;
				BlockCurve = null;
			}
			Hide();
		}

		private void DrawCurve(Graphics g, Rectangle r)
		{
			using (StringFormat stringFormat = new StringFormat())
			{
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				if (BlockCurve == null)
				{
					g.DrawString("No Curve Selected", Font, Brushes.Black, r);
					return;
				}
				stringFormat.LineAlignment = StringAlignment.Near;
				Point location = new Point(40, 24);
				blockRect.Location = location;
				blockRect.Width = 144;
				blockRect.Height = 48;
				float num = 0f;
				Rectangle empty = Rectangle.Empty;
				for (int i = 0; i < 3; i++)
				{
					BlockCurveInfo blockCurveInfo = blocks[i];
					int num2 = (int)(blockCurveInfo.Dist * 48f / 0.33f);
					empty.Location = location;
					empty.Width = num2;
					empty.Height = 48;
					if (num2 > 0)
					{
						g.DrawImage(GameTuner.Framework.Properties.Resources.bc_grid, empty, 0, 0, GameTuner.Framework.Properties.Resources.bc_grid.Width, GameTuner.Framework.Properties.Resources.bc_grid.Height, GraphicsUnit.Pixel);
						FlowGraphControl.PaintBlock(g, blockCurveInfo, location.X, location.Y, num2, 48f);
						if (selected == i)
						{
							g.DrawImage(GameTuner.Framework.Properties.Resources.bc_select, empty, 0, 0, GameTuner.Framework.Properties.Resources.bc_select.Width, GameTuner.Framework.Properties.Resources.bc_select.Height, GraphicsUnit.Pixel);
						}
					}
					num += blockCurveInfo.Dist;
					location.X += num2;
					pts[i] = location.X;
					if (i != 2)
					{
						DrawingHelper.DrawImage(g, GameTuner.Framework.Properties.Resources.bc_arrow, location.X - GameTuner.Framework.Properties.Resources.bc_arrow.Width / 2, location.Y - GameTuner.Framework.Properties.Resources.bc_arrow.Height - 1);
						g.DrawString(string.Format("{0:F2}", num), Font, Brushes.Black, location.X, blockRect.Bottom + 2, stringFormat);
					}
				}
			}
		}

		private void BlockCurvePopup_LostFocus(object sender, EventArgs e)
		{
			HideIt();
		}

		private void BlockCurvePopup_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				HideIt();
			}
		}

		private ButtonType HitButton(Point pt)
		{
			int[] array = new int[5] { 5, 203, 179, 26, 47 };
			for (int i = 0; i < 5; i++)
			{
				if (Math.Abs(pt.X - (array[i] + 8)) < 8 && Math.Abs(pt.Y - 110) < 8)
				{
					return (ButtonType)i;
				}
			}
			return ButtonType.None;
		}

		private int FindBlock(Point pt)
		{
			if (BlockCurve != null && blockRect.Contains(pt.X, pt.Y))
			{
				float num = (float)(pt.X - blockRect.X) / (float)blockRect.Width;
				float num2 = 0f;
				for (int i = 0; i < 3; i++)
				{
					num2 += blocks[i].Dist;
					if (num < num2)
					{
						return i;
					}
				}
			}
			return -1;
		}

		private bool HandleHitButton(Point pt)
		{
			switch (HitButton(dragPt))
			{
			case ButtonType.Close:
				HideIt();
				return true;
			case ButtonType.Reset:
				ResetCurve();
				return true;
			case ButtonType.Type:
				CycleSelected();
				return true;
			case ButtonType.VFlip:
				if (selected != -1)
				{
					blocks[selected].FlipY = 1 - blocks[selected].FlipY;
					modified = true;
					Invalidate();
				}
				return true;
			case ButtonType.HFlip:
				if (selected != -1)
				{
					blocks[selected].FlipX = 1 - blocks[selected].FlipX;
					modified = true;
					Invalidate();
				}
				return true;
			default:
				return false;
			}
		}

		private void BlockCurvePopup_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			dragPt.X = e.X;
			dragPt.Y = e.Y;
			if (HandleHitButton(dragPt))
			{
				return;
			}
			drag = -1;
			for (int i = 0; i < 2; i++)
			{
				if (HitArrow(dragPt, i))
				{
					drag = i;
					base.Capture = true;
					return;
				}
			}
			selected = FindBlock(dragPt);
			Invalidate();
		}

		private bool HitArrow(Point pt, int idx)
		{
			Image bc_arrow = GameTuner.Framework.Properties.Resources.bc_arrow;
			if (BlockCurve != null && Math.Abs(pt.X - pts[idx]) < bc_arrow.Width / 2)
			{
				return Math.Abs(pt.Y - (24 - bc_arrow.Height / 2)) < bc_arrow.Width / 2;
			}
			return false;
		}

		private void BlockCurvePopup_MouseMove(object sender, MouseEventArgs e)
		{
			if (drag != -1)
			{
				int num = Math.Min(Math.Max(Math.Min(e.X, blockRect.Right), blockRect.Left), pts[drag + 1]);
				if (drag == 1)
				{
					num = Math.Max(pts[0], num);
				}
				float num2 = (float)(num - blockRect.X) / ((float)blockRect.Width * 1f);
				switch (drag)
				{
				case 0:
					blocks[0].Dist = num2;
					blocks[1].Dist = 1f - num2 - blocks[2].Dist;
					modified = true;
					break;
				case 1:
					blocks[1].Dist = num2 - blocks[0].Dist;
					blocks[2].Dist = 1f - num2;
					modified = true;
					break;
				}
				Invalidate();
				Update();
			}
			else
			{
				Point pt = new Point(e.X, e.Y);
				if (HitButton(pt) != ButtonType.None || FindBlock(pt) >= 0)
				{
					Cursor = CustomCursors.Get(CustomCursor.FingerPoint);
				}
				else if (HitArrow(pt, 0) || HitArrow(pt, 1))
				{
					Cursor = CustomCursors.Get(CustomCursor.HSplit);
				}
				else
				{
					Cursor = Cursors.Default;
				}
			}
		}

		private void CycleSelected()
		{
			if (selected != -1)
			{
				int num = (int)(blocks[selected].Type + 1);
				if (num == 4)
				{
					num = -1;
				}
				blocks[selected].Type = (BlockCurveType)num;
				modified = true;
				Invalidate();
			}
		}

		private void BlockCurvePopup_MouseUp(object sender, MouseEventArgs e)
		{
			if (drag != -1 && e.Button == MouseButtons.Left)
			{
				drag = -1;
				Invalidate();
				base.Capture = false;
			}
		}

		private void BlockCurvePopup_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				dragPt.X = e.X;
				dragPt.Y = e.Y;
				int num = FindBlock(dragPt);
				if (num != -1)
				{
					selected = num;
					CycleSelected();
				}
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
			this.BackColor = System.Drawing.Color.White;
			base.ClientSize = new System.Drawing.Size(221, 126);
			this.DoubleBuffered = true;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "BlockCurvePopup";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Block Curve";
			base.TopMost = true;
			base.MouseUp += new System.Windows.Forms.MouseEventHandler(BlockCurvePopup_MouseUp);
			base.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(BlockCurvePopup_MouseDoubleClick);
			base.Paint += new System.Windows.Forms.PaintEventHandler(BlockCurveControl_Paint);
			base.MouseDown += new System.Windows.Forms.MouseEventHandler(BlockCurvePopup_MouseDown);
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(BlockCurvePopup_FormClosing);
			base.MouseMove += new System.Windows.Forms.MouseEventHandler(BlockCurvePopup_MouseMove);
			base.ResumeLayout(false);
		}
	}
}
