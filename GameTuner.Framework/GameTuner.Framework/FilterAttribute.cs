using System;

namespace GameTuner.Framework
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public class FilterAttribute : Attribute
	{
		private string filter;

		public string Filter
		{
			get
			{
				return filter;
			}
		}

		public FilterAttribute()
		{
			filter = "";
		}

		public FilterAttribute(string filter)
		{
			this.filter = filter;
		}
	}
}
