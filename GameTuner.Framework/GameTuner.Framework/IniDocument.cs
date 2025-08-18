using System.Runtime.InteropServices;
using System.Text;

namespace GameTuner.Framework
{
	public class IniDocument
	{
		private string fileName;

		public IniDocument(string fileName)
		{
			this.fileName = fileName;
		}

		public static IniDocument FromFile(string fileName)
		{
			return new IniDocument(fileName);
		}

		public string GetValue(string section, string key, string defaultValue)
		{
			StringBuilder stringBuilder = new StringBuilder(255);
			GetPrivateProfileString(section, key, defaultValue, stringBuilder, 255, fileName);
			return stringBuilder.ToString();
		}

		public string GetValue(string section, string key)
		{
			return GetValue(section, key, "");
		}

		public void SetValue(string section, string key, string value)
		{
			WritePrivateProfileString(section, key, value, fileName);
		}

		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
	}
}
