namespace GameTuner.Framework.Trigger
{
	public interface ITriggerSystem : ITagProvider, IServiceProviderProvider
	{
		TriggerCollection Triggers { get; }

		TriggerTrackCollection Tracks { get; }

		Factory<ITrigger> TriggerFactory { get; }
	}
}
