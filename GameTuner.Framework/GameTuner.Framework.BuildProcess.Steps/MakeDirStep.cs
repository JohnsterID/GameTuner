using System.ComponentModel;
using System.IO;

namespace GameTuner.Framework.BuildProcess.Steps
{
	[Description("Creates a directory")]
	[DisplayName("Make Directory")]
	[Syntax("<path>")]
	public class MakeDirStep : IBuildStep
	{
		[Category("Make Directory")]
		[DisplayName("Path")]
		[Description("Path of directory to create")]
		public string Arguments { get; set; }

		[DefaultValue(true)]
		[DisplayName("Rollback")]
		[Description("Set to true to enable undo this step on any error")]
		[Category("Error")]
		public bool AllowRollback { get; set; }

		[Browsable(false)]
		public int Order { get; set; }

		[Browsable(false)]
		public int ID { get; set; }

		[Description("User defined text describing this step")]
		[Category("Display")]
		public string Description { get; set; }

		public MakeDirStep()
		{
			Arguments = "";
			AllowRollback = true;
			Description = "";
		}

		public void Execute(IBuildJob job, IValidationResults results)
		{
			results.AddLog(string.Format("MakeDir: {0}", Arguments));
			if (!Directory.Exists(Arguments))
			{
				Directory.CreateDirectory(Arguments);
				results.AddLog(string.Format("            Created {0}", Arguments));
			}
		}

		public void Rollback(IBuildJob job, IValidationResults results)
		{
			if (Arguments.Length > 0)
			{
				results.AddLog(string.Format("MakeDir.Rollback: Deleting {0}", Arguments));
				Directory.Delete(Arguments, true);
			}
		}
	}
}
