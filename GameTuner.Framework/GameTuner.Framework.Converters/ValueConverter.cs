using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace GameTuner.Framework.Converters
{
	public class ValueConverter : ExpandableObjectConverter
	{
		protected PropertyDescriptorCollection propertyDescriptions;

		protected bool supportStringConvert = true;

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (!supportStringConvert || sourceType != typeof(string))
			{
				return base.CanConvertFrom(context, sourceType);
			}
			return true;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType != typeof(InstanceDescriptor))
			{
				return base.CanConvertTo(context, destinationType);
			}
			return true;
		}

		public static string ConvertFromValues<T>(ITypeDescriptorContext context, CultureInfo culture, T[] values)
		{
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			string separator = culture.TextInfo.ListSeparator + " ";
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			string[] array = new string[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				array[i] = converter.ConvertToString(context, culture, values[i]);
			}
			return string.Join(separator, array);
		}

		public static T[] ConvertToValues<T>(ITypeDescriptorContext context, CultureInfo culture, object value, int arrayCount, string messageParam)
		{
			if (!(value is string))
			{
				return null;
			}
			string text = ((string)value).Trim();
			if (text.Length == 0)
			{
				return null;
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			char[] separator = culture.TextInfo.ListSeparator.ToCharArray();
			string[] array = text.Split(separator);
			T[] array2 = new T[array.Length];
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			for (int i = 0; i < array2.Length; i++)
			{
				try
				{
					array2[i] = (T)converter.ConvertFromString(context, culture, array[i]);
				}
				catch (Exception innerException)
				{
					throw new ArgumentException("Invalid String Format", innerException);
				}
			}
			if (array2.Length != arrayCount)
			{
				throw new ArgumentException("Invalid String Format");
			}
			return array2;
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return propertyDescriptions;
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
