namespace GameTuner.Framework.BuildProcess
{
	public interface IBuildSettings
	{
		bool IgnoreErrors { get; set; }

		bool IgnoreRollback { get; set; }
	}
}
