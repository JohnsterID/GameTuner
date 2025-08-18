using System;
using System.Threading;

namespace GameTuner.Framework.BuildProcess
{
	public class BuildJob : IBuildJob
	{
		private class ValidationResults : IValidationResults
		{
			private BuildResultsArgs resultArgs;

			public object Sender { get; set; }

			public IBuildStep Current { get; set; }

			public ValidationResults(BuildResultsArgs resultArgs)
			{
				this.resultArgs = resultArgs;
			}

			public void AddFailure(ValidationResultLevel level, string brief, object context)
			{
				if (level == ValidationResultLevel.Success || level == ValidationResultLevel.None)
				{
					throw new ArgumentException("Invalid condition for a failure result");
				}
				resultArgs.Results.Add(new BuildResultsArgs.ResultInfo(Current, brief, level, context));
			}

			public void AddSuccess(string brief, object context)
			{
				resultArgs.Results.Add(new BuildResultsArgs.ResultInfo(Current, brief, ValidationResultLevel.Success, context));
			}

			public void AddLog(string brief)
			{
				resultArgs.Log.Add(brief);
			}
		}

		private ValidationResults validation;

		public BuildStepCollection Steps { get; private set; }

		public IValidationResults Validation
		{
			get
			{
				return validation;
			}
		}

		public IBuildSettings Settings { get; set; }

		public BuildResultsArgs BuildResults { get; private set; }

		public EventWaitHandle ExecuteHandle { get; private set; }

		public LocalEnvironment Environment { get; private set; }

		public event BuildResultsHandler BuildFailed;

		public event BuildResultsHandler BuildComplete;

		public event BuildLogAddedHandler BuildLogAdded;

		public BuildJob()
		{
			Steps = new BuildStepCollection();
			ExecuteHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
			BuildResults = new BuildResultsArgs();
			validation = new ValidationResults(BuildResults);
			Environment = new LocalEnvironment();
			BuildResults.Log.AddedItem += Log_OnAddItem;
		}

		private void Log_OnAddItem(object sender, ListEvent<string>.ListEventArgs e)
		{
			BuildLogAddedHandler buildLogAdded = this.BuildLogAdded;
			if (buildLogAdded != null)
			{
				buildLogAdded(this, (string)e.Item.Clone());
			}
		}

		public BuildJob(BuildStepCollection steps)
			: this()
		{
			Steps.AddRange(steps);
		}

		public void Add<T>(string arguments) where T : IBuildStep, new()
		{
			IBuildStep buildStep = new T();
			buildStep.Arguments = arguments;
			Steps.Add(buildStep);
		}

		public void Execute()
		{
			ExecuteHandle.Reset();
			lock (Steps.LockObject)
			{
				BuildResults.Clear();
				bool flag = false;
				int count = Steps.Count;
				for (int i = 0; i < count; i++)
				{
					IBuildStep buildStep = Steps[i];
					validation.Current = buildStep;
					try
					{
						validation.AddLog("BEGIN: " + ReflectionHelper.GetDisplayName(buildStep));
						buildStep.Execute(this, validation);
					}
					catch (Exception ex)
					{
						ExceptionLogger.Log(ex);
						validation.AddFailure(ValidationResultLevel.Error, ex.Message, buildStep);
					}
					finally
					{
						validation.AddLog("END: " + ReflectionHelper.GetDisplayName(buildStep));
						ValidationResultLevel highestResultLevel = BuildResults.HighestResultLevel;
						if (highestResultLevel == ValidationResultLevel.Error && (Settings == null || !Settings.IgnoreErrors))
						{
							if (Settings == null || !Settings.IgnoreRollback)
							{
								Rollback(i);
							}
							flag = true;
						}
					}
					validation.Current = null;
					if (flag)
					{
						break;
					}
				}
				if (flag)
				{
					BuildResultsHandler buildFailed = this.BuildFailed;
					if (buildFailed != null)
					{
						buildFailed(this, BuildResults);
					}
				}
				else
				{
					BuildResultsHandler buildComplete = this.BuildComplete;
					if (buildComplete != null)
					{
						buildComplete(this, BuildResults);
					}
				}
			}
			ExecuteHandle.Set();
		}

		public void Rollback(int index)
		{
			try
			{
				for (int num = index; num >= 0; num--)
				{
					IBuildStep buildStep = Steps[num];
					if (buildStep.AllowRollback)
					{
						buildStep.Rollback(this, validation);
					}
				}
			}
			catch (Exception ex)
			{
				ExceptionLogger.Log(ex);
				validation.AddFailure(ValidationResultLevel.Error, "Rollback: " + ex.Message, null);
			}
		}
	}
}
