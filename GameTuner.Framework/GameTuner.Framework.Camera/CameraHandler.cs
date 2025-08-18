using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner.Framework.Camera
{
	public class CameraHandler
	{
		private Cursor OutsideCursor;

		private bool bAlt;

		public float WheelSensitivity = 0.3f;

		private Point MouseDownLocation;

		private bool IsCaptured;

		private Matrix4x4 MouseDownMatrix;

		private Quaternion MouseDownQuat;

		private CameraUtility.ArcBall ArcBall = new CameraUtility.ArcBall();

		public bool InvertY = true;

		public Control Control { get; private set; }

		public CameraState State { get; private set; }

		public CameraHandler(Control kInputControl, CameraState kState)
		{
			Control = kInputControl;
			State = kState;
			Control.MouseDown += Control_MouseDown;
			Control.MouseMove += Control_MouseMove;
			Control.MouseWheel += Control_MouseWheel;
			Control.Resize += Control_Resize;
			Control.KeyDown += Control_KeyDown;
			Control.KeyUp += Control_KeyUp;
			Control.MouseUp += Control_MouseUp;
			Control.MouseEnter += Control_MouseEnter;
			Control.MouseLeave += Control_MouseLeave;
			State.AspectRatio = (float)Control.Size.Width / (float)Control.Size.Height;
			ArcBall.Width = Control.Size.Width;
			ArcBall.Height = Control.Size.Height;
		}

		private void Control_MouseLeave(object sender, EventArgs e)
		{
			Cursor.Current = OutsideCursor;
		}

		private void Control_MouseEnter(object sender, EventArgs e)
		{
			OutsideCursor = Cursor.Current;
			Cursor.Current = Cursors.Hand;
		}

		private void Control_KeyUp(object sender, KeyEventArgs e)
		{
			bAlt = e.Alt;
		}

		private void Control_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
			case Keys.F:
			case Keys.Z:
				State.Reset(CameraState.ResetType.Front);
				break;
			case Keys.T:
				State.Reset(CameraState.ResetType.Top);
				break;
			case Keys.B:
				if (e.Shift)
				{
					State.Reset(CameraState.ResetType.Back);
				}
				else
				{
					State.Reset(CameraState.ResetType.Bottom);
				}
				break;
			case Keys.L:
				if (e.Shift)
				{
					State.Reset(CameraState.ResetType.Right);
				}
				else
				{
					State.Reset(CameraState.ResetType.Left);
				}
				break;
			}
			bAlt = e.Alt;
		}

		private void Control_Resize(object sender, EventArgs e)
		{
			State.AspectRatio = (float)Control.Size.Width / (float)Control.Size.Height;
			ArcBall.Width = Control.Size.Width;
			ArcBall.Height = Control.Size.Height;
		}

		private void Control_MouseWheel(object sender, MouseEventArgs e)
		{
			CameraUtility.Zoom(State, WheelSensitivity * (float)Convert.ToDouble(e.Delta));
		}

		private void Control_MouseUp(object sender, MouseEventArgs e)
		{
			IsCaptured = false;
		}

		private void Control_MouseDown(object sender, MouseEventArgs e)
		{
			IsCaptured = true;
			MouseDownLocation = e.Location;
			ArcBall.Click(new Point((int)ArcBall.Width - e.Location.X, e.Location.Y));
			MouseDownMatrix = State.ViewMatrix;
			MouseDownQuat = State.Orientation;
		}

		private void Control_MouseMove(object sender, MouseEventArgs e)
		{
			if (!IsCaptured)
			{
				return;
			}
			if (!bAlt)
			{
				Point point = new Point(e.Location.X - MouseDownLocation.X, e.Location.Y - MouseDownLocation.Y);
				if (InvertY)
				{
					point.Y = -point.Y;
				}
				MouseDownLocation = e.Location;
				CameraUtility.Pan(State, point.X, point.Y);
			}
			else
			{
				Quaternion kNewOrientation;
				ArcBall.Drag(new Point((int)ArcBall.Width - e.Location.X, e.Location.Y), out kNewOrientation);
				Matrix4x4 matrix4x = kNewOrientation.ToMatrix4x4();
				Matrix4x4 viewMatrix = matrix4x * MouseDownMatrix;
				State.ViewMatrix = viewMatrix;
			}
		}

		~CameraHandler()
		{
			Control.MouseDown -= Control_MouseDown;
			Control.MouseMove -= Control_MouseMove;
			Control.MouseWheel -= Control_MouseWheel;
			Control.Resize -= Control_Resize;
			Control.KeyDown -= Control_KeyDown;
			Control.KeyUp -= Control_KeyUp;
			Control.MouseUp -= Control_MouseUp;
		}
	}
}
