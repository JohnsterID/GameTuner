using System.Xml.XPath;

namespace GameTuner.Framework.Export
{
	public class MESGlobalVars
	{
		private static string[] m_aszModelExportSettings;

		public static string[] ModelExportSettings
		{
			get
			{
				if (m_aszModelExportSettings == null)
				{
					ProjectConfig projectConfig = Context.Get<ProjectConfig>();
					XPathNavigator xPathNavigator = projectConfig.CurrentProjectNode.CreateNavigator();
					XPathNodeIterator xPathNodeIterator = xPathNavigator.SelectDescendants("ModelSettings", "", false);
					m_aszModelExportSettings = new string[xPathNodeIterator.Count];
					int num = 0;
					while (xPathNodeIterator.MoveNext())
					{
						ModelExportSettings[num++] = xPathNodeIterator.Current.GetAttribute("name", "");
					}
				}
				return m_aszModelExportSettings;
			}
		}
	}
}
