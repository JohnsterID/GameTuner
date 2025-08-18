namespace GameTuner.Framework
{
	public class ValidatorRegistry : IValidatorRegistry
	{
		public ListEvent<ValidatorProvider> Providers { get; private set; }

		public ValidatorRegistry()
		{
			Providers = new ListEvent<ValidatorProvider>();
		}
	}
}
