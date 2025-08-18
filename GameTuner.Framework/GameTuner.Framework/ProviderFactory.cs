using System.Collections.Generic;
using System.IO;

namespace GameTuner.Framework
{
	public class ProviderFactory : Factory<IAssetProvider>
	{
		public IMaker FindByExt(string name)
		{
			string extension = Path.GetExtension(name);
			if (!string.IsNullOrEmpty(extension))
			{
				using (Enumerator enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IMaker current = enumerator.Current;
						AssetTypeAttribute attribute = ReflectionHelper.GetAttribute<AssetTypeAttribute>(current.Type);
						if (attribute != null && string.Compare(extension, attribute.Extension, true) == 0)
						{
							return current;
						}
					}
				}
			}
			return null;
		}
	}
}
