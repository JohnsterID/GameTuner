using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace GameTuner.Framework.Controls
{
	public class ProgressBarEx : UserControl
	{
		private string label;

		private bool labelVisible;

		private bool useStyles;

		private int currentValue;

		private int maximum;

		private int minimum;

		private StringAlignment labelAlignment;

		private VisualStyleElement pulseOverlay;

		private VisualStyleRenderer pulseRenderer;

		private VisualStyleElement moveOverlay;

		private VisualStyleRenderer moveRenderer;

		private Timer timer;

		private int cycle;

		[DefaultValue(10)]
		[Category("Behavior")]
		public int Step { get; set; }

		[Category("Behavior")]
		[DefaultValue(100)]
		public int Maximum
		{
			get
			{
				return maximum;
			}
			set
			{
				maximum = Math.Max(minimum, value);
				ClampValue();
				Invalidate();
			}
		}

		[DefaultValue(0)]
		[Category("Behavior")]
		public int Minimum
		{
			get
			{
				return minimum;
			}
			set
			{
				minimum = Math.Min(maximum, value);
				ClampValue();
				Invalidate();
			}
		}

		[DefaultValue(0)]
		[Bindable(true)]
		[Category("Behavior")]
		public int Value
		{
			get
			{
				return currentValue;
			}
			set
			{
				currentValue = value;
				ClampValue();
				Invalidate();
				Update();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public new bool TabStop { get; set; }

		[Bindable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public override string Text { get; set; }

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Padding Padding { get; set; }

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new ImeMode ImeMode { get; set; }

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Image BackgroundImage { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public override ImageLayout BackgroundImageLayout { get; set; }

		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("The text to display on top of the progress bar")]
		public string Label
		{
			get
			{
				return label;
			}
			set
			{
				label = value;
				Invalidate();
				Update();
			}
		}

		[DefaultValue(StringAlignment.Center)]
		[Description("Horizontal Label positioning")]
		[Category("Appearance")]
		public StringAlignment LabelAlignment
		{
			get
			{
				return labelAlignment;
			}
			set
			{
				labelAlignment = value;
				Invalidate();
			}
		}

		[Description("Set to True for the label to be visible on top")]
		[Category("Appearance")]
		[DefaultValue(true)]
		public bool LabelVisible
		{
			get
			{
				return labelVisible;
			}
			set
			{
				labelVisible = value;
				Invalidate();
			}
		}

		public ProgressBarEx()
		{
			DoubleBuffered = true;
			InitializeComponent();
			useStyles = VisualStyleRenderer.IsSupported;
			if (useStyles)
			{
				ConstructorInfo constructor = typeof(VisualStyleElement).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[3]
				{
					typeof(string),
					typeof(int),
					typeof(int)
				}, null);
				pulseOverlay = (VisualStyleElement)constructor.Invoke(new object[3] { "PROGRESS", 7, 0 });
				moveOverlay = (VisualStyleElement)constructor.Invoke(new object[3] { "PROGRESS", 8, 0 });
				pulseRenderer = new VisualStyleRenderer(pulseOverlay);
				moveRenderer = new VisualStyleRenderer(moveOverlay);
			}
			timer = new Timer();
			timer.Interval = 10;
			timer.Enabled = true;
			timer.Tick += timer_Tick;
			labelAlignment = StringAlignment.Center;
			labelVisible = true;
			label = "";
			minimum = 0;
			maximum = 100;
			currentValue = 0;
			Step = 10;
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			cycle = (cycle + 3) % (base.Width * 2);
			Invalidate();
		}

		public void PerformStep()
		{
			Value += Step;
		}

		private void ClampValue()
		{
			currentValue = Math.Max(Math.Min(maximum, currentValue), minimum);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			Rectangle clientRectangle = base.ClientRectangle;
			if (useStyles)
			{
				ProgressBarRenderer.DrawHorizontalBar(graphics, clientRectangle);
			}
			else
			{
				graphics.FillRectangle(Brushes.White, clientRectangle);
			}
			if (maximum - minimum > 0)
			{
				clientRectangle.Inflate(-2, -2);
				clientRectangle.Width = (currentValue - Minimum) * clientRectangle.Width / (maximum - minimum);
				if (!useStyles)
				{
					graphics.FillRectangle(SystemBrushes.Highlight, clientRectangle);
				}
				else
				{
					ProgressBarRenderer.DrawHorizontalChunks(graphics, clientRectangle);
					using (Region clip = new Region(clientRectangle))
					{
						graphics.Clip = clip;
						clientRectangle.Offset(cycle - base.Width, 0);
						clientRectangle.Width = 50;
						pulseRenderer.DrawBackground(graphics, clientRectangle);
						moveRenderer.DrawBackground(graphics, clientRectangle);
						graphics.ResetClip();
					}
				}
			}
			if (!LabelVisible)
			{
				return;
			}
			using (StringFormat stringFormat = new StringFormat())
			{
				stringFormat.LineAlignment = StringAlignment.Center;
				stringFormat.Alignment = labelAlignment;
				using (Brush brush = new SolidBrush(ForeColor))
				{
					graphics.DrawString(Label, Font, brush, base.ClientRectangle, stringFormat);
				}
			}
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.Name = "ProgressBarEx";
			base.Size = new System.Drawing.Size(129, 26);
			base.ResumeLayout(false);
		}
	}
}
