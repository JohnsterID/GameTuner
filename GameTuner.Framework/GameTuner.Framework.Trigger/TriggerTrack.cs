namespace GameTuner.Framework.Trigger
{
	public class TriggerTrack : IUniqueID
	{
		public class SubTrack : IUniqueID
		{
			public int ID { get; set; }

			public string Name { get; set; }

			public SubTrack()
			{
				Name = "";
			}

			public SubTrack(string name, int id)
			{
				Name = name;
				ID = id;
			}
		}

		public class SubTrackCollection : ListEventID<SubTrack>
		{
		}

		public SubTrackCollection SubTracks { get; private set; }

		public int ID { get; set; }

		public TriggerTrack()
		{
			SubTracks = new SubTrackCollection();
		}

		public TriggerTrack(int id)
		{
			SubTracks = new SubTrackCollection();
			ID = id;
		}
	}
}
