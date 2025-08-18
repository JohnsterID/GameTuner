using System;

namespace GameTuner.Framework
{
	public class ValidatorSourceEventArgs : EventArgs
	{
		public string Name { get; private set; }

		public ValidatorSourceEventArgs(string name)
		{
			Name = name;
		}
	}
}
