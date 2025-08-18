using System;
using System.DirectoryServices;
using System.Security.Principal;

namespace GameTuner.Framework
{
	public class UserInfo
	{
		private static UserInfo m_CurrentUser;

		private string m_sLoginName = string.Empty;

		private string m_sDomainName = string.Empty;

		private string m_sUserName = string.Empty;

		private string m_sDisplayName = string.Empty;

		private string m_sFullName = string.Empty;

		private string m_sFirstName = string.Empty;

		private string m_sLastName = string.Empty;

		private string m_sDocumentsFolder = string.Empty;

		private WindowsIdentity m_Identity;

		public static bool InHouse
		{
			get
			{
				try
				{
					WindowsIdentity current = WindowsIdentity.GetCurrent();
					string text = string.Empty;
					string[] array = current.Name.Split('\\', '@');
					if (array.Length == 2)
					{
						text = array[0];
					}
					return text == "2KGAMES" && ProjectConfig.RegistryKeyExists;
				}
				catch
				{
					return false;
				}
			}
		}

		public string LoginName
		{
			get
			{
				return m_sLoginName;
			}
		}

		public string DomainName
		{
			get
			{
				return m_sDomainName;
			}
		}

		public string UserName
		{
			get
			{
				return m_sUserName;
			}
		}

		public string DisplayName
		{
			get
			{
				return m_sDisplayName;
			}
		}

		public string FullName
		{
			get
			{
				return m_sFullName;
			}
		}

		public string FirstName
		{
			get
			{
				return m_sFirstName;
			}
		}

		public string LastName
		{
			get
			{
				return m_sLastName;
			}
		}

		public string DocumentsFolder
		{
			get
			{
				return m_sDocumentsFolder;
			}
		}

		public WindowsIdentity Identity
		{
			get
			{
				return m_Identity;
			}
			private set
			{
				m_Identity = value;
				if (m_Identity != null)
				{
					try
					{
						m_sLoginName = m_Identity.Name;
						string[] array = m_sLoginName.Split('\\', '@');
						if (array.Length == 2)
						{
							m_sDomainName = array[0];
							m_sUserName = array[1];
						}
						else
						{
							m_sDomainName = Environment.MachineName;
							m_sUserName = m_sLoginName;
						}
						DirectoryEntry directoryEntry = new DirectoryEntry("WinNT://" + m_sDomainName + "/" + m_sUserName);
						m_sDisplayName = directoryEntry.Properties["FullName"].Value.ToString();
							m_sFullName = m_sDisplayName.Replace(" (GameTuner)", "");
						int num = m_sFullName.IndexOf(' ');
						m_sFirstName = ((num > 0) ? m_sFullName.Substring(0, num) : m_sFullName);
						m_sLastName = ((num > 0) ? m_sFullName.Substring(num + 1) : m_sFullName);
						m_sDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
						return;
					}
					catch (Exception exception)
					{
						ErrorHandling.Error(exception, "Failed to retrieve information from WindowsIdentity", ErrorLevel.Log);
						return;
					}
				}
				m_sLoginName = string.Empty;
				m_sDomainName = string.Empty;
				m_sUserName = string.Empty;
				m_sDisplayName = string.Empty;
				m_sFullName = string.Empty;
				m_sFirstName = string.Empty;
				m_sLastName = string.Empty;
			}
		}

		public static UserInfo GetCurrent()
		{
			if (m_CurrentUser == null)
			{
				return m_CurrentUser = new UserInfo(WindowsIdentity.GetCurrent());
			}
			return m_CurrentUser;
		}

		public UserInfo(WindowsIdentity identity)
		{
			Identity = identity;
		}
	}
}
