using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public class ToolTipView : Form, INotifyPopupView, IToolTipView
	{
		private const int maxDescriptionWidth = 200;

		private const int fadeRate = 45;

		private Image image;

		private string caption;

		private string descripton;

		private bool showImage;

		private bool showDescription;

		private PointF ptMargin = new PointF(4f, 4f);

		private SizeF szLabel;

		private SizeF szDesc;

		private Font labelFont;

		private ToolTipFadeMode fadeMode;

		private int fadeTime;

		private IContainer components;

		private Timer timer;

		public ToolTipFadeMode FadeMode
		{
			get
			{
				return fadeMode;
			}
		}

		public bool ShowImage
		{
			get
			{
				return showImage;
			}
			set
			{
				showImage = value;
			}
		}

		public bool ShowDescription
		{
			get
			{
				return showDescription;
			}
			set
			{
				showDescription = value;
			}
		}

		public Image Image
		{
			get
			{
				return image;
			}
			set
			{
				image = value;
			}
		}

		public string Caption
		{
			get
			{
				return caption;
			}
			set
			{
				caption = value;
			}
		}

		public string Description
		{
			get
			{
				return descripton;
			}
			set
			{
				descripton = value;
			}
		}

		public Font LabelFont
		{
			get
			{
				return labelFont;
			}
			set
			{
				labelFont = value;
			}
		}

		[Browsable(false)]
		public Point Position
		{
			get
			{
				return base.Location;
			}
			set
			{
				base.Location = value;
			}
		}

		protected override bool ShowWithoutActivation
		{
			get
			{
				return true;
			}
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= 32;
				return createParams;
			}
		}

		private SizeF ImageSize
		{
			get
			{
				if (image != null && showImage)
				{
					return new SizeF(image.Width, image.Height);
				}
				return SizeF.Empty;
			}
		}

		public Size TipSize
		{
			get
			{
				CalcSize();
				return base.Size;
			}
		}

		public event MouseEventHandler ClickedNotification;

		public ToolTipView()
		{
			fadeMode = ToolTipFadeMode.None;
			showImage = true;
			showDescription = true;
			labelFont = new Font("Tahoma", 8.25f, FontStyle.Bold);
			InitializeComponent();
		}

		private void SetFade(ToolTipFadeMode fade)
		{
			fadeMode = fade;
			fadeTime = 0;
			base.Opacity = ((fade == ToolTipFadeMode.In || fade == ToolTipFadeMode.OutComplete) ? 0.0 : 1.0);
			timer.Enabled = fade == ToolTipFadeMode.In || fade == ToolTipFadeMode.Out;
		}

		public void Dismiss()
		{
			Hide();
		}

		public void ShowTip(Control parent, bool show)
		{
			if (show)
			{
				SetFade(ToolTipFadeMode.In);
				CalcSize();
				if (parent == null)
				{
					base.TopMost = true;
				}
				NativeMethods.ShowWindow(base.Handle, 8);
			}
			else
			{
				SetFade(ToolTipFadeMode.Out);
			}
		}

		protected virtual void CalcSize()
		{
			SizeF sizeF = new SizeF(200f, 0f);
			SizeF imageSize = ImageSize;
			Graphics graphics = Graphics.FromHwnd(base.Handle);
			SizeF sizeF2 = (szLabel = graphics.MeasureString(caption, LabelFont, (int)sizeF.Width));
			if (!showDescription || descripton.Length == 0)
			{
				sizeF2.Height = Math.Max(sizeF2.Height, imageSize.Height);
			}
			else
			{
				SizeF layoutArea = new SizeF(200f, sizeF.Height);
				SizeF sizeF3 = (szDesc = graphics.MeasureString(descripton, Font, layoutArea));
				sizeF2.Width = Math.Max(sizeF2.Width, sizeF3.Width);
				sizeF2.Height = Math.Max(sizeF2.Height + sizeF3.Height, imageSize.Height);
			}
			if (showImage && image != null)
			{
				sizeF2.Width += imageSize.Width;
			}
			sizeF2.Width += ptMargin.X * 2f;
			sizeF2.Height += ptMargin.Y * 2f;
			base.Size = new Size((int)sizeF2.Width, (int)sizeF2.Height);
		}

		protected virtual void DoPaint(Graphics g)
		{
			SizeF imageSize = ImageSize;
			g.DrawRectangle(Pens.Gray, 0, 0, base.Size.Width - 1, base.Size.Height - 1);
			Brush brush = new SolidBrush(ForeColor);
			StringFormat stringFormat = new StringFormat();
			stringFormat.LineAlignment = StringAlignment.Center;
			RectangleF layoutRectangle = new RectangleF(imageSize.Width + ptMargin.X, ptMargin.Y, szLabel.Width, szLabel.Height);
			g.DrawString(caption, LabelFont, brush, layoutRectangle, stringFormat);
			if (showImage && image != null)
			{
				g.DrawImage(image, ptMargin.X, ptMargin.Y, image.Width, image.Height);
			}
			if (showDescription && descripton.Length != 0)
			{
				RectangleF layoutRectangle2 = new RectangleF(imageSize.Width + ptMargin.X, ptMargin.Y + szLabel.Height, szDesc.Width, szDesc.Height);
				g.DrawString(descripton, Font, brush, layoutRectangle2);
			}
		}

		private void ToolTipView_Paint(object sender, PaintEventArgs e)
		{
			DoPaint(e.Graphics);
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			fadeTime += timer.Interval;
			if (fadeTime >= 45)
			{
				switch (fadeMode)
				{
				case ToolTipFadeMode.In:
					SetFade(ToolTipFadeMode.InComplete);
					break;
				case ToolTipFadeMode.Out:
					SetFade(ToolTipFadeMode.OutComplete);
					Hide();
					break;
				case ToolTipFadeMode.InComplete:
					break;
				}
			}
			else
			{
				switch (fadeMode)
				{
				case ToolTipFadeMode.In:
				case ToolTipFadeMode.Out:
				{
					double num = (double)Math.Min(fadeTime, 45) / 45.0;
					base.Opacity = ((fadeMode == ToolTipFadeMode.Out) ? (1.0 - num) : num);
					break;
				}
				case ToolTipFadeMode.InComplete:
					break;
				}
			}
		}

		private void ToolTipView_MouseDown(object sender, MouseEventArgs e)
		{
			MouseEventHandler clickedNotification = this.ClickedNotification;
			if (clickedNotification != null)
			{
				clickedNotification(sender, e);
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
			this.components = new System.ComponentModel.Container();
			this.timer = new System.Windows.Forms.Timer(this.components);
			base.SuspendLayout();
			this.timer.Interval = 5;
			this.timer.Tick += new System.EventHandler(timer_Tick);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
			this.BackColor = System.Drawing.SystemColors.Info;
			base.ClientSize = new System.Drawing.Size(224, 39);
			base.ControlBox = false;
			this.ForeColor = System.Drawing.SystemColors.InfoText;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ToolTipView";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "ToolTipView";
			base.Paint += new System.Windows.Forms.PaintEventHandler(ToolTipView_Paint);
			base.MouseDown += new System.Windows.Forms.MouseEventHandler(ToolTipView_MouseDown);
			base.ResumeLayout(false);
		}
	}
}
