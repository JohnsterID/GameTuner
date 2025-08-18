using System;

namespace GameTuner.Framework
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class MaxValueAttribute : Attribute
	{
		public object Value { get; private set; }

		public MaxValueAttribute(object value)
		{
			Value = value;
		}
	}
}
