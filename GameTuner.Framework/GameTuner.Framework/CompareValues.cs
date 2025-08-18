using System;

namespace GameTuner.Framework
{
	public static class CompareValues
	{
		private static readonly string[] symbols = new string[7] { "", "=", "<", ">", "<=", ">=", "!=" };

		public static string GetMethodSymbol(CompareMethod method)
		{
			return symbols[(int)method];
		}

		public static bool Compare<T>(T a, T b, CompareMethod method) where T : IComparable<T>
		{
			int num = a.CompareTo(b);
			switch (method)
			{
			case CompareMethod.Equal:
				return num == 0;
			case CompareMethod.Less:
				return num < 0;
			case CompareMethod.Greater:
				return num > 0;
			case CompareMethod.LessEqual:
				return num <= 0;
			case CompareMethod.GreaterEqual:
				return num >= 0;
			case CompareMethod.NotEqual:
				return num != 0;
			default:
				return false;
			}
		}
	}
}
