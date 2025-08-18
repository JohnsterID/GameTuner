using System;

namespace GameTuner.Framework
{
	public static class MathEx
	{
		public const float PI = (float)Math.PI;

		public const float PI_MUL_2 = (float)Math.PI * 2f;

		public const float PI_DIV_2 = (float)Math.PI / 2f;

		public const float PI_DIV_4 = (float)Math.PI / 4f;

		public const float PI_INV = 1f / (float)Math.PI;

		private static Random rand = new Random();

		public static void RandSeed()
		{
			rand = new Random((int)DateTime.Now.Ticks);
		}

		public static void RandSeed(int seed)
		{
			rand = new Random(seed);
		}

		public static float Randf()
		{
			return (float)rand.NextDouble();
		}

		public static int Rand()
		{
			return rand.Next();
		}

		public static float Semitone(int semitone)
		{
			return (float)Math.Pow(2.0, (double)semitone / 12.0);
		}

		public static int Rand(int minValue, int maxValue)
		{
			return rand.Next(minValue, maxValue);
		}

		public static float Deg2Rad(float deg)
		{
			return deg * ((float)Math.PI / 180f);
		}

		public static float Rad2Deg(float rad)
		{
			return rad * (180f / (float)Math.PI);
		}

		public static int HighPow2(int pow)
		{
			pow--;
			for (int num = 1; num < 32; num <<= 1)
			{
				pow |= pow >> num;
			}
			return pow + 1;
		}

		public static int Lerp(int v0, int v1, float t)
		{
			return (int)((float)v0 + (float)(v1 - v0) * t);
		}

		public static float Lerp(float v0, float v1, float t)
		{
			return v0 + (v1 - v0) * t;
		}

		public static void SinCos(float rad, out float s, out float c)
		{
			s = (float)Math.Sin(rad);
			c = (float)Math.Cos(rad);
		}

		public static float N(float value)
		{
			return Clamp(value, 0f, 1f);
		}

		public static float Clamp(float value, float min, float max)
		{
			return Math.Min(Math.Max(value, min), max);
		}

		public static int ToFourCC(string fourCC)
		{
			if (string.IsNullOrEmpty(fourCC) || fourCC.Length != 4)
			{
				throw new ArgumentException("fourCC");
			}
			return (int)(fourCC[0] | ((uint)fourCC[1] << 8) | ((uint)fourCC[2] << 16) | ((uint)fourCC[3] << 24));
		}

		public static bool IsFourCC(int aFourCC, string compareTo)
		{
			if (string.IsNullOrEmpty(compareTo))
			{
				return false;
			}
			if (compareTo.Length != 4)
			{
				return false;
			}
			byte b = (byte)aFourCC;
			byte b2 = (byte)(aFourCC >> 8);
			byte b3 = (byte)(aFourCC >> 16);
			byte b4 = (byte)(aFourCC >> 24);
			if (b == compareTo[0] && b2 == compareTo[1] && b3 == compareTo[2])
			{
				return b4 == compareTo[3];
			}
			return false;
		}
	}
}
