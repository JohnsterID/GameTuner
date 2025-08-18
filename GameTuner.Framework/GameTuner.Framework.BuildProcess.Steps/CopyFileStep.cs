using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace GameTuner.Framework.BuildProcess.Steps
{
	[Syntax("<source file> | <dest file or path> | -r {recursive} | {ReadOnly:Normal}")]
	[Description("Copies an existing file to a new file")]
	[DisplayName("Copy File")]
	public class CopyFileStep : IBuildStep
	{
		public enum AttribMode
		{
			Nothing,
			ReadOnly,
			Normal
		}

		private List<string> copiedFiles;

		[Browsable(false)]
		public int Order { get; set; }

		[Browsable(false)]
		public int ID { get; set; }

		[Description("User defined text describing this step")]
		[Category("Display")]
		public string Description { get; set; }

		[DisplayName("Source")]
		[Category("Copy Files")]
		[Description("Source files to copy")]
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

		[Description("Destination file or location")]
		[Category("Copy Files")]
		[DisplayName("Target")]
		public string Target
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

		[DefaultValue(true)]
		[Description("Set to true to copy subfolders")]
		[Category("Method")]
		public bool Recursive
		{
			get
			{
				return StringHelper.GetStringIndex(Arguments, 2) == "-r";
			}
			set
			{
				Arguments = StringHelper.SetStringIndex(Arguments, 2, value ? "-r" : "");
			}
		}

		[Description("Destination file attribute to set")]
		[DefaultValue(true)]
		[DisplayName("AttribMode")]
		[Category("Method")]
		public AttribMode Mode
		{
			get
			{
				return StringHelper.EnumValue<AttribMode>(StringHelper.GetStringIndex(Arguments, 3));
			}
			set
			{
				Arguments = StringHelper.SetStringIndex(Arguments, 3, value.ToString());
			}
		}

		[Browsable(false)]
		public string Arguments { get; set; }

		[Category("Error")]
		[Description("Set to true to enable undo this step on any error")]
		[DefaultValue(true)]
		[DisplayName("Rollback")]
		public bool AllowRollback { get; set; }

		public CopyFileStep()
		{
			copiedFiles = new List<string>();
			Arguments = "||-r";
			Description = "";
			AllowRollback = true;
		}

		public virtual void Execute(IBuildJob job, IValidationResults results)
		{
			string sourceFile = SourceFile;
			string text = Target;
			copiedFiles.Clear();
			if (sourceFile.IndexOfAny(new char[2] { '*', '?' }) != -1)
			{
				string directoryName = Path.GetDirectoryName(sourceFile);
				string fileName = Path.GetFileName(sourceFile);
				CopyDir(directoryName, fileName, text, results);
				return;
			}
			if (text.EndsWith("\\"))
			{
				text = Path.Combine(text, Path.GetFileName(sourceFile));
			}
			Copy(sourceFile, text, results);
		}

		public void Rollback(IBuildJob job, IValidationResults results)
		{
			foreach (string copiedFile in copiedFiles)
			{
				results.AddLog(string.Format("CopyFile.Rollback: Deleting {0}", copiedFile));
				File.Delete(copiedFile);
			}
		}

		protected virtual void Copy(string srcFile, string destFile, IValidationResults results)
		{
			AttribMode mode = Mode;
			results.AddLog(string.Format("CopyFile: {0} -> {1} : {2}", srcFile, destFile, mode.ToString()));
			if (File.Exists(destFile))
			{
				File.SetAttributes(destFile, FileAttributes.Normal);
			}
			File.Copy(srcFile, destFile, true);
			switch (mode)
			{
			case AttribMode.Normal:
				File.SetAttributes(destFile, FileAttributes.Normal);
				break;
			case AttribMode.ReadOnly:
				File.SetAttributes(destFile, FileAttributes.ReadOnly);
				break;
			}
			copiedFiles.Add(destFile);
		}

		private void CopyDir(string dirPath, string mask, string destPath, IValidationResults results)
		{
			if (!Directory.Exists(destPath))
			{
				Directory.CreateDirectory(destPath);
			}
			string[] files = Directory.GetFiles(dirPath, mask);
			string[] array = files;
			foreach (string text in array)
			{
				string destFile = Path.Combine(destPath, Path.GetFileName(text));
				Copy(text, destFile, results);
			}
			if (Recursive)
			{
				string[] directories = Directory.GetDirectories(dirPath);
				string[] array2 = directories;
				foreach (string text2 in array2)
				{
					string text3 = text2.Replace(SourceFile, "");
					text3 = text3.TrimStart('\\');
					text3 = Path.Combine(Target, text3);
					CopyDir(text2, mask, text3, results);
				}
			}
		}
	}
}
