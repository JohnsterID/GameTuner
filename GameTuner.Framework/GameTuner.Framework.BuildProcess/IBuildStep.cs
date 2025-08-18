namespace GameTuner.Framework.BuildProcess
{
	public interface IBuildStep
	{
		string Arguments { get; set; }

		bool AllowRollback { get; set; }

		int Order { get; set; }

		int ID { get; set; }

		string Description { get; set; }

		void Execute(IBuildJob job, IValidationResults results);

		void Rollback(IBuildJob job, IValidationResults results);
	}
}
