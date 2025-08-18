using System;

namespace GameTuner.Framework.BuildProcess
{
	public class BuildResultsArgs : EventArgs
	{
		public class ResultInfo
		{
			public IBuildStep BuildStep;

			public ValidationResultLevel Level;

			public string Brief;

			public object Context;

			public ResultInfo(IBuildStep buildStep, string brief, ValidationResultLevel level, object context)
			{
				BuildStep = buildStep;
				Brief = brief;
				Level = level;
				Context = context;
			}
		}

		public class ResultInfoCollection : ListEvent<ResultInfo>
		{
		}

		public ResultInfoCollection Results { get; private set; }

		public ListEvent<string> Log { get; private set; }

		public ValidationResultLevel HighestResultLevel
		{
			get
			{
				int num = 1;
				foreach (ResultInfo result in Results)
				{
					if ((int)result.Level > num)
					{
						num = (int)result.Level;
					}
				}
				return (ValidationResultLevel)num;
			}
		}

		public BuildResultsArgs()
		{
			Results = new ResultInfoCollection();
			Log = new ListEvent<string>();
		}

		public void Clear()
		{
			Results.Clear();
			Log.Clear();
		}
	}
}
