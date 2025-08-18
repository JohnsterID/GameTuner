using System;

namespace GameTuner.Framework.BuildProcess
{
	public static class BuildHelper
	{
		public static IBuildStep CloneStep(IBuildStep step)
		{
			IBuildStep buildStep;
			if (step is ICloneable)
			{
				buildStep = (IBuildStep)((ICloneable)step).Clone();
			}
			else
			{
				buildStep = (IBuildStep)ReflectionHelper.CreateInstance(step.GetType());
				buildStep.Arguments = step.Arguments;
				buildStep.Description = step.Description;
				buildStep.ID = step.ID;
				buildStep.Order = step.Order;
				buildStep.AllowRollback = step.AllowRollback;
			}
			return buildStep;
		}
	}
}
