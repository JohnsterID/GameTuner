using System.Threading;

namespace GameTuner.Framework.BuildProcess
{
	public interface IBuildJob
	{
		BuildStepCollection Steps { get; }

		IBuildSettings Settings { get; set; }

		IValidationResults Validation { get; }

		LocalEnvironment Environment { get; }

		EventWaitHandle ExecuteHandle { get; }

		BuildResultsArgs BuildResults { get; }

		event BuildResultsHandler BuildFailed;

		event BuildResultsHandler BuildComplete;

		event BuildLogAddedHandler BuildLogAdded;

		void Add<T>(string arguments) where T : IBuildStep, new();

		void Execute();

		void Rollback(int index);
	}
}
