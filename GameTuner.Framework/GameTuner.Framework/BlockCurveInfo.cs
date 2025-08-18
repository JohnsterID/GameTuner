namespace GameTuner.Framework
{
	public class BlockCurveInfo
	{
		public BlockCurveType Type { get; set; }

		public int FlipX { get; set; }

		public int FlipY { get; set; }

		public float Dist { get; set; }

		public BlockCurveInfo()
		{
			Reset();
		}

		public BlockCurveInfo(BlockCurveInfo block)
		{
			Set(block);
		}

		public void Set(BlockCurveInfo block)
		{
			Type = block.Type;
			Dist = block.Dist;
			FlipX = block.FlipX;
			FlipY = block.FlipY;
		}

		public BlockCurveInfo(BlockCurveType type, float dist)
		{
			Type = type;
			Dist = dist;
			FlipX = 0;
			FlipY = 0;
		}

		public void Reset()
		{
			Type = BlockCurveType.None;
			Dist = 0.33f;
			FlipX = 0;
			FlipY = 0;
		}
	}
}
