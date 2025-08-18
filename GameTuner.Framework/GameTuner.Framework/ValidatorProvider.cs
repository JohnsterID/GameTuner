using System;
using System.Collections.Generic;

namespace GameTuner.Framework
{
	public class ValidatorProvider
	{
		public class ResultInfo
		{
			public IValidator Validator;

			public ValidationResultLevel Level;

			public string Brief;

			public object Context;

			public object Sender;

			public ResultInfo(IValidator validator, string brief, ValidationResultLevel level, object context, object sender)
			{
				Validator = validator;
				Brief = brief;
				Level = level;
				Context = context;
				Sender = sender;
			}
		}

		public class ResultInfoCollection : ListEvent<ResultInfo>
		{
		}

		private class ValidationResults : IValidationResults
		{
			private ValidatorProvider provider;

			private IValidator current;

			private ResultInfoCollection results;

			public object Sender { get; set; }

			public ResultInfoCollection Results
			{
				get
				{
					return results;
				}
			}

			public List<string> Log { get; private set; }

			public IValidator Current
			{
				get
				{
					return current;
				}
				set
				{
					current = value;
				}
			}

			public ValidationResults(ValidatorProvider provider)
			{
				this.provider = provider;
				results = new ResultInfoCollection();
				Log = new List<string>();
			}

			public void AddFailure(ValidationResultLevel level, string brief, object context)
			{
				if (level == ValidationResultLevel.Success || level == ValidationResultLevel.None)
				{
					throw new ArgumentException("Invalid condition for a failure result");
				}
				results.Add(new ResultInfo(current, brief, level, context, Sender));
			}

			public void AddSuccess(string brief, object context)
			{
				results.Add(new ResultInfo(current, brief, ValidationResultLevel.Success, context, Sender));
			}

			public void AddLog(string brief)
			{
				Log.Add(brief);
			}
		}

		private ValidatorCollection validators;

		private ValidationResults results;

		private bool cleared;

		public string Name { get; set; }

		public bool SaveResults { get; set; }

		public bool UpdateStatus { get; set; }

		public IValidationResults ResultObject
		{
			get
			{
				return results;
			}
		}

		public IEnumerable<IValidator> Validators
		{
			get
			{
				foreach (IValidator validator in validators)
				{
					yield return validator;
				}
				IValidatorSource source = Context.GetService<IValidatorSource>();
				if (source == null)
				{
					yield break;
				}
				foreach (IValidator validator2 in source.GetValidators(Name))
				{
					yield return validator2;
				}
			}
		}

		public ResultInfoCollection Results
		{
			get
			{
				return results.Results;
			}
		}

		public List<string> Log
		{
			get
			{
				return results.Log;
			}
		}

		public ValidationResultLevel HighestResultLevel
		{
			get
			{
				int num = 0;
				if (!cleared)
				{
					num = 1;
					foreach (ResultInfo result in Results)
					{
						if ((int)result.Level > num)
						{
							num = (int)result.Level;
						}
					}
				}
				return (ValidationResultLevel)num;
			}
		}

		public event EventHandler TestPerformed;

		public event EventHandler ResultsCleared;

		public event EventHandler ValidatorsChanged;

		public ValidatorProvider()
			: this("")
		{
		}

		public ValidatorProvider(string name)
		{
			Name = name;
			cleared = true;
			UpdateStatus = true;
			validators = new ValidatorCollection();
			validators.ItemCountChanged += validators_ItemCountChanged;
			results = new ValidationResults(this);
			results.Results.ClearedItems += Results_OnClear;
			IValidatorSource service = Context.GetService<IValidatorSource>();
			if (service != null)
			{
				service.ValidationChanged += source_ValidationChanged;
			}
		}

		private void source_ValidationChanged(object sender, ValidatorSourceEventArgs e)
		{
			if (!string.IsNullOrEmpty(Name) && Name.CompareTo(e.Name) == 0)
			{
				OnValidatorsChanged(EventArgs.Empty);
			}
		}

		private void validators_ItemCountChanged(object sender, EventArgs e)
		{
			OnValidatorsChanged(EventArgs.Empty);
		}

		protected virtual void OnValidatorsChanged(EventArgs e)
		{
			EventHandler validatorsChanged = this.ValidatorsChanged;
			if (validatorsChanged != null)
			{
				validatorsChanged(this, e);
			}
		}

		private void Results_OnClear(object sender, EventArgs e)
		{
			cleared = true;
			EventHandler resultsCleared = this.ResultsCleared;
			if (resultsCleared != null)
			{
				resultsCleared(this, EventArgs.Empty);
			}
		}

		public void Add(IValidator v)
		{
			validators.Add(v);
		}

		public virtual void RunTests()
		{
			RunTests(null);
		}

		public void RunTests(object sender)
		{
			bool flag = false;
			if (!SaveResults)
			{
				Clear();
			}
			results.Sender = sender;
			foreach (IValidator validator in Validators)
			{
				if (validator.Enabled)
				{
					results.Current = validator;
					try
					{
						validator.Evaluate(results);
					}
					catch (Exception ex)
					{
						string text = "Validate Exception: " + ex.Message;
						ErrorHandling.Error(ex, text, ErrorLevel.Log);
						results.AddFailure(ValidationResultLevel.Error, text, null);
					}
					results.Current = null;
					flag = true;
				}
			}
			if (flag)
			{
				cleared = false;
			}
			OnTestPerformed(EventArgs.Empty);
		}

		public void Clear()
		{
			Results.Clear();
			results.Log.Clear();
		}

		protected virtual void OnTestPerformed(EventArgs e)
		{
			EventHandler testPerformed = this.TestPerformed;
			if (testPerformed != null)
			{
				testPerformed(this, e);
			}
		}
	}
}
