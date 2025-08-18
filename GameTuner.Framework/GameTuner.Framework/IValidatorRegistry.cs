namespace GameTuner.Framework
{
	public interface IValidatorRegistry
	{
		ListEvent<ValidatorProvider> Providers { get; }
	}
}
