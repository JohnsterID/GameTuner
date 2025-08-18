using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using GameTuner.Framework.Resources;

namespace GameTuner.Framework
{
	public static class ImageHelper
	{
		public static Image LoadImage(Stream stream)
		{
			if (stream != null)
			{
				Image image = Image.FromStream(stream);
				Image image2 = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
				using (Graphics graphics = Graphics.FromImage(image2))
				{
					graphics.DrawImage(image, 0, 0, image.Width, image.Height);
				}
				image.Dispose();
				return image2;
			}
			return null;
		}

		public static Image CreateSolidImage(Color color1, Color color2, int width, int height)
		{
			Image image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
			using (Graphics graphics = Graphics.FromImage(image))
			{
				using (Brush brush = new SolidBrush(color1))
				{
					graphics.FillRectangle(brush, 0, 0, width / 2, height);
				}
				using (Brush brush2 = new SolidBrush(color2))
				{
					graphics.FillRectangle(brush2, width / 2, 0, width / 2, height);
					return image;
				}
			}
		}

		public static Image CreateSolidImage(Color color, int width, int height)
		{
			Image image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
			using (Graphics graphics = Graphics.FromImage(image))
			{
				using (Brush brush = new SolidBrush(color))
				{
					graphics.FillRectangle(brush, 0, 0, width, height);
					return image;
				}
			}
		}

		public static Image GetResourceImage(Type type, string name)
		{
			Assembly assembly = type.Assembly;
			Stream manifestResourceStream = assembly.GetManifestResourceStream(type, name);
			if (manifestResourceStream == null)
			{
				string text = type.Namespace;
				text = text + "." + name;
				manifestResourceStream = assembly.GetManifestResourceStream(text);
			}
			return LoadImage(manifestResourceStream);
		}

		public static Image GetResourceImage(string name)
		{
			return GetResourceImage(typeof(ResourceTag), name);
		}
	}
}
