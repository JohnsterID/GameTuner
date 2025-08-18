namespace GameTuner.Framework.Trigger
{
	public interface ITrigger : IUniqueID
	{
		ITriggerSystem Owner { get; }

		float Time { get; set; }

		float Duration { get; set; }

		bool Repeat { get; set; }
	}
}
