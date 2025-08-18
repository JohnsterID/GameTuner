using System.IO;

namespace GameTuner.Framework
{
	public class AssetProviderCollection : ListEvent<IAssetProvider>
	{
		public IAssetProvider FindByAsset(string file)
		{
			string name = Path.GetFileName(file);
			return Find((IAssetProvider a) => string.Compare(name, Path.GetFileName(a.Asset), true) == 0);
		}
	}
}
