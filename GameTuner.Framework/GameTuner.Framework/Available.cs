using System;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Security.Authentication;
using System.Windows.Forms;
using GameTuner.Framework.Properties;
using Microsoft.Win32;

namespace GameTuner.Framework
{
	public static class Available
	{
		public static bool Initialized { get; private set; }

		public static bool SourceControl { get; private set; }

		public static bool Enterprise { get; private set; }

		public static string ProjectName { get; private set; }

		public static bool EnableVirtualSpace { get; set; }

		static Available()
		{
			Initialized = false;
			SourceControl = false;
			Enterprise = false;
			ProjectName = null;
			EnableVirtualSpace = true;
		}

		public static void Startup()
		{
			Startup(null, false);
		}

		public static void Startup(string projectName)
		{
			Startup(projectName, false);
		}

		public static void Startup(string projectName, bool disconnected)
		{
			Initialized = true;
			if (disconnected)
			{
				Enterprise = false;
			}
			else
			{
				try
				{
					Domain computerDomain = Domain.GetComputerDomain();
					Enterprise = computerDomain != null && computerDomain.Name.CompareTo(GameTuner.Framework.Properties.Resources.Enterprise) == 0;
				}
				catch (ActiveDirectoryObjectNotFoundException)
				{
					Enterprise = false;
				}
				catch (AuthenticationException)
				{
					Enterprise = false;
				}
			}
			IVirtualSpace virtualSpace = null;
			if (EnableVirtualSpace)
			{
				virtualSpace = new VirtualSpace();
				Context.Add(virtualSpace);
			}
			Context.Add(new SourceControl());
			ProjectConfig projectConfig = new ProjectConfig();
			projectConfig.Init();
			Context.Add(projectConfig);
			if (Enterprise)
			{
				ISourceControl service = Context.GetService<ISourceControl>();
				try
				{
					SourceControl = Enterprise && service != null && service.IsConnected;
				}
				catch (Exception exception)
				{
					ErrorHandling.Error(exception, ErrorLevel.Log);
					SourceControl = false;
				}
				if (SourceControl)
				{
					string[] array = new string[2] { "Export", "Resource" };
					string[] array2 = array;
					foreach (string text in array2)
					{
						string controlledLocalPath = projectConfig.GetControlledLocalPath(text);
						if (virtualSpace != null && controlledLocalPath != null)
						{
							virtualSpace.BasePaths.Add(controlledLocalPath);
							SaveBasePath(Registry.CurrentUser, GameTuner.Framework.Properties.Resources.ToolsRegKey, "ToolAssetPath", text);
						}
					}
					ProjectName = projectConfig.CurrentProjectName;
				}
			}
			else
			{
				string text2 = null;
				if (!string.IsNullOrEmpty(projectName))
				{
					text2 = GetRegistryBasePath(Registry.CurrentUser, GameTuner.Framework.Properties.Resources.ToolsRegKey, "ToolAssetPath");
					if (string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(projectName))
					{
						ProjectName = projectName;
						text2 = GetRegistryBasePath(Registry.LocalMachine, string.Format(GameTuner.Framework.Properties.Resources.AssetsRegKey, projectName), "InstallPath");
					}
				}
				while ((EnableVirtualSpace && string.IsNullOrEmpty(text2)) || !Directory.Exists(text2))
				{
					PickAssetPathForm pickAssetPathForm = new PickAssetPathForm(null);
					pickAssetPathForm.ShowDialog();
					text2 = pickAssetPathForm.BasePath;
					if (string.IsNullOrEmpty(text2) || !Directory.Exists(text2))
					{
						MessageBox.Show("Invalid path specified. Application must have a directory to function.");
					}
					else
					{
						SaveBasePath(Registry.CurrentUser, GameTuner.Framework.Properties.Resources.ToolsRegKey, "ToolAssetPath", text2);
					}
				}
				SourceControl = false;
				if (virtualSpace != null)
				{
					virtualSpace.BasePaths.Add(Path.Combine(text2, "Assets"));
					virtualSpace.BasePaths.Add(Path.Combine(text2, "Resource"));
				}
			}
			if (virtualSpace != null)
			{
				virtualSpace.Refresh();
			}
		}

		private static void SaveBasePath(RegistryKey baseKey, string regKey, string keyValue, string basePath)
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = baseKey.CreateSubKey(regKey);
				if (registryKey != null)
				{
					registryKey.SetValue(keyValue, basePath);
				}
			}
			catch (Exception exception)
			{
				ErrorHandling.Error(exception, ErrorLevel.Log);
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
		}

		private static string GetRegistryBasePath(RegistryKey baseKey, string regKey, string keyValue)
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = baseKey.OpenSubKey(regKey);
				if (registryKey != null)
				{
					return (string)registryKey.GetValue(keyValue);
				}
			}
			catch (Exception exception)
			{
				ErrorHandling.Error(exception, ErrorLevel.Log);
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
			return null;
		}
	}
}
