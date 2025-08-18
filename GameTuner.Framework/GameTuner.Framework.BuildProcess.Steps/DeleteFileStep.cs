using System.ComponentModel;
using System.IO;

namespace GameTuner.Framework.BuildProcess.Steps
{
	[Syntax("<Path or file> | [<wildcard mask]>")]
	[DisplayName("Delete File")]
	[Description("Deletes the specified file(s)")]
	public class DeleteFileStep : IBuildStep
	{
		[Browsable(false)]
		public int Order { get; set; }

		[Browsable(false)]
		public int ID { get; set; }

		[Description("User defined text describing this step")]
		[Category("Display")]
		public string Description { get; set; }

		[Description("Path of file(s) to delete")]
		[DisplayName("Path")]
		[Category("Delete File")]
		public string PathFile
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

		[Description("File or Mask name (e.g. *.tmp)")]
		[Category("Delete File")]
		[DisplayName("File")]
		public string MaskFile
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

		public DeleteFileStep()
		{
			Arguments = "";
			Description = "";
		}

		public void Execute(IBuildJob job, IValidationResults results)
		{
			string pathFile = PathFile;
			string maskFile = MaskFile;
			results.AddLog(string.Format("DeleteFile: {0}\\{1}", pathFile, maskFile));
			if (maskFile.Length > 0)
			{
				string[] files = Directory.GetFiles(pathFile, maskFile);
				string[] array = files;
				foreach (string text in array)
				{
					File.Delete(text);
					results.AddLog(string.Format("Deleted {0}", text));
				}
			}
			else
			{
				File.Delete(pathFile);
				results.AddLog(string.Format("Deleted {0}", pathFile));
			}
		}

		public void Rollback(IBuildJob job, IValidationResults results)
		{
		}
	}
}
