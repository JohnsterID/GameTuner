using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public static class ReflectionHelper
	{
		public delegate bool PropertyFilter(PropertyInfo p);

		public static bool Is64Bit
		{
			get
			{
				return IntPtr.Size == 8;
			}
		}

		public static T CreateInstance<T>()
		{
			return Activator.CreateInstance<T>();
		}

		public static object CreateInstance(Type type)
		{
			return Activator.CreateInstance(type);
		}

		public static object CreateInstance(string assembly, string name)
		{
			ObjectHandle objectHandle = Activator.CreateInstance(assembly, name);
			return objectHandle.Unwrap();
		}

		public static T GetAttribute<T>(Type type)
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(T), true);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				return (T)customAttributes[0];
			}
			return default(T);
		}

		public static T GetAttribute<T>(object obj)
		{
			Type type = obj.GetType();
			if (type.IsEnum)
			{
				FieldInfo field = type.GetField(obj.ToString());
				object[] customAttributes = field.GetCustomAttributes(typeof(T), true);
				if (customAttributes != null && customAttributes.GetLength(0) > 0)
				{
					return (T)customAttributes[0];
				}
			}
			return GetAttribute<T>(obj.GetType());
		}

		public static string GetDisplayName(Type type)
		{
			DisplayNameAttribute attribute = GetAttribute<DisplayNameAttribute>(type);
			if (attribute == null)
			{
				return type.Name;
			}
			return attribute.DisplayName;
		}

		public static string GetDisplayName(object obj)
		{
			DisplayNameAttribute attribute = GetAttribute<DisplayNameAttribute>(obj);
			if (attribute == null)
			{
				return obj.GetType().Name;
			}
			return attribute.DisplayName;
		}

		public static string GetDescription(Type type)
		{
			DescriptionAttribute attribute = GetAttribute<DescriptionAttribute>(type);
			if (attribute == null)
			{
				return "";
			}
			return attribute.Description;
		}

		public static string GetDescription(object obj)
		{
			DescriptionAttribute attribute = GetAttribute<DescriptionAttribute>(obj);
			if (attribute == null)
			{
				return "";
			}
			return attribute.Description;
		}

		public static int GetStride(Type type)
		{
			StrideAttribute attribute = GetAttribute<StrideAttribute>(type);
			if (attribute == null)
			{
				return 0;
			}
			return attribute.Size;
		}

		public static int GetStride(object obj)
		{
			StrideAttribute attribute = GetAttribute<StrideAttribute>(obj);
			if (attribute == null)
			{
				return 0;
			}
			return attribute.Size;
		}

		public static bool IsType<T>(Type type)
		{
			return typeof(T).IsAssignableFrom(type);
		}

		public static bool ValidEnumValue<T>(int value)
		{
			if (!typeof(T).IsEnum)
			{
				throw new InvalidCastException("Type is not an enum");
			}
			Array values = Enum.GetValues(typeof(T));
			foreach (int item in values)
			{
				if (item == value)
				{
					return true;
				}
			}
			return false;
		}

		public static T EnumType<T>(string name)
		{
			int result;
			if (int.TryParse(name, out result))
			{
				name = Enum.GetName(typeof(T), result);
			}
			return (T)Enum.Parse(typeof(T), name);
		}

		public static Color GetColor(Type type)
		{
			ColorProviderAttribute attribute = GetAttribute<ColorProviderAttribute>(type);
			if (attribute == null)
			{
				return Color.White;
			}
			return attribute.Color;
		}

		public static Color GetColor(object obj)
		{
			ColorProviderAttribute attribute = GetAttribute<ColorProviderAttribute>(obj);
			if (attribute == null)
			{
				return Color.White;
			}
			return attribute.Color;
		}

		public static Assembly ProxyAssembly(string baseAssemblyName)
		{
			return ProxyAssembly(baseAssemblyName, "Win32", "x64", ".dll");
		}

		public static Assembly ProxyAssembly(string baseAssemblyName, string x32, string x64, string ext)
		{
			string text = baseAssemblyName + (Is64Bit ? x64 : x32);
			if (!Path.IsPathRooted(text))
			{
				string directoryName = Path.GetDirectoryName(Application.ExecutablePath);
				text = Path.Combine(directoryName, text);
			}
			text += ext;
			return Assembly.LoadFile(text);
		}

		public static T TypeLoader<T>(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (!type.IsAbstract && type.IsClass && typeof(T).IsAssignableFrom(type))
				{
					return (T)Activator.CreateInstance(type);
				}
			}
			throw new IOException(string.Format("Module '{0}' does not contain an {1} object", assembly.FullName, typeof(T).Name));
		}

		public static IEnumerable<PropertyInfo> CollectProperties(object obj)
		{
			return CollectProperties(obj, PropertyFilterOnlyPubliclyModifiable);
		}

		private static bool PropertyFilterOnlyPubliclyModifiable(PropertyInfo p)
		{
			MethodInfo[] accessors = p.GetAccessors(false);
			if (p.CanRead && p.CanWrite && accessors != null)
			{
				return accessors.Length == 2;
			}
			return false;
		}

		public static IEnumerable<PropertyInfo> CollectProperties(object obj, PropertyFilter pfnFilterFunction)
		{
			if (obj == null)
			{
				throw new ArgumentNullException();
			}
			Type t = obj.GetType();
			if (!t.IsClass)
			{
				throw new ArgumentException("Object must be a class");
			}
			MemberInfo[] members = t.GetMembers();
			try
			{
				MemberInfo[] array = members;
				foreach (MemberInfo m in array)
				{
					PropertyInfo p = m as PropertyInfo;
					if (p == null)
					{
						continue;
					}
					object[] attrib = p.GetCustomAttributes(typeof(NoSerializeAttribute), true);
					if (attrib == null || attrib.Length <= 0)
					{
						attrib = p.GetCustomAttributes(typeof(NonSerializedAttribute), true);
						if ((attrib == null || attrib.Length <= 0) && (pfnFilterFunction == null || pfnFilterFunction(p)))
						{
							yield return p;
						}
					}
				}
			}
			finally
			{
			}
		}
	}
}
