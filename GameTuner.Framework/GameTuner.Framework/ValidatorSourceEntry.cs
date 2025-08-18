using System;
using System.Collections.Generic;

namespace GameTuner.Framework
{
	public class ValidatorSourceEntry
	{
		public string Name { get; private set; }

		public IEnumerable<Type> Types { get; private set; }

		public ValidatorSourceEntry(string name, IEnumerable<Type> types)
		{
			Name = name;
			Types = types;
		}
	}
}
