using System.Collections.Generic;

namespace GameTuner.Framework.Graph
{
	[UndoStyle(UndoStyle.Property)]
	public class UndoGraphFields : IUndo
	{
		private class GraphItem
		{
			public GraphItemDescriptor Store;

			public List<UndoField> UndoFields;

			public List<UndoField> RedoFields;

			public GraphItem(IGraph graph, object item)
			{
				Store = new GraphItemDescriptor(graph, item);
				UndoFields = UndoHelper.AcquireFields(item);
			}

			public void PerformUndo()
			{
				Apply(UndoFields);
			}

			public void PerformRedo()
			{
				Apply(RedoFields);
			}

			private void Apply(List<UndoField> fields)
			{
				switch (Store.Type)
				{
				case GraphItemType.Node:
					UndoHelper.ApplyFields(Store.Node, fields);
					break;
				case GraphItemType.Socket:
					UndoHelper.ApplyFields(Store.Socket, fields);
					break;
				case GraphItemType.Nub:
					UndoHelper.ApplyFields(Store.Nub, fields);
					break;
				}
			}

			public void StoreRedo()
			{
				switch (Store.Type)
				{
				case GraphItemType.Node:
					RedoFields = UndoHelper.AcquireFields(Store.Node);
					break;
				case GraphItemType.Socket:
					RedoFields = UndoHelper.AcquireFields(Store.Socket);
					break;
				case GraphItemType.Nub:
					RedoFields = UndoHelper.AcquireFields(Store.Nub);
					break;
				}
			}
		}

		private bool allFields;

		private List<GraphItemDescriptor> store;

		private List<GraphItem> storeItems;

		private string field;

		private string oldValue;

		private string newValue;

		public UndoGraphFields(IGraph graph, List<object> items)
		{
			allFields = true;
			storeItems = new List<GraphItem>();
			foreach (object item in items)
			{
				storeItems.Add(new GraphItem(graph, item));
			}
		}

		public UndoGraphFields(IGraph graph, List<object> items, string field, object oldValue)
		{
			allFields = false;
			store = new List<GraphItemDescriptor>();
			this.field = field;
			this.oldValue = Transpose.ToString(oldValue);
			foreach (object item in items)
			{
				store.Add(new GraphItemDescriptor(graph, item));
			}
		}

		public void PerformUndo()
		{
			if (allFields)
			{
				foreach (GraphItem storeItem in storeItems)
				{
					storeItem.PerformUndo();
				}
				return;
			}
			foreach (GraphItemDescriptor item in store)
			{
				switch (item.Type)
				{
				case GraphItemType.Node:
					UndoHelper.ApplyValue(item.Node, field, oldValue);
					break;
				case GraphItemType.Socket:
					UndoHelper.ApplyValue(item.Socket, field, oldValue);
					break;
				case GraphItemType.Nub:
					UndoHelper.ApplyValue(item.Nub, field, oldValue);
					break;
				}
			}
		}

		public void PerformRedo()
		{
			if (allFields)
			{
				foreach (GraphItem storeItem in storeItems)
				{
					storeItem.PerformRedo();
				}
				return;
			}
			foreach (GraphItemDescriptor item in store)
			{
				switch (item.Type)
				{
				case GraphItemType.Node:
					UndoHelper.ApplyValue(item.Node, field, newValue);
					break;
				case GraphItemType.Socket:
					UndoHelper.ApplyValue(item.Socket, field, newValue);
					break;
				case GraphItemType.Nub:
					UndoHelper.ApplyValue(item.Nub, field, newValue);
					break;
				}
			}
		}

		public void StoreUndo()
		{
		}

		public void StoreRedo()
		{
			if (allFields)
			{
				foreach (GraphItem storeItem in storeItems)
				{
					storeItem.StoreRedo();
				}
				return;
			}
			GraphItemDescriptor graphItemDescriptor = store[0];
			switch (graphItemDescriptor.Type)
			{
			case GraphItemType.Node:
				newValue = UndoHelper.AcquireValue(graphItemDescriptor.Node, field);
				break;
			case GraphItemType.Socket:
				newValue = UndoHelper.AcquireValue(graphItemDescriptor.Socket, field);
				break;
			case GraphItemType.Nub:
				newValue = UndoHelper.AcquireValue(graphItemDescriptor.Nub, field);
				break;
			}
		}
	}
}
