using System;

namespace GameTuner.Framework
{
	public interface IMaker
	{
		string Name { get; }

		Type Type { get; }

		object Make();

		object Make(params object[] args);
	}
}
