using System;

namespace GameTuner.Framework
{
	public interface ISourceControlLabel
	{
		string Name { get; }

		DateTime Date { get; }
	}
}
