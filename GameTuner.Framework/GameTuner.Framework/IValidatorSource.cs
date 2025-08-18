using System;
using System.Collections.Generic;

namespace GameTuner.Framework
{
	public interface IValidatorSource
	{
		IEnumerable<IValidator> Validators { get; }

		event EventHandler<ValidatorSourceEventArgs> ValidationChanged;

		IEnumerable<IValidator> GetValidators(string name);

		void Rebuild(IEnumerable<ValidatorSourceEntry> entries);
	}
}
