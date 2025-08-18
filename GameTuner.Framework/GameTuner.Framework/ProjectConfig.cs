using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using GameTuner.Framework.Properties;
using Microsoft.Win32;

namespace GameTuner.Framework
{
	public class ProjectConfig
	{
		public XmlDocument Xml { get; private set; }

		public List<string> Projects { get; private set; }

		public ProjectPathMappingInfoDictionary PathMappings { get; private set; }

		public IEnumerable<string> MappedPaths
		{
			get
			{
				foreach (PathMappingInfoDictionary kPMID in PathMappings.Values)
				{
					foreach (PathMappingInfo value in kPMID.Values)
					{
						yield return value.DepotPath;
					}
				}
			}
		}

		public PathMappingInfoDictionary ProjectPathMappings
		{
			get
			{
				return PathMappings[CurrentProjectName];
			}
		}

		public IEnumerable<string> ProjectMappedPaths
		{
			get
			{
				foreach (PathMappingInfo value in ProjectPathMappings.Values)
				{
					yield return value.DepotPath;
				}
			}
		}

		public string OptionsRegistryKeyName
		{
			get
			{
				return string.Format("{0}\\{1}", GameTuner.Framework.Properties.Resources.ProjectsRegKey, CurrentProjectName);
			}
		}

		public RegistryKey OptionsRegistryKey
		{
			get
			{
				RegistryKey registryKey = null;
				registryKey = Registry.CurrentUser.OpenSubKey(OptionsRegistryKeyName, true);
				if (registryKey != null)
				{
					return registryKey;
				}
				return Registry.CurrentUser.CreateSubKey(OptionsRegistryKeyName);
			}
		}

		public static bool RegistryKeyExists
		{
			get
			{
				RegistryKey registryKey = null;
				bool result = true;
				try
				{
					registryKey = Registry.LocalMachine.OpenSubKey(GameTuner.Framework.Properties.Resources.ConfigRegKey, false);
					result = registryKey != null;
				}
				catch
				{
					result = false;
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

		public Version Version
		{
			get
			{
				if (Xml == null)
				{
					return null;
				}
				XmlNodeList elementsByTagName = Xml.GetElementsByTagName("GameTunerProjects");
				foreach (XmlNode item in elementsByTagName)
				{
					foreach (XmlAttribute attribute in item.Attributes)
					{
						if (attribute.Name == "version")
						{
							return new Version(attribute.Value);
						}
					}
				}
				return null;
			}
		}

		public string DefaultProjectName
		{
			get
			{
				if (Xml != null)
				{
					XmlDoc xmlDoc = new XmlDoc(Xml);
					XmlNode node;
					if ((node = xmlDoc.Find("GameTunerProject")) != null)
					{
						return xmlDoc.GetAttrib(node, "name");
					}
				}
				return "";
			}
		}

		public string CurrentProjectName
		{
			get
			{
				RegistryKey registryKey = null;
				registryKey = Registry.CurrentUser.OpenSubKey(GameTuner.Framework.Properties.Resources.ProjectsRegKey, false);
				if (registryKey != null)
				{
					return registryKey.GetValue("CurrentProjectName", DefaultProjectName) as string;
				}
				return DefaultProjectName;
			}
			set
			{
				RegistryKey registryKey = null;
				registryKey = Registry.CurrentUser.OpenSubKey(GameTuner.Framework.Properties.Resources.ProjectsRegKey, true);
				if (registryKey == null)
				{
					registryKey = Registry.CurrentUser.CreateSubKey(GameTuner.Framework.Properties.Resources.ProjectsRegKey);
				}
				registryKey.SetValue("CurrentProjectName", value);
			}
		}

		public XmlNode CurrentProjectNode
		{
			get
			{
				if (Xml != null)
				{
					XmlDoc xmlDoc = new XmlDoc(Xml);
					string currentProjectName = CurrentProjectName;
					for (XmlNode xmlNode = xmlDoc.Find("GameTunerProject"); xmlNode != null; xmlNode = xmlDoc.Sibling(xmlNode))
					{
						if (string.Compare(xmlDoc.GetAttrib(xmlNode, "name"), currentProjectName, true) == 0)
						{
							return xmlNode;
						}
					}
				}
				return null;
			}
		}

		public ProjectConfig()
		{
			Projects = new List<string>();
			PathMappings = new ProjectPathMappingInfoDictionary();
			Xml = new XmlDocument();
		}

		public bool Init()
		{
			if (Available.Enterprise)
			{
				return InitEnterprise();
			}
			return true;
		}

		public bool Init(XmlDocument kDocument)
		{
			if (Xml.ChildNodes.Count > 0)
			{
				XPathNavigator xPathNavigator = Xml.CreateNavigator();
				XPathNodeIterator xPathNodeIterator = xPathNavigator.Select("/GameTunerProjects");
				xPathNodeIterator.MoveNext();
				XPathNavigator xPathNavigator2 = kDocument.CreateNavigator();
				XPathNodeIterator xPathNodeIterator2 = xPathNavigator2.Select("//GameTunerProject");
				while (xPathNodeIterator2.MoveNext())
				{
					if (xPathNodeIterator2.Current.IsNode)
					{
						xPathNodeIterator.Current.AppendChild(xPathNodeIterator2.Current);
					}
				}
			}
			else
			{
				Xml = kDocument;
			}
			return true;
		}

		private bool InitEnterprise()
		{
			RegistryKey registryKey = null;
			bool flag = true;
			try
			{
				registryKey = Registry.LocalMachine.OpenSubKey(GameTuner.Framework.Properties.Resources.ConfigRegKey, false);
				if (registryKey != null)
				{
					string filename = (string)registryKey.GetValue(GameTuner.Framework.Properties.Resources.ConfigXmlName, "");
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(filename);
					Init(xmlDocument);
				}
			}
			catch (Exception e)
			{
				ExceptionLogger.Log(e, "Init config files");
				flag = false;
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
			if (flag)
			{
				PopulateLists();
			}
			return flag;
		}

		public PathMappingInfoDictionary GetProjectPaths()
		{
			PathMappingInfoDictionary pathMappingInfoDictionary = new PathMappingInfoDictionary();
			XmlNode currentProjectNode;
			if ((currentProjectNode = CurrentProjectNode) != null)
			{
				XmlDoc xmlDoc = new XmlDoc(Xml);
				XmlNode node;
				if ((node = xmlDoc.Find(currentProjectNode, "Perforce")) != null)
				{
					// Source control path mapping disabled - no P4API available
					// Skip Perforce path resolution
				}
			}
			return pathMappingInfoDictionary;
		}

		public T GetOption<T>(string szCategory, string szOption, T DefaultValue)
		{
			RegistryKey registryKey = OptionsRegistryKey.CreateSubKey(szCategory);
			if (registryKey == null)
			{
				return DefaultValue;
			}
			object value = registryKey.GetValue(szOption);
			if (value != null)
			{
				return (T)Convert.ChangeType(value, typeof(T));
			}
			return DefaultValue;
		}

		public void SetOption<T>(string szCategory, string szOption, T Value)
		{
			RegistryKey registryKey = OptionsRegistryKey.CreateSubKey(szCategory);
			registryKey.SetValue(szOption, Value);
		}

		public string ConvertPathLocation(string szPath, string szFromLocation, string szToLocation)
		{
			ISourceControl sourceControl = Context.Get<ISourceControl>();
			if (sourceControl == null)
			{
				return null;
			}
			PathMappingInfoDictionary pathMappingInfoDictionary = PathMappings[CurrentProjectName];
			if (!pathMappingInfoDictionary.ContainsKey(szFromLocation) || !pathMappingInfoDictionary.ContainsKey(szToLocation))
			{
				return null;
			}
			string text;
			string text2;
			if (sourceControl.IsSourceControlPath(szPath))
			{
				text = pathMappingInfoDictionary[szFromLocation].DepotPath;
				text2 = pathMappingInfoDictionary[szToLocation].DepotPath;
			}
			else
			{
				text = pathMappingInfoDictionary[szFromLocation].LocalPath;
				text2 = pathMappingInfoDictionary[szToLocation].LocalPath;
			}
			if (!szPath.StartsWith(text, StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			string text3 = szPath.Remove(0, text.Length);
			return text2 + text3;
		}

		public string GetControlledLocalPath(string mappedPathName)
		{
			SourceControl sourceControl = Context.Get<SourceControl>();
			if (sourceControl.IsConnected)
			{
				try
				{
					PathMappingInfo value;
					if (GetProjectPaths().TryGetValue(mappedPathName, out value))
					{
						SetLocalPath(CurrentProjectName, mappedPathName, value.LocalPath);
						return value.LocalPath;
					}
				}
				catch (Exception e)
				{
					ExceptionLogger.Log(e);
				}
			}
			return GetLocalPath(mappedPathName);
		}

		public void SetLocalPath(string project, string mappedPathName, string localPath)
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.CurrentUser.CreateSubKey(string.Format("{0}\\{1}", GameTuner.Framework.Properties.Resources.ProjectsRegKey, project));
				if (registryKey != null)
				{
					registryKey.SetValue(mappedPathName, localPath);
				}
			}
			catch (Exception e)
			{
				ExceptionLogger.Log(e, "Set Local Path");
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
		}

		public string GetLocalPath(string mappedPathName)
		{
			string currentProjectName = CurrentProjectName;
			string result = "";
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.CurrentUser.OpenSubKey(string.Format("{0}\\{1}", GameTuner.Framework.Properties.Resources.ProjectsRegKey, currentProjectName), false);
				if (registryKey != null)
				{
					result = (string)registryKey.GetValue(mappedPathName);
				}
			}
			catch (Exception e)
			{
				ExceptionLogger.Log(e, "Get Local Path");
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

		private void PopulateLists()
		{
			XmlDoc xmlDoc = new XmlDoc(Xml);
			SourceControl sourceControl = Context.Get<SourceControl>();
			bool isConnected = sourceControl.IsConnected;
			Projects.Clear();
			PathMappings.Clear();
			try
			{
				XmlNode node = xmlDoc.Find("GameTunerProjects");
				for (node = xmlDoc.Child(node, "GameTunerProject"); node != null; node = xmlDoc.Sibling(node, "GameTunerProject"))
				{
					string attrib = xmlDoc.GetAttrib(node, "name");
					PathMappingInfoDictionary pathMappingInfoDictionary = new PathMappingInfoDictionary();
					Projects.Add(attrib);
					PathMappings.Add(attrib, pathMappingInfoDictionary);
					if (isConnected)
					{
						XmlNode node2 = xmlDoc.Find(node, "Perforce");
						for (node2 = xmlDoc.Child(node2); node2 != null; node2 = xmlDoc.Sibling(node2))
						{
							string name = node2.Name;
							string value = node2.Attributes["path"].Value;
							if (value != null && !pathMappingInfoDictionary.ContainsKey(name))
							{
								string localPathFromDepot = sourceControl.GetLocalPathFromDepot(value);
								pathMappingInfoDictionary.Add(name, new PathMappingInfo(localPathFromDepot, value));
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				ExceptionLogger.Log(e, "PopulateLists");
			}
		}
	}
}
