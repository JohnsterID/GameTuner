namespace GameTuner.Framework
{
	public class Matrix4x4
	{
		public static Matrix4x4 Identity;

		public static Matrix4x4 Zero;

		public float[,] Data { get; set; }

		public float this[int x, int y]
		{
			get
			{
				return Data[x, y];
			}
			set
			{
				Data[x, y] = value;
			}
		}

		public float this[int i]
		{
			get
			{
				return Data[i / 4, i % 4];
			}
			set
			{
				Data[i / 4, i % 4] = value;
			}
		}

		public Matrix4x4()
		{
			Data = new float[4, 4];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					Data[i, j] = 0f;
				}
			}
		}

		public Matrix4x4(float[,] afInit)
		{
			Data = new float[4, 4];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					Data[i, j] = afInit[i, j];
				}
			}
		}

		public Matrix4x4(Matrix4x4 kCopy)
		{
			Data = new float[4, 4];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					Data[i, j] = kCopy[i, j];
				}
			}
		}

		public Matrix4x4(Vec3 kTranslation, Quaternion kRotation)
		{
			Matrix4x4 identity = Identity;
			Matrix4x4 matrix4x = kRotation.ToMatrix4x4();
			for (int i = 0; i < 3; i++)
			{
				identity[3, i] = 0f - kTranslation[i];
			}
			Matrix4x4 matrix4x2 = identity * matrix4x;
			Data = matrix4x2.Data;
		}

		public static Matrix4x4 operator *(Matrix4x4 m1, Matrix4x4 m2)
		{
			float[,] array = new float[4, 4];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					array[i, j] = 0f;
					for (int k = 0; k < 4; k++)
					{
						array[i, j] += m1.Data[i, k] * m2.Data[k, j];
					}
				}
			}
			return new Matrix4x4(array);
		}

		public static Vec3 operator *(Vec3 v1, Matrix4x4 m1)
		{
			Vec3 result = default(Vec3);
			for (int i = 0; i < 4; i++)
			{
				result[i] = 0f;
				for (int j = 0; j < 4; j++)
				{
					result[i] += v1[j] * m1.Data[j, i];
				}
			}
			return result;
		}

		public void Transpose()
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = i + 1; j < 4; j++)
				{
					float num = Data[i, j];
					Data[i, j] = Data[j, i];
					Data[j, i] = num;
				}
			}
		}

		static Matrix4x4()
		{
			Zero = new Matrix4x4();
			Identity = new Matrix4x4();
			for (int i = 0; i < 4; i++)
			{
				Identity.Data[i, i] = 1f;
			}
		}
	}
}
