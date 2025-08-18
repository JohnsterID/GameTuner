using System;

namespace GameTuner.Framework.BuildProcess
{
	public class BuildStepCollection : ListEvent<IBuildStep>
	{
		public void SortByOrder()
		{
			Sort((IBuildStep a, IBuildStep b) => Math.Sign(a.Order - b.Order));
		}
	}
}
