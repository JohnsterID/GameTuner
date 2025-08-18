using System.Collections.Generic;

namespace GameTuner.Framework
{
	public interface ISourceControl
	{
		bool IsConnected { get; }

		string Port { get; set; }

		string Password { get; set; }

		string Client { get; set; }

		void Submit(string description, string[] files);

		string GetLocalPathFromDepot(string depot);

		bool IsSourceControlPath(string szPath);

		List<ISourceControlLabel> CollectLabels();

		List<ISourceControlLabel> CollectLabels(string pattern);
	}
}
