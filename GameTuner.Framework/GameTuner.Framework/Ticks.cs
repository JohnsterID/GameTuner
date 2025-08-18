using System;
using System.Diagnostics;

namespace GameTuner.Framework
{
	public static class Ticks
	{
		public static long Count
		{
			get
			{
				return Stopwatch.GetTimestamp();
			}
		}

		public static long Frequency
		{
			get
			{
				return Stopwatch.Frequency;
			}
		}

		public static float SecondsCount
		{
			get
			{
				return (float)CountToTimeSpan(Count).TotalSeconds;
			}
		}

		public static TimeSpan CountToTimeSpan(long value)
		{
			long value2 = value * 10000000 / Frequency;
			return TimeSpan.FromTicks(value2);
		}
	}
}
