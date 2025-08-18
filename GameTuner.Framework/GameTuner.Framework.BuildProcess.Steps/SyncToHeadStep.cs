using System.ComponentModel;

namespace GameTuner.Framework.BuildProcess.Steps
{
	[Description("Perforce sync specifed depot to head revision")]
	[DisplayName("Sync To Head")]
	[Syntax("-f | <depot paths>")]
	public class SyncToHeadStep : IBuildStep
	{
		[DisplayName("Force Sync")]
		[Description("Set to true to force sync")]
		[Category("Sync To Head")]
		public bool ForceSync
		{
			get
			{
				return StringHelper.GetStringIndex(Arguments, 0) == "-f";
			}
			set
			{
				Arguments = StringHelper.SetStringIndex(Arguments, 0, value ? "-f" : "");
			}
		}

		[Category("Sync To Head")]
		[Description("Perforce depot path to sync. Separate multiple paths by semicolons")]
		[DisplayName("Depot Path")]
		public virtual string DepotPath
		{
			get
			{
				return StringHelper.GetStringIndex(Arguments, 1);
			}
			set
			{
				Arguments = StringHelper.SetStringIndex(Arguments, 1, value);
			}
		}

		[Browsable(false)]
		public string Arguments { get; set; }

		[Browsable(false)]
		public bool AllowRollback { get; set; }

		[Browsable(false)]
		public int Order { get; set; }

		[Browsable(false)]
		public int ID { get; set; }

		[Description("User defined text describing this step")]
		[Category("Display")]
		public string Description { get; set; }

		public SyncToHeadStep()
		{
			Arguments = "";
			Description = "";
		}

		public virtual void Execute(IBuildJob job, IValidationResults results)
		{
			string[] array = DepotPath.Split(';');
			results.AddLog(string.Format("SyncToHead: {0} (DISABLED - No P4API)", string.Join(" ", array)));
			results.AddLog("Source control sync operations are disabled in this build.");
			
			// No-op implementation - source control functionality removed
			foreach (string path in array)
			{
				results.AddLog(string.Format("      {0} (skipped)", path));
			}
		}

		public void Rollback(IBuildJob job, IValidationResults results)
		{
		}
	}
}
