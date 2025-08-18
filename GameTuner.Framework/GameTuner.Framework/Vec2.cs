using System;
using System.ComponentModel;
using System.Drawing;
using GameTuner.Framework.Converters;

namespace GameTuner.Framework
{
	[TypeConverter(typeof(Vec2Converter))]
	public struct Vec2
	{
		public float X;

		public float Y;

		private static Vec2 empty = default(Vec2);

		public static Vec2 Empty
		{
			get
			{
				return empty;
			}
		}

		public Point ToPoint
		{
			get
			{
				return new Point((int)X, (int)Y);
			}
		}

		public PointF ToPointF
		{
			get
			{
				return new PointF(X, Y);
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

		public Vec2 Normalize
		{
			get
			{
				return this / Length;
			}
		}

		public Vec2(float x, float y)
		{
			X = x;
			Y = y;
		}

		public Vec2(Point point)
		{
			X = point.X;
			Y = point.Y;
		}

		public override string ToString()
		{
			return string.Format("{0}, {1}", X, Y);
		}

		public void Set(int x, int y)
		{
			X = x;
			Y = y;
		}

		public void Set(Point point)
		{
			X = point.X;
			Y = point.Y;
		}

		public override bool Equals(object obj)
		{
			if (obj is Vec2)
			{
				return Equals((Vec2)obj);
			}
			return false;
		}

		public bool Equals(Vec2 other)
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

		public static Vec2 operator -(Vec2 value)
		{
			return new Vec2(0f - value.X, 0f - value.Y);
		}

		public static Vec2 operator -(Vec2 value1, Vec2 value2)
		{
			return new Vec2(value1.X - value2.X, value1.Y - value2.Y);
		}

		public static bool operator !=(Vec2 value1, Vec2 value2)
		{
			if (value1.X == value2.X)
			{
				return value1.Y != value2.Y;
			}
			return true;
		}

		public static Vec2 operator *(int scaleFactor, Vec2 value)
		{
			return new Vec2(value.X * (float)scaleFactor, value.Y * (float)scaleFactor);
		}

		public static Vec2 Sum(Point2 value1, Point2 value2)
		{
			return new Vec2(value1.X + value2.X, value1.Y + value2.Y);
		}

		public static Vec2 Sum(Vec2 value1, Vec2 value2)
		{
			return new Vec2(value1.X + value2.X, value1.Y + value2.Y);
		}

		public static Vec2 operator *(Vec2 value, float scaleFactor)
		{
			return new Vec2(value.X * scaleFactor, value.Y * scaleFactor);
		}

		public static Vec2 operator *(Vec2 value1, Vec2 value2)
		{
			return new Vec2(value1.X * value2.X, value1.Y * value2.Y);
		}

		public static Vec2 operator /(Vec2 value, float divider)
		{
			float num = 1f / divider;
			Vec2 result = default(Vec2);
			result.X = value.X * num;
			result.Y = value.Y * num;
			return result;
		}

		public static Vec2 operator /(Vec2 value1, Vec2 value2)
		{
			return new Vec2(value1.X / value2.X, value1.Y / value2.Y);
		}

		public static Vec2 operator +(Vec2 value1, Vec2 value2)
		{
			return new Vec2(value1.X + value2.X, value1.Y + value2.Y);
		}

		public static bool operator ==(Vec2 value1, Vec2 value2)
		{
			if (value1.X == value2.X)
			{
				return value1.Y == value2.Y;
			}
			return false;
		}

		public float Dot(Vec2 v)
		{
			return X * v.X + Y * v.Y;
		}

		public static Vec2 NearestPoint(Vec2 point, Vec2 a, Vec2 b)
		{
			Vec2 vec = point - a;
			Vec2 normalize = (b - a).Normalize;
			return normalize * vec.Dot(normalize) + a;
		}

		public static float FloatPointLine(Vec2 point, Vec2 a, Vec2 b)
		{
			float length = (b - a).Length;
			if (!(length > float.Epsilon))
			{
				return 0f;
			}
			return (point - a).Length / length;
		}

		public static Vec2 Lerp(Vec2 v0, Vec2 v1, float t)
		{
			return v0 + (v1 - v0) * t;
		}

		public static float DistancePointLine(Vec2 point, Vec2 a, Vec2 b)
		{
			float num = Dot(a, b, point);
			if (num > 0f)
			{
				return (b - point).Length;
			}
			num = Dot(b, a, point);
			if (num > 0f)
			{
				return (a - point).Length;
			}
			float value = Cross(a, b, point) / (b - a).Length;
			return Math.Abs(value);
		}

		public static float Cross(Vec2 v1, Vec2 v2, Vec2 v3)
		{
			Vec2 vec = v2 - v1;
			Vec2 vec2 = v3 - v1;
			return vec.X * vec2.Y - vec.Y * vec2.X;
		}

		public static float Dot(Vec2 v1, Vec2 v2, Vec2 v3)
		{
			Vec2 vec = v3 - v2;
			Vec2 v4 = v2 - v1;
			return vec.Dot(v4);
		}
	}
}
