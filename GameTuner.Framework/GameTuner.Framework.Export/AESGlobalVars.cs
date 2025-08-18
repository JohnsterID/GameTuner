using System.Xml.XPath;

namespace GameTuner.Framework.Export
{
	public class AESGlobalVars
	{
		private static string[] m_aszAnimExportSettings;

		public static string[] AnimExportSettings
		{
			get
			{
				if (m_aszAnimExportSettings == null)
				{
					ProjectConfig projectConfig = Context.Get<ProjectConfig>();
					XPathNavigator xPathNavigator = projectConfig.CurrentProjectNode.CreateNavigator();
					XPathNodeIterator xPathNodeIterator = xPathNavigator.SelectDescendants("AnimSettings", "", false);
					m_aszAnimExportSettings = new string[xPathNodeIterator.Count];
					int num = 0;
					while (xPathNodeIterator.MoveNext())
					{
						m_aszAnimExportSettings[num++] = xPathNodeIterator.Current.GetAttribute("name", "");
					}
				}
				return m_aszAnimExportSettings;
			}
		}
	}
}
