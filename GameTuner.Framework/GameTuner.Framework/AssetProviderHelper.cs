using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public static class AssetProviderHelper
	{
		public static void SaveAll(IAssetProvider provider, string caption)
		{
			IAssetTargetCollector assetTargetCollector = Context.Get<IAssetTargetCollector>();
			List<IAssetProvider> list = new List<IAssetProvider>();
			list.Add(provider);
			foreach (IAssetTarget assetTarget in assetTargetCollector.AssetTargets)
			{
				if (assetTarget is IAssetProvider && assetTarget != provider && (assetTarget == provider.ParentAsset || assetTarget.ParentAsset == provider || (assetTarget.ParentAsset != null && assetTarget.ParentAsset == provider.ParentAsset)))
				{
					list.Add((IAssetProvider)assetTarget);
				}
			}
			foreach (IAssetProvider item in list)
			{
				if (item.Modified)
				{
					item.ShowIt();
				}
				Save(item, caption, SaveMethod.Save);
			}
			provider.ShowIt();
		}

		public static void Save(IAssetProvider provider, string caption, SaveMethod method)
		{
			if (provider == null || (!provider.Modified && method == SaveMethod.Save))
			{
				return;
			}
			if (method == SaveMethod.SaveAs || string.IsNullOrEmpty(provider.Asset))
			{
				FilterAttribute attribute = ReflectionHelper.GetAttribute<FilterAttribute>(provider);
				AssetTypeAttribute attribute2 = ReflectionHelper.GetAttribute<AssetTypeAttribute>(provider);
				string text = (string.IsNullOrEmpty(provider.Asset) ? "" : provider.Asset);
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Title = caption;
				saveFileDialog.Filter = ((attribute != null) ? attribute.Filter : "All Files (*.*)|*.*");
				saveFileDialog.DefaultExt = ((attribute2 != null) ? attribute2.Extension : "");
				saveFileDialog.FileName = Path.GetFileName(text);
				saveFileDialog.InitialDirectory = (string.IsNullOrEmpty(text) ? "" : Path.GetDirectoryName(text));
				DialogResult dialogResult = saveFileDialog.ShowDialog();
				if (dialogResult == DialogResult.OK)
				{
					provider.SaveAs(saveFileDialog.FileName);
				}
			}
			else
			{
				provider.Save();
			}
		}

		public static bool SaveModified(IAssetProvider provider, string caption)
		{
			bool result = true;
			if (provider != null && provider.Modified)
			{
				string fileName = Path.GetFileName(provider.Asset);
				switch (MessageBox.Show(fileName + " has been modified. Save changes?", caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
				{
				case DialogResult.Yes:
					try
					{
						Save(provider, caption, SaveMethod.Save);
					}
					catch (Exception e)
					{
						ExceptionLogger.Log(e);
						MessageBox.Show("Error saving document", caption, MessageBoxButtons.OK, MessageBoxIcon.Hand);
						result = false;
					}
					if (provider.Modified)
					{
						result = false;
					}
					break;
				case DialogResult.Cancel:
					result = false;
					break;
				}
			}
			return result;
		}
	}
}
