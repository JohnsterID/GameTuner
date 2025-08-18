using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GameTuner.Framework
{
	public class PluginFactory
	{
		private Type pluginType;

		private List<Type> plugins;

		public Type PluginType
		{
			get
			{
				return pluginType;
			}
		}

		public List<Type> Plugins
		{
			get
			{
				return plugins;
			}
		}

		public PluginFactory(Type pluginType)
		{
			if (!pluginType.IsInterface)
			{
				throw new InvalidCastException("Plugin type must be an interface");
			}
			this.pluginType = pluginType;
			plugins = new List<Type>();
		}

		public Type Find(string name)
		{
			return plugins.Find((Type type) => string.Compare(name, type.FullName) == 0);
		}

		public Type FindByDisplayName(string name)
		{
			return plugins.Find((Type type) => string.Compare(name, ReflectionHelper.GetDisplayName(type)) == 0);
		}

		public void ScanAssembly(Assembly assembly)
		{
			if (assembly == null)
			{
				return;
			}
			try
			{
				Type[] types = assembly.GetTypes();
				foreach (Type type in types)
				{
					if (!type.IsAbstract && type.IsClass && pluginType.IsAssignableFrom(type))
					{
						plugins.Add(type);
					}
				}
			}
			catch (Exception e)
			{
				ExceptionLogger.Log(e);
			}
		}

		public void ScanDirectory(string path, string searchPattern)
		{
			string[] files = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
			string[] array = files;
			foreach (string path2 in array)
			{
				try
				{
					if (File.Exists(path2))
					{
						Assembly assembly = Assembly.LoadFile(path2);
						ScanAssembly(assembly);
					}
				}
				catch (FileLoadException)
				{
				}
				catch (FileNotFoundException)
				{
				}
				catch (BadImageFormatException)
				{
				}
			}
		}
	}
}
