using System.Drawing;

namespace GameTuner.Framework.Graph
{
	public interface IGraphDataProvider
	{
		string GraphDataName { get; }

		Image GraphDataImage { get; }
	}
}
