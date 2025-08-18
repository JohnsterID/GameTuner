using System;

namespace GameTuner.Framework.Graph
{
	public class NubTypeAttribute : Attribute
	{
		public NubType Type { get; private set; }

		public NubTypeAttribute(NubType type)
		{
			Type = type;
		}
	}
}
