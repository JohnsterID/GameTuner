using System.ComponentModel;

namespace GameTuner.Framework.BuildProcess.Steps
{
	[Syntax("-f | -k | <label>")]
	[DisplayName("Sync To Label")]
	[Description("Perforce sync depot to specified label")]
	public class SyncToLabelStep : IBuildStep
	{
		[Category("Sync To Label")]
		[DisplayName("Force Sync")]
		[Description("Set to true to force sync")]
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

		[Category("Sync To Label")]
		[Description("Set to true to keep workspace files")]
		[DisplayName("Keep Workspace")]
		public bool KeepWorkspace
		{
			get
			{
				return StringHelper.GetStringIndex(Arguments, 1) == "-k";
			}
			set
			{
				Arguments = StringHelper.SetStringIndex(Arguments, 1, value ? "-k" : "");
			}
		}

		[Category("Sync To Label")]
		[DisplayName("Label")]
		[Description("Perforce depot label to sync")]
		public string Label
		{
			get
			{
				return StringHelper.GetStringIndex(Arguments, 2);
			}
			set
			{
				Arguments = StringHelper.SetStringIndex(Arguments, 2, value);
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

		[Category("Display")]
		[Description("User defined text describing this step")]
		public string Description { get; set; }

		public SyncToLabelStep()
		{
			Arguments = "|-k|";
			Description = "";
		}

		public void Execute(IBuildJob job, IValidationResults results)
		{
			string text = Label;
			bool keepWorkspace = KeepWorkspace;
			bool forceSync = ForceSync;
			if (!text.StartsWith("#"))
			{
				text = ((!keepWorkspace) ? ("@" + text) : string.Format("@{0},@{0}", text));
			}
			results.AddLog(string.Format("SyncToLabel: {0} (DISABLED - No P4API)", text));
			results.AddLog("Source control sync operations are disabled in this build.");
		}

		public void Rollback(IBuildJob job, IValidationResults results)
		{
		}
	}
}
