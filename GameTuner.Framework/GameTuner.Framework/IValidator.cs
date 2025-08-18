namespace GameTuner.Framework
{
	public interface IValidator
	{
		bool Enabled { get; set; }

		void Evaluate(IValidationResults results);

		void Inspect(IValidationResults results, object context);
	}
}
