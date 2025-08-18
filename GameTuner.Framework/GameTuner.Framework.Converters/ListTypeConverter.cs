using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace GameTuner.Framework.Converters
{
	public class ListTypeConverter<T> : StringConverter
	{
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				List<T> list = new List<T>();
				using (StringReader stringReader = new StringReader((string)value))
				{
					string value2;
					while ((value2 = stringReader.ReadLine()) != null)
					{
						list.Add(Transpose.FromString<T>(value2));
					}
					return list;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				StringBuilder stringBuilder = new StringBuilder();
				List<T> list = (List<T>)value;
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					if (i != count - 1)
					{
						stringBuilder.AppendLine(list[i].ToString() + ",");
					}
					else
					{
						stringBuilder.AppendLine(list[i].ToString());
					}
				}
				return stringBuilder.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
