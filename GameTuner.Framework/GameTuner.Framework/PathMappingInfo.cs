namespace GameTuner.Framework
{
	public struct PathMappingInfo
	{
		public string LocalPath;

		public string DepotPath;

		private static PathMappingInfo empty = new PathMappingInfo("", "");

		public static PathMappingInfo Empty
		{
			get
			{
				return empty;
			}
		}

		public PathMappingInfo(string localPath, string depotPath)
		{
			LocalPath = localPath;
			DepotPath = depotPath;
		}
	}
}
