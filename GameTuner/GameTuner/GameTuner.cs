using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using GameTuner.Framework;

namespace GameTuner
{
	internal static class GameTuner
	{
		private static bool bAppRunning = false;

		private static string m_sConnectedApp = string.Empty;

		private static string m_sDefaultPanelDir = null;

		private static string m_sLocalSettingsFolder = null;

		private static List<string> m_Admins = new List<string>();

		public static bool AppRunning
		{
			get
			{
				return bAppRunning;
			}
		}

		public static string ConnectedApp
		{
			get
			{
				return m_sConnectedApp;
			}
			set
			{
				m_sConnectedApp = value;
			}
		}

		public static string DefaultPanelDir
		{
			get
			{
				if (m_sDefaultPanelDir == null)
				{
					m_sDefaultPanelDir = Directory.GetCurrentDirectory();
				}
				return m_sDefaultPanelDir;
			}
			set
			{
				m_sDefaultPanelDir = value;
			}
		}

		public static string LocalSettingsFolder
		{
			get
			{
				if (m_sLocalSettingsFolder == null)
				{
					m_sLocalSettingsFolder = InitLocalSettingsFolder();
				}
				return m_sLocalSettingsFolder;
			}
		}

		public static List<string> Admins
		{
			get
			{
				return m_Admins;
			}
		}

		[STAThread]
		private static int Main(string[] args)
		{
			ErrorHandling.AppName = "GameTuner";
			ErrorHandling.AppVersion = ApplicationHelper.ProductVersion;
			// ErrorHandling.ErrorReportRecipients.Add("name@localhost");
			ErrorHandling.CatchUnhandledExceptions();
			Thread.CurrentThread.Name = "Main Thread";
			bool createdNew = false;
			Mutex mutex = new Mutex(true, "GameTuner", out createdNew);
			if (!createdNew)
			{
				Process currentProcess = Process.GetCurrentProcess();
				Process[] processesByName = Process.GetProcessesByName(currentProcess.ProcessName);
				Process[] array = processesByName;
				foreach (Process process in array)
				{
					if (process.Id != currentProcess.Id)
					{
						IntPtr mainWindowHandle = process.MainWindowHandle;
						if (NativeMethods.IsIconic(mainWindowHandle))
						{
							NativeMethods.ShowWindowAsync(mainWindowHandle, 9);
						}
						else
						{
							NativeMethods.SetForegroundWindow(mainWindowHandle);
						}
						break;
					}
				}
			}
			else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				bAppRunning = true;
				LoadSettings();
				Application.Run(new frmMainForm());
				SaveSettings();
				bAppRunning = false;
			}
			mutex.Close();
			return 0;
		}

		public static bool CheckForAdminPrivileges()
		{
			if (UserInfo.InHouse && !m_Admins.Contains(UserInfo.GetCurrent().FullName))
			{
				DialogResult dialogResult = MessageBox.Show("This is an administrative feature that you do not have permission to use.\nWould you like to use it anyway?", "Administrative Access Required", MessageBoxButtons.YesNo);
				return dialogResult == DialogResult.Yes;
			}
			return true;
		}

		private static UserSettings GetCurrentSettings()
		{
			UserSettings userSettings = new UserSettings();
			userSettings.ValueControlSettings = ValueControlBuilder.Settings;
			return userSettings;
		}

		private static void SetCurrentSettings(UserSettings settings)
		{
			ValueControlBuilder.Settings = settings.ValueControlSettings;
		}

		private static void SaveSettings()
		{
			UserSettings currentSettings = GetCurrentSettings();
			try
			{
				Stream stream = new FileStream(LocalSettingsFolder + "\\UserSettings.xml", FileMode.Create, FileAccess.Write);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserSettings));
				xmlSerializer.Serialize(stream, currentSettings);
				stream.Close();
			}
			catch (Exception ex)
			{
				ErrorHandling.Error(ex, "Unable to save user settings:\n" + ex.Message, ErrorLevel.ShowMessage);
			}
		}

		private static void LoadSettings()
		{
			try
			{
				string path = LocalSettingsFolder + "\\UserSettings.xml";
				if (File.Exists(path))
				{
					Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserSettings));
					UserSettings userSettings = (UserSettings)xmlSerializer.Deserialize(stream);
					stream.Close();
					if (userSettings != null)
					{
						SetCurrentSettings(userSettings);
					}
				}
			}
			catch (Exception ex)
			{
				ErrorHandling.Error(ex, "Error loading user settings:\n" + ex.Message, ErrorLevel.ShowMessage);
			}
		}

		private static string InitLocalSettingsFolder()
		{
			try
			{
				string text = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\GameTuner";
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				return text;
			}
			catch (Exception ex)
			{
				ErrorHandling.Error(ex, "Unable to create local settings folder: " + ex.Message, ErrorLevel.ShowMessage);
				return string.Empty;
			}
		}
	}
}
