using System.Windows.Forms;
using GameTuner.Framework.Properties;
using Microsoft.Win32;

namespace GameTuner.Framework
{
	public static class ApplicationHelper
	{
		public static string LocalUserCommonAppDataPath
		{
			get
			{
				string localUserAppDataPath = Application.LocalUserAppDataPath;
				return localUserAppDataPath.Remove(localUserAppDataPath.IndexOf(Application.ProductVersion) - 1);
			}
		}

		public static string ProductVersion
		{
			get
			{
				RegistryKey registryKey = null;
				string result = Application.ProductVersion;
				try
				{
					registryKey = Registry.CurrentUser.OpenSubKey(GameTuner.Framework.Properties.Resources.ToolsRegKey + "\\" + Application.ProductName);
					if (registryKey != null)
					{
						result = (string)registryKey.GetValue("Version");
					}
				}
				finally
				{
					if (registryKey != null)
					{
						registryKey.Close();
					}
				}
				return result;
			}
		}
	}
}
