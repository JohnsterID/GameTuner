using System;
using System.Drawing;

namespace GameTuner.Framework
{
	public class ImageProviderAttribute : Attribute
	{
		private Type Location { get; set; }

		private string Name { get; set; }

		public ImageProviderAttribute(Type location, string name)
		{
			Location = location;
			Name = name;
		}

		public Image GetImage()
		{
			return ImageHelper.GetResourceImage(Location, Name);
		}
	}
}
