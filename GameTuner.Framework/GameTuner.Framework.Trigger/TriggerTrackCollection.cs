namespace GameTuner.Framework.Trigger
{
	public class TriggerTrackCollection : ListEvent<TriggerTrack>
	{
		public TriggerTrack Find(int ec)
		{
			return Find((TriggerTrack a) => a.ID == ec);
		}
	}
}
