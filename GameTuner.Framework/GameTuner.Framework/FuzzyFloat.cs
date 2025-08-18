using System;

namespace GameTuner.Framework
{
	public struct FuzzyFloat : IEquatable<FuzzyFloat>
	{
		public float Min;

		public float Max;

		public static readonly FuzzyFloat Zero;

		public float Value
		{
			get
			{
				return MathEx.Randf() * Range + Min;
			}
		}

		public float Range
		{
			get
			{
				return Max - Min;
			}
		}

		public float Center
		{
			get
			{
				return (Max + Min) * 0.5f;
			}
		}

		public FuzzyFloat(float min, float max)
		{
			Min = min;
			Max = max;
		}

		static FuzzyFloat()
		{
			Zero = default(FuzzyFloat);
		}

		public override string ToString()
		{
			return string.Format("{0} .. {1}", Min, Max);
		}

		public override int GetHashCode()
		{
			return Min.GetHashCode() + Max.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is FuzzyFloat)
			{
				return Equals((FuzzyFloat)obj);
			}
			return base.Equals(obj);
		}

		public bool Equals(FuzzyFloat other)
		{
			if (Min == other.Min)
			{
				return Max == other.Max;
			}
			return false;
		}

		public static bool operator !=(FuzzyFloat a, FuzzyFloat b)
		{
			return !a.Equals(b);
		}

		public static bool operator ==(FuzzyFloat a, FuzzyFloat b)
		{
			return a.Equals(b);
		}

		public static implicit operator float(FuzzyFloat v)
		{
			return v.Value;
		}

		public static implicit operator FuzzyFloat(float v)
		{
			FuzzyFloat result = default(FuzzyFloat);
			result.Min = v;
			result.Max = v;
			return result;
		}

		public static implicit operator FuzzyFloat(Vec2 v)
		{
			FuzzyFloat result = default(FuzzyFloat);
			result.Min = v.X;
			result.Max = v.Y;
			return result;
		}

		public static bool TryParse(string input, out FuzzyFloat value)
		{
			string[] array = input.Split(new char[2] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 2)
			{
				value = new FuzzyFloat(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]));
				return true;
			}
			if (array.Length == 1)
			{
				value = new FuzzyFloat(Convert.ToSingle(array[0]), Convert.ToSingle(array[0]));
				return true;
			}
			value = Zero;
			return false;
		}
	}
}
