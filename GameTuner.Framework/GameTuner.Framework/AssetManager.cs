using System.IO;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public class AssetManager : IAssetManager
	{
		public AssetProviderCollection Assets { get; private set; }

		public ProviderFactory Factory { get; private set; }

		public AssetManager()
		{
			Assets = new AssetProviderCollection();
			Factory = new ProviderFactory();
		}

		public IAssetProvider LaunchAsset(string file)
		{
			return LaunchAsset(file, null);
		}

		public IAssetProvider LaunchAsset(string file, IAssetProvider parent)
		{
			IVirtualSpace virtualSpace = Context.Get<IVirtualSpace>();
			string text = null;
			if (parent != null && parent.Asset != null)
			{
				string name = Path.Combine(Path.GetDirectoryName(parent.Asset), file);
				IVirtualFile virtualFile = virtualSpace.FindItem(name) as IVirtualFile;
				if (virtualFile != null)
				{
					text = virtualFile.FullPath;
				}
			}
			if (text == null)
			{
				text = virtualSpace.FindFullPath(file);
			}
			if (string.IsNullOrEmpty(text))
			{
				ExceptionLogger.Log("Missing file", "Attempt to launch a missing file: <br>" + file, ValidationResultLevel.Error);
				MessageBox.Show(string.Format("Unable to launch asset '{0}'. Check path location", Path.GetFileName(file)), "Launch Asset", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return null;
			}
			string fileName = Path.GetFileName(text);
			IAssetProvider assetProvider = Assets.FindByAsset(fileName);
			if (assetProvider != null)
			{
				assetProvider.ParentAsset = parent;
				assetProvider.ShowIt();
				return assetProvider;
			}
			IMaker maker = Factory.FindByExt(fileName);
			if (maker != null)
			{
				assetProvider = (IAssetProvider)maker.Make();
				assetProvider.ShowIt();
				assetProvider.SetAsset(text, parent);
				return assetProvider;
			}
			return null;
		}
	}
}
