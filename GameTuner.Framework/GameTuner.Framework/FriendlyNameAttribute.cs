using System;
using System.ComponentModel;

namespace GameTuner.Framework
{
	[AttributeUsage(AttributeTargets.All)]
	public class FriendlyNameAttribute : DisplayNameAttribute
	{
		public FriendlyNameAttribute(string name)
			: base(name)
		{
		}
	}
}
