using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public class GridControl : UserControl
	{
		private const double MinGridSize = 50.0;

		private const double MinGridScale = 1E-05;

		private const double MaxGridScale = 1000.0;

		private const double MinOffset = -100000000.0;

		private const double MaxOffset = 100000000.0;

		private const float GridTextLeftMargin = 2f;

		private const float GridTextBottomMargin = 2f;

		private static double[] _span_table = new double[7] { 0.1, 0.2, 0.5, 1.0, 2.0, 4.0, 5.0 };

		private Color gridTextColor = Color.Black;

		private Color gridLineColor = Color.FromArgb(200, 205, 211);

		private Color gridBoldLineColor = Color.FromArgb(100, 102, 106);

		private bool gridVisible = true;

		private double[] gridSpans = new double[2] { 0.1, 0.1 };

		private double[] gridOffsets = new double[2];

		private double[] gridScale = new double[2];

		private double[] invGridScale = new double[2];

		private Vector2 viewSize = default(Vector2);

		private IContainer components;

		[Category("Grid Control")]
		[DefaultValue(true)]
		[Description("Turn on/off the grid.")]
		public bool GridVisible
		{
			get
			{
				return gridVisible;
			}
			set
			{
				gridVisible = value;
			}
		}

		[Category("Grid Control")]
		[Description("The text color of grid.")]
		[DefaultValue(typeof(Color), "Black")]
		public Color GridTextColor
		{
			get
			{
				return gridTextColor;
			}
			set
			{
				gridTextColor = value;
			}
		}

		[DefaultValue(typeof(Color), "200, 205, 211")]
		[Category("Grid Control")]
		[Description("The line color of grid.")]
		public Color GridLineColor
		{
			get
			{
				return gridLineColor;
			}
			set
			{
				gridLineColor = value;
			}
		}

		[Description("The bold line color of grid.")]
		[Category("Grid Control")]
		[DefaultValue(typeof(Color), "100, 102, 106")]
		public Color GridBoldLineColor
		{
			get
			{
				return gridBoldLineColor;
			}
			set
			{
				gridBoldLineColor = value;
			}
		}

		[Browsable(false)]
		public Vector2 ViewSize
		{
			get
			{
				return viewSize;
			}
		}

		[Browsable(false)]
		public double OffsetX
		{
			get
			{
				return gridOffsets[0];
			}
		}

		[Browsable(false)]
		public double OffsetY
		{
			get
			{
				return gridOffsets[1];
			}
		}

		[Browsable(false)]
		public double ScaleX
		{
			get
			{
				return gridScale[0];
			}
		}

		[Browsable(false)]
		public double ScaleY
		{
			get
			{
				return gridScale[1];
			}
		}

		public GridControl()
		{
			InitializeComponent();
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			viewSize = new Vector2(base.Width, base.Height);
			gridScale[0] = 2.0 / (double)viewSize.X;
			gridScale[1] = 2.0 / (double)viewSize.Y;
			invGridScale[0] = 1.0 / gridScale[0];
			invGridScale[1] = 1.0 / gridScale[1];
			gridOffsets[0] = (gridOffsets[1] = -1.0);
			CalcGridSpans();
		}

		public Vector2 ToPixelCoordinate(float position, float value)
		{
			double num = ((double)position - gridOffsets[0]) * invGridScale[0];
			double num2 = (double)viewSize.Y - ((double)value - gridOffsets[1]) * invGridScale[1];
			return new Vector2((float)num, (float)num2);
		}

		public Vector2 ToPixelCoordinate(Vector2 unitPos)
		{
			return ToPixelCoordinate(unitPos.X, unitPos.Y);
		}

		public Vector2 ToUnitCoordinate(Vector2 pixelPos)
		{
			double num = gridOffsets[0] + (double)pixelPos.X * gridScale[0];
			double num2 = gridOffsets[1] + ((double)viewSize.Y - (double)pixelPos.Y) * gridScale[1];
			return new Vector2((float)num, (float)num2);
		}

		public Vector2 ToUnitCoordinate(float xp, float yp)
		{
			double num = gridOffsets[0] + (double)xp * gridScale[0];
			double num2 = gridOffsets[1] + ((double)viewSize.Y - (double)yp) * gridScale[1];
			return new Vector2((float)num, (float)num2);
		}

		public float UnitToScreenTangentAngle(float unitTangent)
		{
			return (float)((double)unitTangent / (gridScale[1] / gridScale[0]));
		}

		public void Pan(Vector2 dv)
		{
			SetOffset(gridOffsets[0] - (double)dv.X * gridScale[0], gridOffsets[1] - (double)dv.Y * (0.0 - gridScale[1]));
		}

		public void Zoom(Vector2 dv)
		{
			double num = (double)dv.X * gridScale[0];
			double num2 = (double)dv.Y * (0.0 - gridScale[1]);
			double num3 = -0.001 * ((Math.Abs(num) > Math.Abs(num2)) ? num : num2);
			Zoom(num3, num3);
		}

		public void Zoom(int wheelDelta)
		{
			float num = (float)wheelDelta / 120f;
			double dx = (double)num * 0.1 * gridScale[0];
			double dy = (double)num * 0.1 * gridScale[1];
			Zoom(dx, dy);
		}

		public bool Frame(Vector2 min, Vector2 max)
		{
			bool result = false;
			if (min.X == max.X)
			{
				max.X = min.X + 1f;
			}
			if (min.Y == max.Y)
			{
				max.Y = min.Y + 1f;
			}
			Vector2 vector = Vector2.Max(max - min, new Vector2(1E-05f, 1E-05f));
			float num = 50f;
			double[] array = new double[2]
			{
				(double)vector.X / (double)Math.Max(viewSize.X - num, 10f),
				(double)vector.Y / (double)Math.Max(viewSize.Y - num, 10f)
			};
			double num2 = 0.0001;
			if (Math.Abs(array[0] - gridScale[0]) > num2 || Math.Abs(array[1] - gridScale[1]) > num2)
			{
				SetScale(array[0], array[1]);
				SetOffset((double)min.X - gridScale[0] * (double)num * 0.5, (double)min.Y - gridScale[1] * (double)num * 0.5);
				result = true;
			}
			return result;
		}

		public void DrawGridLines(Graphics g)
		{
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			CalcGridSpans();
			if (!gridVisible)
			{
				return;
			}
			Pen pen = new Pen(GridLineColor);
			Pen pen2 = new Pen(GridBoldLineColor);
			Brush brush = new SolidBrush(GridTextColor);
			float num = g.MeasureString("-+123456789E", Font).Height;
			double num2 = gridOffsets[0] - gridOffsets[0] % gridSpans[0];
			double num3 = gridOffsets[1] - gridOffsets[1] % gridSpans[1];
			double num4 = num2 + gridScale[0] * (double)viewSize.X;
			double num5 = num3 + gridScale[1] * (double)viewSize.Y;
			for (double num6 = num2; num6 < num4; num6 += gridSpans[0])
			{
				int num7 = (int)((num6 - gridOffsets[0]) * invGridScale[0]);
				bool flag = Math.Abs(num6) < 1E-08;
				g.DrawLine(flag ? pen2 : pen, num7, 0f, num7, viewSize.Y);
			}
			for (double num8 = num3; num8 < num5; num8 += gridSpans[1])
			{
				int num9 = (int)((num8 - gridOffsets[1]) * invGridScale[1]);
				int num10 = (int)viewSize.Y - num9;
				bool flag2 = Math.Abs(num8) < 1E-08;
				g.DrawLine(flag2 ? pen2 : pen, 0f, num10, viewSize.X, num10);
			}
			float num11 = -100f;
			for (double num12 = num2; num12 < num4; num12 += gridSpans[0])
			{
				int num13 = (int)((num12 - gridOffsets[0]) * invGridScale[0]);
				string s = ToNumberString(num12);
				SizeF sizeF = g.MeasureString(s, Font);
				float num14 = sizeF.Width * 0.5f;
				float num15 = (float)num13 - num14;
				if (num11 < num15)
				{
					num11 = (float)num13 + num14;
					g.DrawString(s, Font, brush, num15, viewSize.Y - sizeF.Height - 2f);
				}
			}
			float num16 = num * 0.5f;
			float num17 = viewSize.Y - 2f * num - 2f;
			num11 = viewSize.Y + 100f;
			for (double num18 = num3; num18 < num5; num18 += gridSpans[1])
			{
				int num19 = (int)((num18 - gridOffsets[1]) * invGridScale[1]);
				float num20 = viewSize.Y - (float)num19 - num16;
				if (num11 > num20 && num20 < num17)
				{
					num11 = num20 - num16;
					g.DrawString(ToNumberString(num18), Font, brush, 2f, num20);
				}
			}
		}

		private void Zoom(double dx, double dy)
		{
			double num = gridScale[0];
			double num2 = gridScale[1];
			SetScale(gridScale[0] + dx, gridScale[1] + dy);
			dx = gridScale[0] - num;
			dy = gridScale[1] - num2;
			gridOffsets[0] -= dx * (double)viewSize.X * 0.5;
			gridOffsets[1] -= dy * (double)viewSize.Y * 0.5;
		}

		private void CalcGridSpans()
		{
			if ((double)viewSize.X >= 50.0)
			{
				gridSpans[0] = ComputeGridSpan(50.0 * gridScale[0]);
			}
			if ((double)viewSize.Y >= 50.0)
			{
				gridSpans[1] = ComputeGridSpan(50.0 * gridScale[1]);
			}
		}

		private static string ToNumberString(double value)
		{
			return Math.Round(value, 9).ToString();
		}

		private void GridControl_Resize(object sender, EventArgs e)
		{
			Vector2 vector = Vector2.Max(new Vector2(base.Width, base.Height), Vector2.One * 10f);
			Vector2 vector2 = viewSize / vector;
			SetScale(gridScale[0] * (double)vector2.X, gridScale[1] * (double)vector2.Y);
			viewSize = vector;
			Invalidate(true);
		}

		private static double ComputeGridSpan(double attendedSpan)
		{
			double num = Math.Truncate(Math.Log10(attendedSpan));
			double num2 = Math.Pow(10.0, num);
			double result = 0.0;
			double num3 = double.MaxValue;
			for (int i = 0; i < _span_table.Length; i++)
			{
				double num4 = num2 * _span_table[i];
				double num5 = Math.Abs(num4 - attendedSpan);
				if (num5 < num3)
				{
					num3 = num5;
					result = num4;
				}
			}
			return result;
		}

		private void SetOffset(double x, double y)
		{
			gridOffsets[0] = Math.Min(Math.Max(x, -100000000.0), 100000000.0);
			gridOffsets[1] = Math.Min(Math.Max(y, -100000000.0), 100000000.0);
		}

		private void SetScale(double x, double y)
		{
			gridScale[0] = Math.Min(Math.Max(x, 1E-05), 1000.0);
			gridScale[1] = Math.Min(Math.Max(y, 1E-05), 1000.0);
			invGridScale[0] = 1.0 / gridScale[0];
			invGridScale[1] = 1.0 / gridScale[1];
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
			base.Name = "GridControl";
			base.Size = new System.Drawing.Size(252, 154);
			base.ResumeLayout(false);
		}
	}
}
