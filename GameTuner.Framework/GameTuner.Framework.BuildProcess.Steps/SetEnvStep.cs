using System;
using System.ComponentModel;

namespace GameTuner.Framework.BuildProcess.Steps
{
	[Syntax("<Environment name> | <Environment Value>")]
	[DisplayName("Set Environment")]
	[Description("Sets an Environment to the specified value")]
	public class SetEnvStep : IBuildStep
	{
		private string oldValue;

		[Browsable(false)]
		public int Order { get; set; }

		[Browsable(false)]
		public int ID { get; set; }

		[Description("User defined text describing this step")]
		[Category("Display")]
		public string Description { get; set; }

		[Description("Environment variable name to set")]
		[Category("Set Environment")]
		[DisplayName("Variable")]
		public string Variable
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

		[Category("Set Environment")]
		[Description("Value of variable")]
		[DisplayName("Value")]
		public string VarValue
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

		[Category("Error")]
		[DisplayName("Rollback")]
		[DefaultValue(true)]
		[Description("Set to true to enable undo this step on any error")]
		public bool AllowRollback { get; set; }

		public SetEnvStep()
		{
			AllowRollback = true;
			Arguments = "";
			Description = "";
		}

		public void Execute(IBuildJob job, IValidationResults results)
		{
			string variable = Variable;
			string varValue = VarValue;
			results.AddLog(string.Format("SetEnv: {0} = {1}", variable, varValue));
			oldValue = Environment.GetEnvironmentVariable(variable);
			Environment.SetEnvironmentVariable(variable, varValue);
		}

		public void Rollback(IBuildJob job, IValidationResults results)
		{
			if (oldValue != null)
			{
				Environment.SetEnvironmentVariable(Variable, oldValue);
			}
		}
	}
}
