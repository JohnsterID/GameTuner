using System;
using System.ComponentModel;
using System.Globalization;

namespace GameTuner.Framework.Converters
{
	public class Vec2Converter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType != typeof(string))
			{
				return base.CanConvertFrom(context, sourceType);
			}
			return true;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType != typeof(string))
			{
				return base.CanConvertTo(context, destinationType);
			}
			return true;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string[] array = ((string)value).Split(new char[2] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
				return new Vec2(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]));
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				Vec2 vec = (Vec2)value;
				return string.Format("{0}, {1}", vec.X, vec.Y);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
