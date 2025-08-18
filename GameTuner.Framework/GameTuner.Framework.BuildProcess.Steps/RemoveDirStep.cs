using System.ComponentModel;
using System.IO;

namespace GameTuner.Framework.BuildProcess.Steps
{
	[DisplayName("Remove Directory")]
	[Description("Removes a directory or directories")]
	[Syntax("<path> | <mask>")]
	public class RemoveDirStep : IBuildStep
	{
		[Browsable(false)]
		public string Arguments { get; set; }

		[Description("Path of directory to remove")]
		[Category("Remove Directory")]
		[DisplayName("Path")]
		public string Path
		{
			get
			{
				return StringHelper.GetStringIndex(Arguments, 0);
			}
			set
			{
				Arguments = StringHelper.SetStringIndex(Arguments, 0, value);
			}
		}

		[DisplayName("Mask")]
		[Description("Optional wildcard for multiple removals")]
		[Category("Remove Directory")]
		public string Mask
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
		public bool AllowRollback { get; set; }

		[Browsable(false)]
		public int Order { get; set; }

		[Browsable(false)]
		public int ID { get; set; }

		[Category("Display")]
		[Description("User defined text describing this step")]
		public string Description { get; set; }

		public RemoveDirStep()
		{
			Arguments = "";
			Description = "";
		}

		public void Execute(IBuildJob job, IValidationResults results)
		{
			string path = Path;
			string mask = Mask;
			results.AddLog(string.Format("RemoveDir: {0}\\{1}", path, mask));
			if (!Directory.Exists(path))
			{
				return;
			}
			if (mask.Length > 0)
			{
				string[] directories = Directory.GetDirectories(path, mask);
				string[] array = directories;
				foreach (string text in array)
				{
					Directory.Delete(text, true);
					results.AddLog(string.Format("            Deleted {0}", text));
				}
			}
			else
			{
				Directory.Delete(path, true);
				results.AddLog(string.Format("            Deleted {0}", path));
			}
		}

		public void Rollback(IBuildJob job, IValidationResults results)
		{
		}
	}
}
