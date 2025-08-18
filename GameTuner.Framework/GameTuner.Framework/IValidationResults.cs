namespace GameTuner.Framework
{
	public interface IValidationResults
	{
		object Sender { get; set; }

		void AddFailure(ValidationResultLevel level, string brief, object context);

		void AddSuccess(string brief, object context);

		void AddLog(string brief);
	}
}
