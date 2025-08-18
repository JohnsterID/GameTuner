using System;
using System.Collections.Generic;

namespace GameTuner.Framework
{
	public class DelayedDelegate
	{
		private KeyValuePair<Delegate, object[]> proc;

		public DelayedDelegate(Delegate d, params object[] args)
		{
			proc = new KeyValuePair<Delegate, object[]>(d, args);
		}

		public object Execute()
		{
			if ((object)proc.Key != null)
			{
				return proc.Key.DynamicInvoke(proc.Value);
			}
			return null;
		}
	}
}
