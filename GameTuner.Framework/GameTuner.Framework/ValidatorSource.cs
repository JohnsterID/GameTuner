using System;
using System.Collections.Generic;

namespace GameTuner.Framework
{
	public class ValidatorSource : IValidatorSource
	{
		private Dictionary<string, ListEvent<IValidator>> validatorMap;

		public IEnumerable<IValidator> Validators
		{
			get
			{
				foreach (KeyValuePair<string, ListEvent<IValidator>> kvp in validatorMap)
				{
					KeyValuePair<string, ListEvent<IValidator>> keyValuePair = kvp;
					foreach (IValidator item in keyValuePair.Value)
					{
						yield return item;
					}
				}
			}
		}

		public event EventHandler<ValidatorSourceEventArgs> ValidationChanged;

		public ValidatorSource()
		{
			validatorMap = new Dictionary<string, ListEvent<IValidator>>();
		}

		public void Rebuild(IEnumerable<ValidatorSourceEntry> entries)
		{
			foreach (ValidatorSourceEntry entry in entries)
			{
				string name = entry.Name;
				ListEvent<IValidator> value;
				if (!validatorMap.TryGetValue(name, out value))
				{
					value = new ListEvent<IValidator>();
					validatorMap.Add(name, value);
				}
				value.Clear();
				foreach (Type type in entry.Types)
				{
					value.Add((IValidator)ReflectionHelper.CreateInstance(type));
				}
				OnValidationChanged(new ValidatorSourceEventArgs(name));
			}
		}

		public IEnumerable<IValidator> GetValidators(string name)
		{
			ListEvent<IValidator> list;
			if (!validatorMap.TryGetValue(name, out list))
			{
				yield break;
			}
			foreach (IValidator item in list)
			{
				yield return item;
			}
		}

		protected virtual void OnValidationChanged(ValidatorSourceEventArgs e)
		{
			EventHandler<ValidatorSourceEventArgs> validationChanged = this.ValidationChanged;
			if (validationChanged != null)
			{
				validationChanged(this, e);
			}
		}
	}
}
