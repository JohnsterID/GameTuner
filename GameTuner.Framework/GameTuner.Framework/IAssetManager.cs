namespace GameTuner.Framework
{
	public interface IAssetManager
	{
		AssetProviderCollection Assets { get; }

		ProviderFactory Factory { get; }

		IAssetProvider LaunchAsset(string file);

		IAssetProvider LaunchAsset(string file, IAssetProvider parent);
	}
}
