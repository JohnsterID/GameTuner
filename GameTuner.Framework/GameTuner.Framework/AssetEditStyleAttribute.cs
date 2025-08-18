using System;

namespace GameTuner.Framework
{
	public class AssetEditStyleAttribute : Attribute
	{
		public AssetEditStyle EditStyle { get; private set; }

		public AssetEditStyleAttribute(AssetEditStyle editStyle)
		{
			EditStyle = editStyle;
		}
	}
}
