using System.Collections.Generic;
using System.Reflection;
using Microsoft.Win32;

namespace GameTuner.Framework
{
	public static class RegistryObject
	{
		public static void Save(object obj, string regKey)
		{
			RegistryKey registryKey = null;
			List<PropertyInfo> list = new List<PropertyInfo>(ReflectionHelper.CollectProperties(obj));
			try
			{
				registryKey = Registry.CurrentUser.CreateSubKey(regKey);
				if (registryKey == null)
				{
					return;
				}
				foreach (PropertyInfo item in list)
				{
					registryKey.SetValue(item.Name, Transpose.ToString(item.GetValue(obj, null), item.PropertyType));
				}
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
		}

		public static void Load(object obj, string regKey)
		{
			RegistryKey registryKey = null;
			List<PropertyInfo> list = new List<PropertyInfo>(ReflectionHelper.CollectProperties(obj));
			try
			{
				registryKey = Registry.CurrentUser.OpenSubKey(regKey, false);
				if (registryKey == null)
				{
					return;
				}
				foreach (PropertyInfo item in list)
				{
					object value = registryKey.GetValue(item.Name, null);
					if (value != null)
					{
						item.SetValue(obj, Transpose.FromString((string)value, item.PropertyType), null);
					}
				}
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
		}
	}
}
