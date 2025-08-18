using System;
using System.ComponentModel;
using System.Reflection;

namespace GameTuner.Framework.Converters
{
	public class FieldPropertyDescriptor : PropertyDescriptor
	{
		private PropertyInfo prop;

		public override Type ComponentType
		{
			get
			{
				return prop.DeclaringType;
			}
		}

		public PropertyInfo Field
		{
			get
			{
				return prop;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return prop.PropertyType;
			}
		}

		public FieldPropertyDescriptor(PropertyInfo prop)
			: base(prop.Name, (Attribute[])prop.GetCustomAttributes(typeof(Attribute), true))
		{
			this.prop = prop;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override bool Equals(object obj)
		{
			FieldPropertyDescriptor fieldPropertyDescriptor = obj as FieldPropertyDescriptor;
			if (fieldPropertyDescriptor != null)
			{
				return fieldPropertyDescriptor.prop.Equals(prop);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return prop.GetHashCode();
		}

		public override object GetValue(object component)
		{
			return prop.GetValue(component, null);
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
			prop.SetValue(component, value, null);
			OnValueChanged(component, EventArgs.Empty);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}
	}
}
