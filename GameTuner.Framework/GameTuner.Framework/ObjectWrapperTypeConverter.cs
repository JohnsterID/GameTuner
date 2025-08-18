using System;
using System.ComponentModel;
using System.Globalization;

namespace GameTuner.Framework
{
	public class ObjectWrapperTypeConverter : TypeConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				if (value == null)
				{
					return "(null)";
				}
				return value.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return false;
			}
			return base.CanConvertFrom(context, sourceType);
		}
	}
}
