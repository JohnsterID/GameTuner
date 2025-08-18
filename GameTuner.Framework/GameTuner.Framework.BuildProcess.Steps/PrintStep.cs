using System.ComponentModel;

namespace GameTuner.Framework.BuildProcess.Steps
{
	[DisplayName("Print")]
	[Description("Adds an info entry to the job results")]
	[Syntax("<text to display>")]
	public class PrintStep : IBuildStep
	{
		[Browsable(false)]
		public int Order { get; set; }

		[Browsable(false)]
		public int ID { get; set; }

		[Description("User defined text describing this step")]
		[Category("Display")]
		public string Description { get; set; }

		[Category("Display")]
		[DisplayName("Info Text")]
		[Description("Informational text to display in the job results")]
		public string Arguments { get; set; }

		[Browsable(false)]
		public bool AllowRollback { get; set; }

		public PrintStep()
		{
			Arguments = "";
			Description = "";
		}

		public void Execute(IBuildJob job, IValidationResults results)
		{
			results.AddLog("Print: " + Arguments);
		}

		public void Rollback(IBuildJob job, IValidationResults results)
		{
		}
	}
}
