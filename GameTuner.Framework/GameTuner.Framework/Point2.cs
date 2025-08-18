using System.Drawing;

namespace GameTuner.Framework
{
	public struct Point2
	{
		public int X;

		public int Y;

		private static Point2 empty = default(Point2);

		public static Point2 Empty
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
				return new Point(X, Y);
			}
		}

		public Vec2 ToVec2
		{
			get
			{
				return new Vec2(X, Y);
			}
		}

		public Point2(int x, int y)
		{
			X = x;
			Y = y;
		}

		public Point2(Point point)
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
			if (obj is Point2)
			{
				return Equals((Point2)obj);
			}
			return false;
		}

		public bool Equals(Point2 other)
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

		public static Point2 operator -(Point2 value)
		{
			return new Point2(-value.X, -value.Y);
		}

		public static Point2 operator -(Point2 value1, Point2 value2)
		{
			return new Point2(value1.X - value2.X, value1.Y - value2.Y);
		}

		public static bool operator !=(Point2 value1, Point2 value2)
		{
			if (value1.X == value2.X)
			{
				return value1.Y != value2.Y;
			}
			return true;
		}

		public static Point2 operator *(int scaleFactor, Point2 value)
		{
			return new Point2(value.X * scaleFactor, value.Y * scaleFactor);
		}

		public static Point2 operator *(Point2 value, int scaleFactor)
		{
			return new Point2(value.X * scaleFactor, value.Y * scaleFactor);
		}

		public static Point2 operator *(Point2 value1, Point2 value2)
		{
			return new Point2(value1.X * value2.X, value1.Y * value2.Y);
		}

		public static Point2 operator /(Point2 value, int divider)
		{
			float num = 1f / (float)divider;
			Point2 result = default(Point2);
			result.X = (int)((float)value.X * num);
			result.Y = (int)((float)value.Y * num);
			return result;
		}

		public static Point2 operator /(Point2 value1, Point2 value2)
		{
			return new Point2(value1.X / value2.X, value1.Y / value2.Y);
		}

		public static Point2 operator +(Point2 value1, Point2 value2)
		{
			return new Point2(value1.X + value2.X, value1.Y + value2.Y);
		}

		public static bool operator ==(Point2 value1, Point2 value2)
		{
			if (value1.X == value2.X)
			{
				return value1.Y == value2.Y;
			}
			return false;
		}
	}
}
