using System;
using System.Collections.Generic;

namespace GameTuner.Framework
{
	public class ListEvent<T> : List<T>
	{
		public class ListEventArgs : EventArgs
		{
			public T Item { get; private set; }

			public IEnumerable<T> Collection { get; private set; }

			public ListEventArgs()
			{
			}

			public ListEventArgs(T item)
			{
				Item = item;
			}

			public ListEventArgs(IEnumerable<T> collection)
			{
				Collection = collection;
			}
		}

		public delegate void ListEventHandler(object sender, ListEventArgs e);

		public object LockObject { get; private set; }

		public event ListEventHandler AddedItem;

		public event ListEventHandler RemovedItem;

		public event ListEventHandler ClearedItems;

		public event ListEventHandler ItemCountChanged;

		public ListEvent()
		{
			LockObject = new object();
		}

		public ListEvent(IEnumerable<T> collection)
			: base(collection)
		{
			LockObject = new object();
		}

		public ListEvent(int capacity)
			: base(capacity)
		{
			LockObject = new object();
		}

		public new void AddRange(IEnumerable<T> collection)
		{
			lock (LockObject)
			{
				base.AddRange(collection);
				ListEventHandler addedItem = this.AddedItem;
				if (addedItem != null)
				{
					addedItem(this, new ListEventArgs(collection));
				}
				NotifyCountChanged();
			}
		}

		private void NotifyCountChanged()
		{
			ListEventHandler itemCountChanged = this.ItemCountChanged;
			if (itemCountChanged != null)
			{
				itemCountChanged(this, new ListEventArgs());
			}
		}

		public new void Add(T item)
		{
			lock (LockObject)
			{
				base.Add(item);
				ListEventHandler addedItem = this.AddedItem;
				if (addedItem != null)
				{
					addedItem(this, new ListEventArgs(item));
				}
				NotifyCountChanged();
			}
		}

		public new void Remove(T item)
		{
			lock (LockObject)
			{
				base.Remove(item);
				ListEventHandler removedItem = this.RemovedItem;
				if (removedItem != null)
				{
					removedItem(this, new ListEventArgs(item));
				}
				NotifyCountChanged();
			}
		}

		public new void Clear()
		{
			lock (LockObject)
			{
				base.Clear();
				ListEventHandler clearedItems = this.ClearedItems;
				if (clearedItems != null)
				{
					clearedItems(this, new ListEventArgs());
				}
				NotifyCountChanged();
			}
		}
	}
}
