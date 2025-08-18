using System;
using System.Drawing;

namespace GameTuner.Framework.Camera
{
	public class CameraUtility
	{
		public class ArcBall
		{
			private Vec3 StartVector;

			private Vec3 EndVector;

			public float Width { get; set; }

			public float Height { get; set; }

			private float AdjustWidth
			{
				get
				{
					return 1f / ((Width - 1f) * 0.5f);
				}
			}

			private float AdjustHeight
			{
				get
				{
					return 1f / ((Height - 1f) * 0.5f);
				}
			}

			public void Click(Point kClickPosition)
			{
				MapToSphere(kClickPosition, out StartVector);
			}

			public void Drag(Point kDragPosition, out Quaternion kNewOrientation)
			{
				MapToSphere(kDragPosition, out EndVector);
				Vec3 vec = StartVector.Cross(EndVector);
				if (vec.LengthSquared > 0.001f)
				{
					kNewOrientation = new Quaternion(vec.X, vec.Z, vec.Y, StartVector.Dot(EndVector));
				}
				else
				{
					kNewOrientation = Quaternion.Identity;
				}
			}

			private void MapToSphere(Point kNewPoint, out Vec3 kNewVector)
			{
				float num = (float)kNewPoint.X * AdjustWidth - 1f;
				float num2 = (float)kNewPoint.Y * AdjustHeight - 1f;
				float num3 = num * num + num2 * num2;
				if (num3 > 1f)
				{
					float num4 = 1f / (float)Math.Sqrt(num3);
					kNewVector = new Vec3(num * num4, num2 * num4, 0f);
				}
				else
				{
					kNewVector = new Vec3(num, num2, (float)Math.Sqrt(1f - num3));
				}
			}
		}

		public static void Pan(CameraState kState, float fDX, float fDY)
		{
			Vec3 vec = new Vec3(kState.ViewMatrix[0, 0], kState.ViewMatrix[1, 0], kState.ViewMatrix[2, 0]);
			Vec3 vec2 = new Vec3(kState.ViewMatrix[0, 1], kState.ViewMatrix[1, 1], kState.ViewMatrix[2, 1]);
			kState.Position += vec * fDX - vec2 * fDY;
		}

		public static void Zoom(CameraState kState, float fDelta)
		{
			Vec3 vec = new Vec3(kState.ViewMatrix[0, 2], kState.ViewMatrix[1, 2], kState.ViewMatrix[2, 2]);
			kState.Position -= vec * fDelta;
		}
	}
}
