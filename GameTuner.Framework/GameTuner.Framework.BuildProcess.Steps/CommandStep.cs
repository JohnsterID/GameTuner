using System;
using System.ComponentModel;
using System.Diagnostics;

namespace GameTuner.Framework.BuildProcess.Steps
{
	[Syntax("<executable> | <arguments> | -c {check return code as error}")]
	[Description("Executes a command")]
	[DisplayName("Command")]
	public class CommandStep : IBuildStep
	{
		private IValidationResults results;

		[Description("Executable or script to run")]
		[Category("Command")]
		public string Command
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

		[Description("parameters to pass as arguments to the command")]
		[Category("Command")]
		[DisplayName("Arguments")]
		public string Args
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

		[Category("Command")]
		[DefaultValue(true)]
		[DisplayName("Check ReturnCode")]
		[Description("Set to True for non-zero return codes to act as step failure")]
		public bool CheckReturnCode
		{
			get
			{
				return StringHelper.GetStringIndex(Arguments, 2) == "-c";
			}
			set
			{
				Arguments = StringHelper.SetStringIndex(Arguments, 2, value ? "-c" : "");
			}
		}

		[Browsable(false)]
		public int Order { get; set; }

		[Browsable(false)]
		public int ID { get; set; }

		[Description("User defined text describing this step")]
		[Category("Display")]
		public string Description { get; set; }

		[Browsable(false)]
		public bool AllowRollback { get; set; }

		[Browsable(false)]
		public Process CmdProcess { get; private set; }

		public CommandStep()
		{
			Arguments = "||-c";
			Description = "";
			CmdProcess = new Process();
			CmdProcess.StartInfo.UseShellExecute = false;
			CmdProcess.StartInfo.RedirectStandardOutput = true;
			CmdProcess.OutputDataReceived += StandardOutputHandler;
			CmdProcess.StartInfo.RedirectStandardError = true;
			CmdProcess.ErrorDataReceived += StandardErrorHandler;
		}

		public void Execute(IBuildJob job, IValidationResults results)
		{
			this.results = results;
			CmdProcess.StartInfo.FileName = Command;
			CmdProcess.StartInfo.Arguments = Args;
			CmdProcess.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
			results.AddLog(string.Format("Command: {0} {1}", CmdProcess.StartInfo.FileName, CmdProcess.StartInfo.Arguments));
			CmdProcess.Start();
			CmdProcess.BeginOutputReadLine();
			CmdProcess.BeginErrorReadLine();
			CmdProcess.WaitForExit();
			if (CheckReturnCode && CmdProcess.ExitCode != 0)
			{
				results.AddFailure(ValidationResultLevel.Error, string.Format("Non-zero exit code {0}", CmdProcess.ExitCode), this);
			}
		}

		public void Rollback(IBuildJob job, IValidationResults results)
		{
		}

		private void StandardOutputHandler(object sendingProcess, DataReceivedEventArgs outputLine)
		{
			if (outputLine.Data != null && outputLine.Data.Length > 0)
			{
				results.AddLog(outputLine.Data);
			}
		}

		private void StandardErrorHandler(object sendingProcess, DataReceivedEventArgs outputLine)
		{
			if (outputLine.Data != null && outputLine.Data.Length > 0)
			{
				results.AddFailure(ValidationResultLevel.Error, outputLine.Data, this);
			}
		}
	}
}
