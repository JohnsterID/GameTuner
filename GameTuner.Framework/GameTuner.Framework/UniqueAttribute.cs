using System;

namespace GameTuner.Framework
{
	public class UniqueAttribute : Attribute
	{
		public bool Unique { get; private set; }

		public UniqueAttribute()
			: this(true)
		{
		}

		public UniqueAttribute(bool unique)
		{
			Unique = unique;
		}
	}
}
