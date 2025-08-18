using System;
using System.Reflection;

namespace GameTuner.Framework
{
	public class DataNameAttribute : Attribute
	{
		private string name;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public DataNameAttribute(string name)
		{
			this.name = name;
		}

		public static string GetDataName<T>(T t)
		{
			FieldInfo field = typeof(T).GetField(t.ToString());
			object[] customAttributes = field.GetCustomAttributes(typeof(DataNameAttribute), true);
			if (customAttributes != null && customAttributes.GetLength(0) > 0)
			{
				return ((DataNameAttribute)customAttributes[0]).Name;
			}
			return "";
		}

		public static T GetDataNameType<T>(string t)
		{
			Array values = Enum.GetValues(typeof(T));
			foreach (object item in values)
			{
				if (string.Compare(t, GetDataName((T)item)) == 0)
				{
					return (T)item;
				}
			}
			return default(T);
		}
	}
}
