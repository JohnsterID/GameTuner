using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameTuner.Framework
{
	public class PropertyGridProxy : ICustomTypeDescriptor
	{
		private class ProxyPropertyDescriptor : PropertyDescriptor
		{
			private PropertyGridProxy proxy;

			private PropertyDescriptor desc;

			public override Type ComponentType
			{
				get
				{
					return desc.ComponentType;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return desc.IsReadOnly;
				}
			}

			public override Type PropertyType
			{
				get
				{
					return desc.PropertyType;
				}
			}

			public ProxyPropertyDescriptor(PropertyGridProxy proxy, PropertyDescriptor desc, Attribute[] attribs)
				: base(desc.Name, attribs)
			{
				this.proxy = proxy;
				this.desc = desc;
			}

			public override bool CanResetValue(object component)
			{
				return desc.CanResetValue(component);
			}

			public override object GetValue(object component)
			{
				return desc.GetValue(component);
			}

			public override void ResetValue(object component)
			{
				desc.ResetValue(component);
			}

			public override void SetValue(object component, object value)
			{
				proxy.OnPropertyValueChanging(EventArgs.Empty);
				if (desc.Converter != null && !desc.PropertyType.IsInstanceOfType(value))
				{
					TypeConverter typeConverter = desc.Converter;
					desc.SetValue(component, typeConverter.ConvertFrom(value));
				}
				else
				{
					desc.SetValue(component, value);
				}
				proxy.OnPropertyValueChanged(EventArgs.Empty);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return desc.ShouldSerializeValue(component);
			}
		}

		private object obj;

		[Browsable(false)]
		public object Object
		{
			get
			{
				return obj;
			}
		}

		public event EventHandler PropertyValueChanging;

		public event EventHandler PropertyValueChanged;

		public PropertyGridProxy(object obj)
		{
			this.obj = obj;
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[0]);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
			foreach (PropertyDescriptor item2 in properties)
			{
				List<Attribute> list2 = new List<Attribute>();
				foreach (Attribute attribute in item2.Attributes)
				{
					list2.Add(attribute);
				}
				list.Add(new ProxyPropertyDescriptor(this, item2, list2.ToArray()));
			}
			return new PropertyDescriptorCollection(list.ToArray());
		}

		protected virtual void OnPropertyValueChanging(EventArgs e)
		{
			EventHandler propertyValueChanging = this.PropertyValueChanging;
			if (propertyValueChanging != null)
			{
				propertyValueChanging(this, e);
			}
		}

		protected virtual void OnPropertyValueChanged(EventArgs e)
		{
			EventHandler propertyValueChanged = this.PropertyValueChanged;
			if (propertyValueChanged != null)
			{
				propertyValueChanged(this, e);
			}
		}

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(obj, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(obj, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(obj, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(obj, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(obj, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(obj);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(obj, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(obj, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(obj, true);
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return obj;
		}
	}
}
