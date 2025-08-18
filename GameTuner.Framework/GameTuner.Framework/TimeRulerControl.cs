using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public class TimeRulerControl : UserControl
	{
		public enum HitLocation
		{
			Nothing,
			Shuttle,
			RangeBegin,
			RangeEnd,
			Range
		}

		private enum DragMode
		{
			Idle,
			Pan,
			Shuttle,
			RangeLeft,
			RangeRight
		}

		public class TimeRulerEventArgs : EventArgs
		{
			public float Time { get; private set; }

			public TimeRulerEventArgs(float time)
			{
				Time = time;
			}
		}

		public delegate void TimeRuleEventHandler(object sender, TimeRulerEventArgs e);

		public const int PadX = 10;

		public const int PadY = 1;

		private DragMode drag;

		private Point drag_pt;

		private float current_time;

		private float major_scale;

		private float origin;

		private float range_begin;

		private float range_duration;

		private bool trackRangeVisible;

		private IContainer components;

		public Color TickColor { get; set; }

		public Color ShuttleColor { get; set; }

		public Color RangeColor { get; set; }

		public bool ShuttleVisible { get; set; }

		public float RangeStart
		{
			get
			{
				return range_begin;
			}
			set
			{
				range_begin = Math.Max(0f, value);
				OnRangeChanged(EventArgs.Empty);
				Invalidate();
			}
		}

		public float RangeDuration
		{
			get
			{
				return range_duration;
			}
			set
			{
				range_duration = Math.Max(0f, value);
				OnRangeChanged(EventArgs.Empty);
				Invalidate();
			}
		}

		public bool TrackRangeVisible
		{
			get
			{
				return trackRangeVisible;
			}
			set
			{
				trackRangeVisible = value;
				Invalidate();
			}
		}

		public float CurrentTime
		{
			get
			{
				return current_time;
			}
			set
			{
				current_time = value;
				Invalidate();
				Update();
				TimeRuleEventHandler currentTimeChanged = this.CurrentTimeChanged;
				if (currentTimeChanged != null)
				{
					currentTimeChanged(this, new TimeRulerEventArgs(current_time));
				}
			}
		}

		public float MajorScale
		{
			get
			{
				return major_scale;
			}
			set
			{
				major_scale = value;
				EventHandler scaleChanged = this.ScaleChanged;
				if (scaleChanged != null)
				{
					scaleChanged(this, EventArgs.Empty);
				}
				Invalidate();
				Update();
			}
		}

		public float Origin
		{
			get
			{
				return origin;
			}
			set
			{
				origin = value;
				Invalidate();
				Update();
				EventHandler originChanged = this.OriginChanged;
				if (originChanged != null)
				{
					originChanged(this, EventArgs.Empty);
				}
			}
		}

		public float TimeSpan
		{
			get
			{
				return (float)base.ClientSize.Width / MajorScale;
			}
		}

		public event TimeRuleEventHandler CurrentTimeChanged;

		public event EventHandler OriginChanged;

		public event EventHandler ScaleChanged;

		public event EventHandler RangeChanged;

		public event PaintEventHandler PreShuttlePaint;

		public event TimeRuleEventHandler UserTimeChanged;

		public TimeRulerControl()
		{
			InitializeComponent();
			ShuttleVisible = true;
			drag = DragMode.Idle;
			origin = 0f;
			current_time = 0f;
			major_scale = 540f;
			range_begin = 0f;
			range_duration = 3f;
			TickColor = Color.FromArgb(186, 182, 169);
			ShuttleColor = Color.FromArgb(216, 30, 0);
			RangeColor = Color.FromArgb(245, 164, 83);
		}

		public void SetTime(float time)
		{
			current_time = time;
			Invalidate();
		}

		protected virtual void OnRangeChanged(EventArgs e)
		{
			EventHandler rangeChanged = this.RangeChanged;
			if (rangeChanged != null)
			{
				rangeChanged(this, e);
			}
		}

		public int TimeToX(float time)
		{
			float num = time - Origin;
			return (int)(num * MajorScale) + 10;
		}

		public float XToTime(int x)
		{
			x -= 10;
			return (float)x / MajorScale + Origin;
		}

		public void EnsureVisible()
		{
			EnsureVisible(CurrentTime);
		}

		public void EnsureVisible(float time)
		{
			if (time < Origin)
			{
				Origin = Math.Max(0f, time - 64f / MajorScale);
			}
			else if (time > Origin + TimeSpan)
			{
				Origin = Math.Max(0f, time - TimeSpan / 2f);
			}
		}

		private void TimeRulerControl_Paint(object sender, PaintEventArgs e)
		{
			PaintRuler(sender, e);
		}

		protected virtual void PaintRuler(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			graphics.MeasureString("00:00:00:00", Font);
			float num = (float)Math.Truncate(Origin);
			float num2 = Origin + TimeSpan;
			float num3 = 0.125f / (MajorScale / 100f);
			Pen pen = new Pen(TickColor);
			int num5;
			for (float num4 = num; num4 < num2; num4 += num3)
			{
				num5 = TimeToX(num4);
				graphics.DrawLine(pen, num5, base.ClientSize.Height - 4, num5, base.ClientSize.Height - 2);
			}
			using (Brush brush = new SolidBrush(ForeColor))
			{
				using (StringFormat stringFormat = new StringFormat())
				{
					stringFormat.Alignment = StringAlignment.Center;
					for (float num4 = num; num4 < num2; num4 += 1f)
					{
						num5 = TimeToX(num4);
						graphics.DrawLine(pen, num5, base.ClientSize.Height - 8, num5, base.ClientSize.Height - 2);
						graphics.DrawString(TimeCode.ToString(num4, TimeCodeFormat.Seconds), Font, brush, num5, 1f, stringFormat);
					}
					PaintEventHandler preShuttlePaint = this.PreShuttlePaint;
					if (preShuttlePaint != null)
					{
						preShuttlePaint(sender, e);
					}
					if (TrackRangeVisible)
					{
						num5 = TimeToX(RangeStart);
						int num6 = (int)(RangeDuration * MajorScale);
						if (num6 > 0)
						{
							Rectangle rect = new Rectangle(num5, base.ClientSize.Height / 2, num6, base.ClientSize.Height / 2);
							using (Brush brush2 = new SolidBrush(Color.FromArgb(128, RangeColor)))
							{
								graphics.FillRectangle(brush2, rect);
							}
							using (Pen pen2 = new Pen(RangeColor))
							{
								graphics.DrawRectangle(pen2, rect);
							}
						}
					}
					if (!ShuttleVisible)
					{
						return;
					}
					using (Pen pen3 = new Pen(ShuttleColor))
					{
						using (Brush brush3 = new SolidBrush(Color.FromArgb(128, ShuttleColor)))
						{
							num5 = TimeToX(CurrentTime);
							Rectangle rect2 = new Rectangle(num5 - 3, base.ClientSize.Height - 22, 7, 16);
							graphics.FillRectangle(brush3, rect2);
							graphics.DrawRectangle(pen3, rect2);
							graphics.DrawLine(pen3, num5, rect2.Bottom, num5, base.ClientSize.Height);
						}
					}
					if (drag == DragMode.Shuttle)
					{
						using (Brush brush4 = new SolidBrush(ShuttleColor))
						{
							stringFormat.LineAlignment = StringAlignment.Center;
							stringFormat.Alignment = StringAlignment.Near;
							graphics.DrawString(TimeCode.ToString(CurrentTime, TimeCodeFormat.Frame), Font, brush4, new PointF(num5 + 5, base.ClientSize.Height / 2), stringFormat);
							return;
						}
					}
				}
			}
		}

		private HitLocation GetHitLocation(int x, int y)
		{
			int num = TimeToX(CurrentTime);
			if (Math.Abs(num - x) < 8)
			{
				return HitLocation.Shuttle;
			}
			num = TimeToX(RangeStart + RangeDuration);
			if (Math.Abs(num - x) < 8)
			{
				return HitLocation.RangeEnd;
			}
			num = TimeToX(RangeStart);
			if (Math.Abs(num - x) < 8)
			{
				return HitLocation.RangeBegin;
			}
			float num2 = XToTime(x);
			if (num2 >= RangeStart && num2 < RangeStart + RangeDuration)
			{
				return HitLocation.Range;
			}
			return HitLocation.Nothing;
		}

		private bool HitShuttle(int x, int y)
		{
			return GetHitLocation(x, y) == HitLocation.Shuttle;
		}

		private void TimeRulerControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
			{
				drag = DragMode.Pan;
				drag_pt.X = e.X;
				base.Capture = true;
				Cursor = CustomCursors.Get(CustomCursor.HandDrag);
			}
			if (e.Button == MouseButtons.Left)
			{
				switch (GetHitLocation(e.X, e.Y))
				{
				case HitLocation.Shuttle:
					drag = DragMode.Shuttle;
					base.Capture = true;
					break;
				case HitLocation.RangeBegin:
					drag = DragMode.RangeLeft;
					drag_pt.X = e.X;
					base.Capture = true;
					break;
				case HitLocation.RangeEnd:
					drag = DragMode.RangeRight;
					drag_pt.X = e.X;
					base.Capture = true;
					break;
				}
			}
		}

		private void NotifyUserTimeChanged(float time)
		{
			TimeRuleEventHandler userTimeChanged = this.UserTimeChanged;
			if (userTimeChanged != null)
			{
				userTimeChanged(this, new TimeRulerEventArgs(time));
			}
		}

		private void TimeRulerControl_MouseMove(object sender, MouseEventArgs e)
		{
			switch (drag)
			{
			case DragMode.Pan:
				Origin = Math.Max(0f, Origin - (float)(e.X - drag_pt.X) / MajorScale);
				drag_pt.X = e.X;
				break;
			case DragMode.Shuttle:
			{
				float num3 = Math.Max(0f, XToTime(e.X));
				NotifyUserTimeChanged(num3);
				CurrentTime = num3;
				Update();
				break;
			}
			case DragMode.RangeLeft:
			{
				float num4 = range_begin + range_duration;
				float num5 = (float)(e.X - drag_pt.X) / MajorScale;
				range_begin += num5;
				range_duration -= num5;
				if (range_begin < 0f)
				{
					range_duration = num4;
					range_begin = 0f;
				}
				if (range_duration < 0f)
				{
					float num6 = range_begin;
					range_begin += range_duration;
					range_duration = num6 - range_begin;
					drag = DragMode.RangeRight;
					Cursor = CustomCursors.Get(CustomCursor.RightExtend);
				}
				OnRangeChanged(EventArgs.Empty);
				drag_pt.X = e.X;
				Invalidate();
				break;
			}
			case DragMode.RangeRight:
			{
				float num = (float)(e.X - drag_pt.X) / MajorScale;
				range_duration += num;
				if (range_duration < 0f)
				{
					float num2 = range_begin;
					range_begin += range_duration;
					range_duration = num2 - range_begin;
					drag = DragMode.RangeLeft;
					Cursor = CustomCursors.Get(CustomCursor.LeftExtend);
				}
				OnRangeChanged(EventArgs.Empty);
				drag_pt.X = e.X;
				Invalidate();
				break;
			}
			case DragMode.Idle:
				switch (GetHitLocation(e.X, e.Y))
				{
				case HitLocation.Shuttle:
					Cursor = CustomCursors.Get(CustomCursor.FingerPoint);
					break;
				case HitLocation.RangeBegin:
					Cursor = CustomCursors.Get(CustomCursor.LeftExtend);
					break;
				case HitLocation.RangeEnd:
					Cursor = CustomCursors.Get(CustomCursor.RightExtend);
					break;
				case HitLocation.Range:
					Cursor = CustomCursors.Get(CustomCursor.HandDrag);
					break;
				default:
					Cursor = Cursors.Default;
					break;
				}
				break;
			}
		}

		private void TimeRulerControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				switch (drag)
				{
				case DragMode.Shuttle:
					EnsureVisible();
					drag = DragMode.Idle;
					base.Capture = false;
					Cursor = Cursors.Default;
					Invalidate();
					break;
				case DragMode.RangeLeft:
				case DragMode.RangeRight:
					drag = DragMode.Idle;
					base.Capture = false;
					Cursor = Cursors.Default;
					break;
				}
			}
			else if (e.Button == MouseButtons.Middle && drag == DragMode.Pan)
			{
				drag = DragMode.Idle;
				base.Capture = false;
				Cursor = Cursors.Default;
			}
		}

		private void TimeRulerControl_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				float num = Math.Max(0f, XToTime(e.X));
				NotifyUserTimeChanged(num);
				CurrentTime = num;
				EnsureVisible();
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
			this.DoubleBuffered = true;
			base.Name = "TimeRulerControl";
			this.ForeColor = System.Drawing.Color.Black;
			this.BackColor = System.Drawing.Color.FromArgb(232, 232, 232);
			base.Size = new System.Drawing.Size(412, 55);
			base.Paint += new System.Windows.Forms.PaintEventHandler(TimeRulerControl_Paint);
			base.MouseMove += new System.Windows.Forms.MouseEventHandler(TimeRulerControl_MouseMove);
			base.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(TimeRulerControl_MouseDoubleClick);
			base.MouseDown += new System.Windows.Forms.MouseEventHandler(TimeRulerControl_MouseDown);
			base.MouseUp += new System.Windows.Forms.MouseEventHandler(TimeRulerControl_MouseUp);
			base.ResumeLayout(false);
		}
	}
}
