using System.ComponentModel;
using System.IO;

namespace GameTuner.Framework.BuildProcess.Steps
{
	[DisplayName("Rename File")]
	[Description("Renames the specified file")]
	[Syntax("<oldfile> | [<new file]> | -f")]
	public class RenameFileStep : IBuildStep
	{
		private bool succeeded;

		[Browsable(false)]
		public int Order { get; set; }

		[Browsable(false)]
		public int ID { get; set; }

		[Category("Display")]
		[Description("User defined text describing this step")]
		public string Description { get; set; }

		[DisplayName("Source")]
		[Description("Source file to rename")]
		[Category("Rename File")]
		public string SourceFile
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

		[Category("Rename File")]
		[Description("Result file to rename")]
		[DisplayName("Target")]
		public string TargetFile
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

		[DefaultValue(false)]
		[Description("Set to true to force rename (e.g. dest file already exists)")]
		[Category("Rename File")]
		public bool ForceRename
		{
			get
			{
				return StringHelper.GetStringIndex(Arguments, 2) == "-f";
			}
			set
			{
				Arguments = StringHelper.SetStringIndex(Arguments, 2, value ? "-f" : "");
			}
		}

		[Description("Set to true to enable undo this step on any error")]
		[Category("Error")]
		[DisplayName("Rollback")]
		[DefaultValue(true)]
		public bool AllowRollback { get; set; }

		[Browsable(false)]
		public string Arguments { get; set; }

		public RenameFileStep()
		{
			Arguments = "";
			Description = "";
			AllowRollback = true;
		}

		public virtual void Execute(IBuildJob job, IValidationResults results)
		{
			succeeded = Rename(job, results, SourceFile, TargetFile);
		}

		public virtual void Rollback(IBuildJob job, IValidationResults results)
		{
			if (succeeded)
			{
				Rename(job, results, TargetFile, SourceFile);
			}
		}

		protected virtual bool Rename(IBuildJob job, IValidationResults results, string src, string dst)
		{
			if (!File.Exists(src))
			{
				results.AddFailure(ValidationResultLevel.Error, string.Format("RenameFile: source file '{0}' not found", src), this);
			}
			else
			{
				bool flag = File.Exists(dst);
				if (ForceRename || !flag)
				{
					if (flag)
					{
						File.SetAttributes(dst, FileAttributes.Normal);
						File.Delete(dst);
					}
					results.AddLog(string.Format("RenameFile: {0} -> {1}", src, dst));
					File.Move(src, dst);
					return true;
				}
				results.AddFailure(ValidationResultLevel.Error, string.Format("RenameFile: dest file name '{0}' already exists", dst), this);
			}
			return false;
		}
	}
}
