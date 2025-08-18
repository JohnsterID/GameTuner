using System.Collections.Generic;

namespace GameTuner.Framework
{
	public interface IAssetTargetCollector
	{
		IEnumerable<IAssetTarget> AssetTargets { get; }
	}
}
