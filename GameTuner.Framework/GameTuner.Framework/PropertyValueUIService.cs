using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;

namespace GameTuner.Framework
{
	public class PropertyValueUIService : IPropertyValueUIService
	{
		private PropertyValueUIHandler valueUIHandler;

		private EventHandler notifyHandler;

		public event EventHandler PropertyUIValueItemsChanged
		{
			add
			{
				lock (this)
				{
					notifyHandler = (EventHandler)Delegate.Combine(notifyHandler, value);
				}
			}
			remove
			{
				lock (this)
				{
					notifyHandler = (EventHandler)Delegate.Remove(notifyHandler, value);
				}
			}
		}

		public void AddPropertyValueUIHandler(PropertyValueUIHandler newHandler)
		{
			if (newHandler == null)
			{
				throw new ArgumentNullException();
			}
			lock (this)
			{
				valueUIHandler = (PropertyValueUIHandler)Delegate.Combine(valueUIHandler, newHandler);
			}
		}

		public void NotifyPropertyValueUIItemsChanged()
		{
			if (notifyHandler != null)
			{
				notifyHandler(this, EventArgs.Empty);
			}
		}

		public void RemovePropertyValueUIHandler(PropertyValueUIHandler newHandler)
		{
			if (newHandler == null)
			{
				throw new ArgumentNullException();
			}
			valueUIHandler = (PropertyValueUIHandler)Delegate.Remove(valueUIHandler, newHandler);
		}

		public PropertyValueUIItem[] GetPropertyUIValueItems(ITypeDescriptorContext context, PropertyDescriptor propDesc)
		{
			if (propDesc == null)
			{
				throw new ArgumentNullException();
			}
			if (valueUIHandler == null)
			{
				return new PropertyValueUIItem[0];
			}
			lock (this)
			{
				ArrayList arrayList = new ArrayList();
				valueUIHandler(context, propDesc, arrayList);
				return (PropertyValueUIItem[])arrayList.ToArray(typeof(PropertyValueUIItem));
			}
		}
	}
}
