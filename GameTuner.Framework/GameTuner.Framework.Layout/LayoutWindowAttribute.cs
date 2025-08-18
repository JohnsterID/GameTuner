using System;

namespace GameTuner.Framework.Layout
{
	public class LayoutWindowAttribute : Attribute
	{
		public string Name { get; private set; }

		public LayoutWindowAttribute(string name)
		{
			Name = name;
		}
	}
}
