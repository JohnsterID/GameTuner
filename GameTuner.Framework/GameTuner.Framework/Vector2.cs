using System;

namespace GameTuner.Framework
{
	public struct Vector2
	{
		public float X;

		public float Y;

		private static Vector2 zero = default(Vector2);

		private static Vector2 one = new Vector2(1f, 1f);

		public static Vector2 Zero
		{
			get
			{
				return zero;
			}
		}

		public static Vector2 One
		{
			get
			{
				return one;
			}
		}

		public float LengthSquared
		{
			get
			{
				return X * X + Y * Y;
			}
		}

		public float Length
		{
			get
			{
				return (float)Math.Sqrt(X * X + Y * Y);
			}
		}

		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		public void Set(float x, float y)
		{
			X = x;
			Y = y;
		}

		public override bool Equals(object obj)
		{
			if (obj is Vector2)
			{
				return Equals((Vector2)obj);
			}
			return false;
		}

		public bool Equals(Vector2 other)
		{
			if (X == other.X)
			{
				return Y == other.Y;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode();
		}

		public static Vector2 operator -(Vector2 value)
		{
			return new Vector2(0f - value.X, 0f - value.Y);
		}

		public static Vector2 operator -(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X - value2.X, value1.Y - value2.Y);
		}

		public static bool operator !=(Vector2 value1, Vector2 value2)
		{
			if (value1.X == value2.X)
			{
				return value1.Y != value2.Y;
			}
			return true;
		}

		public static Vector2 operator *(float scaleFactor, Vector2 value)
		{
			return new Vector2(value.X * scaleFactor, value.Y * scaleFactor);
		}

		public static Vector2 operator *(Vector2 value, float scaleFactor)
		{
			return new Vector2(value.X * scaleFactor, value.Y * scaleFactor);
		}

		public static Vector2 operator *(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X * value2.X, value1.Y * value2.Y);
		}

		public static Vector2 operator /(Vector2 value, float divider)
		{
			float num = 1f / divider;
			Vector2 result = default(Vector2);
			result.X = value.X * num;
			result.Y = value.Y * num;
			return result;
		}

		public static Vector2 operator /(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X / value2.X, value1.Y / value2.Y);
		}

		public static Vector2 operator +(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X + value2.X, value1.Y + value2.Y);
		}

		public static bool operator ==(Vector2 value1, Vector2 value2)
		{
			if (value1.X == value2.X)
			{
				return value1.Y == value2.Y;
			}
			return false;
		}

		public static Vector2 Rotate(float rad, Vector2 value)
		{
			float num = (float)Math.Sin(rad);
			float num2 = (float)Math.Cos(rad);
			Vector2 result = default(Vector2);
			result.X = num2 * value.X - num * value.Y;
			result.Y = num * value.X + num2 * value.Y;
			return result;
		}

		public static Vector2 Lerp(Vector2 v0, Vector2 v1, float t)
		{
			return v0 + (v1 - v0) * t;
		}

		public float DotProduct(Vector2 v)
		{
			return X * v.X + Y * v.Y;
		}

		public static Vector2 Max(Vector2 a, Vector2 b)
		{
			return new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
		}

		public static Vector2 Min(Vector2 a, Vector2 b)
		{
			return new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
		}
	}
}
