using System;

namespace GameTuner.Framework
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class MinValueAttribute : Attribute
	{
		public object Value { get; private set; }

		public MinValueAttribute(object value)
		{
			Value = value;
		}
	}
}
