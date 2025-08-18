namespace GameTuner.Framework.Graph
{
	public interface IFlowGraphBlockCurve
	{
		int NumBlocks { get; }

		int NumUsedBlocks { get; }

		BlockCurveInfo GetBlock(int index);

		void ResetBlocks();
	}
}
