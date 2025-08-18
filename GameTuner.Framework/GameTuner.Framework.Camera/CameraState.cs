using System;

namespace GameTuner.Framework.Camera
{
	public class CameraState
	{
		public enum Handedness
		{
			LeftHanded,
			RightHanded
		}

		public enum ResetType
		{
			Front,
			Back,
			Top,
			Bottom,
			Left,
			Right
		}

		private Quaternion m_kOrientation = Quaternion.Identity;

		private Vec3 m_kPosition = Vec3.Zero;

		private float m_fFOV = (float)Math.PI / 4f;

		private float m_fAR = 1f;

		private float m_fNear = 1f;

		private float m_fFar = 1000f;

		private Handedness m_eHandedness = Handedness.RightHanded;

		private Vec3 m_kBoundMin = new Vec3(-1f, -1f, -1f);

		private bool m_bBoundMinSet;

		private Vec3 m_kBoundMax = new Vec3(1f, 1f, 1f);

		private bool m_bBoundMaxSet;

		private Vec3 m_kBoundCenter = default(Vec3);

		private bool m_bBoundCenterSet;

		private bool m_bResetPending = true;

		private ResetType m_eResetType;

		private Matrix4x4 m_kProjMatrix = new Matrix4x4();

		private Matrix4x4 m_kViewMatrix = new Matrix4x4();

		public Matrix4x4 ProjMatrix
		{
			get
			{
				return m_kProjMatrix;
			}
			set
			{
				m_kProjMatrix = value;
				if (this.CameraUpdate != null)
				{
					this.CameraUpdate(this, EventArgs.Empty);
				}
			}
		}

		public Matrix4x4 ViewMatrix
		{
			get
			{
				return m_kViewMatrix;
			}
			set
			{
				m_kViewMatrix = value;
				m_kOrientation = Quaternion.FromRotationMatrix(m_kViewMatrix);
				Matrix4x4 matrix4x = new Matrix4x4(m_kViewMatrix);
				Vec3 vec = default(Vec3);
				for (int i = 0; i < 3; i++)
				{
					matrix4x[3, i] = 0f;
					vec[i] = m_kViewMatrix[3, i];
				}
				matrix4x.Transpose();
				Matrix4x4 matrix4x2 = m_kViewMatrix * matrix4x;
				m_kPosition = new Vec3(0f - matrix4x2[3, 0], 0f - matrix4x2[3, 1], 0f - matrix4x2[3, 2]);
				if (this.CameraUpdate != null)
				{
					this.CameraUpdate(this, EventArgs.Empty);
				}
			}
		}

		public Quaternion Orientation
		{
			get
			{
				return m_kOrientation;
			}
			set
			{
				m_kOrientation = value;
				RecalculateViewMatrix();
			}
		}

		public Vec3 Position
		{
			get
			{
				return m_kPosition;
			}
			set
			{
				m_kPosition = value;
				RecalculateViewMatrix();
			}
		}

		public float FOV
		{
			get
			{
				return m_fFOV;
			}
			set
			{
				m_fFOV = value;
				RecalculateProjMatrix();
			}
		}

		public float AspectRatio
		{
			get
			{
				return m_fAR;
			}
			set
			{
				m_fAR = value;
				RecalculateProjMatrix();
			}
		}

		public float NearPlane
		{
			get
			{
				return m_fNear;
			}
			set
			{
				m_fNear = value;
				RecalculateProjMatrix();
			}
		}

		public float FarPlane
		{
			get
			{
				return m_fFar;
			}
			set
			{
				m_fFar = value;
				RecalculateProjMatrix();
			}
		}

		public bool AutoFarPlane { get; set; }

		public Handedness ProjectionHandedness
		{
			get
			{
				return m_eHandedness;
			}
			set
			{
				m_eHandedness = value;
				RecalculateProjMatrix();
			}
		}

		public Vec3 BoundMinimum
		{
			get
			{
				return m_kBoundMin;
			}
			set
			{
				m_kBoundMin = value;
				m_bBoundMinSet = true;
				ResetOnInitialBounds();
			}
		}

		public Vec3 BoundMaximum
		{
			get
			{
				return m_kBoundMax;
			}
			set
			{
				m_kBoundMax = value;
				m_bBoundMaxSet = true;
				ResetOnInitialBounds();
			}
		}

		public Vec3 BoundCenter
		{
			get
			{
				return m_kBoundCenter;
			}
			set
			{
				m_kBoundCenter = value;
				m_bBoundCenterSet = true;
				ResetOnInitialBounds();
			}
		}

		public event EventHandler CameraUpdate;

		public CameraState()
		{
			AutoFarPlane = true;
			RecalculateProjMatrix();
			RecalculateViewMatrix();
		}

		private void ResetOnInitialBounds()
		{
			if (m_bResetPending && m_bBoundMinSet && m_bBoundMaxSet && m_bBoundCenterSet)
			{
				Reset(m_eResetType);
				m_bResetPending = false;
			}
		}

		public void Reset(ResetType eType)
		{
			switch (eType)
			{
			case ResetType.Front:
				m_kPosition = new Vec3(BoundCenter.X, BoundCenter.Y - (BoundMaximum - BoundMinimum).Length, BoundCenter.Z);
				m_kOrientation = new Quaternion(1f, 0f, 0f, 1f);
				break;
			case ResetType.Back:
				m_kPosition = new Vec3(BoundCenter.X, BoundCenter.Y + (BoundMaximum - BoundMinimum).Length, BoundCenter.Z);
				m_kOrientation = new Quaternion(0f, 1f, 1f, 0f);
				break;
			case ResetType.Top:
				m_kPosition = new Vec3(BoundCenter.X, BoundCenter.Y, BoundCenter.Z + (BoundMaximum - BoundMinimum).Length);
				m_kOrientation = new Quaternion(0f, 0f, 0f, 1f);
				break;
			case ResetType.Bottom:
				m_kPosition = new Vec3(BoundCenter.X, BoundCenter.Y, BoundCenter.Z - (BoundMaximum - BoundMinimum).Length);
				m_kOrientation = new Quaternion(1f, 0f, 0f, 0f);
				break;
			case ResetType.Left:
				m_kPosition = new Vec3(BoundCenter.X - (BoundMaximum - BoundMinimum).Length, BoundCenter.Y, BoundCenter.Z);
				m_kPosition = new Vec3(0f, 0f, 0f);
				m_kOrientation = new Quaternion(1f, -1f, -1f, -1f);
				break;
			case ResetType.Right:
				m_kPosition = new Vec3(BoundCenter.X + (BoundMaximum - BoundMinimum).Length, BoundCenter.Y, BoundCenter.Z);
				m_kOrientation = new Quaternion(-1f, 1f, 1f, 1f);
				break;
			}
			RecalculateViewMatrix();
		}

		private void RecalculateProjMatrix()
		{
			float num = 1f / (float)Math.Tan(FOV / 2f);
			float value = num / AspectRatio;
			switch (ProjectionHandedness)
			{
			case Handedness.LeftHanded:
				m_kProjMatrix[0, 0] = value;
				m_kProjMatrix[1, 1] = num;
				m_kProjMatrix[2, 2] = FarPlane / (FarPlane - NearPlane);
				m_kProjMatrix[2, 3] = 1f;
				m_kProjMatrix[3, 2] = (0f - NearPlane * FarPlane) / (FarPlane - NearPlane);
				break;
			case Handedness.RightHanded:
				m_kProjMatrix[0, 0] = value;
				m_kProjMatrix[1, 1] = num;
				m_kProjMatrix[2, 2] = FarPlane / (NearPlane - FarPlane);
				m_kProjMatrix[2, 3] = -1f;
				m_kProjMatrix[3, 2] = NearPlane * FarPlane / (NearPlane - FarPlane);
				break;
			}
			if (this.CameraUpdate != null)
			{
				this.CameraUpdate(this, EventArgs.Empty);
			}
		}

		private void RecalculateViewMatrix()
		{
			Matrix4x4 matrix4x = Orientation.ToMatrix4x4();
			Matrix4x4 identity = Matrix4x4.Identity;
			identity[3, 0] = 0f - Position.X;
			identity[3, 1] = 0f - Position.Y;
			identity[3, 2] = 0f - Position.Z;
			m_kViewMatrix = identity * matrix4x;
			if (AutoFarPlane)
			{
				float num = (BoundMaximum - BoundMinimum).Length / 2f;
				float length = (Position - BoundCenter).Length;
				FarPlane = (num + length) * 1.05f;
			}
			if (this.CameraUpdate != null)
			{
				this.CameraUpdate(this, EventArgs.Empty);
			}
		}
	}
}
