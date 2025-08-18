using System.Drawing;

namespace GameTuner.Framework
{
	public static class ColorExtension
	{
		private static Vec3 Luminance = new Vec3(0.3f, 0.59f, 0.11f);

		public static Color Desaturate(this Color color)
		{
			return color.Desaturate(0f);
		}

		public static Color Desaturate(this Color color, float s)
		{
			Vec3 vec = new Vec3((float)(int)color.R / 255f, (float)(int)color.G / 255f, (float)(int)color.B / 255f);
			float num = Luminance.Dot(vec);
			Vec3 v = new Vec3(num, num, num);
			Vec3 vec2 = Vec3.Lerp(v, vec, s);
			return Color.FromArgb(color.A, (int)(vec2.X * 255f), (int)(vec2.Y * 255f), (int)(vec2.Z * 255f));
		}
	}
}
