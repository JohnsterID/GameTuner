using System;

namespace GameTuner.Framework
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
	public class StrideAttribute : Attribute
	{
		private int size;

		public int Size
		{
			get
			{
				return size;
			}
		}

		public StrideAttribute(int size)
		{
			this.size = size;
		}
	}
}
