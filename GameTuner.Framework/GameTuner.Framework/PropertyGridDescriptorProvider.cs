using System;

namespace GameTuner.Framework
{
	public class PropertyGridDescriptorProvider
	{
		private IPropertyGridDescriptor current;

		private ListEvent<IPropertyGridDescriptor> descriptors;

		public ListEvent<IPropertyGridDescriptor> Descriptors
		{
			get
			{
				return descriptors;
			}
		}

		public IPropertyGridDescriptor ActivePropertyGridDescriptor
		{
			get
			{
				return current;
			}
			set
			{
				current = value;
				EventHandler activeDescriptorChanged = this.ActiveDescriptorChanged;
				if (activeDescriptorChanged != null)
				{
					activeDescriptorChanged(this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler ActiveDescriptorChanged;

		public PropertyGridDescriptorProvider()
		{
			descriptors = new ListEvent<IPropertyGridDescriptor>();
			descriptors.ClearedItems += descriptors_OnClear;
			descriptors.RemovedItem += descriptors_OnRemoveItem;
		}

		private void descriptors_OnRemoveItem(object sender, ListEvent<IPropertyGridDescriptor>.ListEventArgs e)
		{
			if (current == e.Item)
			{
				ActivePropertyGridDescriptor = null;
			}
		}

		private void descriptors_OnClear(object sender, EventArgs e)
		{
			ActivePropertyGridDescriptor = null;
		}
	}
}
