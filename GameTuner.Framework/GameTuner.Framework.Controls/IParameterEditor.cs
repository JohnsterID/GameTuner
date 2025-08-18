using System;

namespace GameTuner.Framework.Controls
{
	public interface IParameterEditor
	{
		string Name { get; set; }

		object Value { get; set; }

		object Tag { get; set; }

		event EventHandler ValueChanged;
	}
}
