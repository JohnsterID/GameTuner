namespace GameTuner.Framework
{
	public interface IAssetTarget
	{
		IAssetProvider ParentAsset { get; set; }

		string TargetName { get; }

		void ShowIt();
	}
}
