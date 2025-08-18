using System.Collections.Generic;

namespace GameTuner.Framework.Graph
{
	[UndoStyle(UndoStyle.Delete)]
	public class UndoGraphDelete : IUndo
	{
		private List<UndoHelper.UndoInfo> store;

		public UndoGraphDelete(IGraph graph, IEnumerable<object> items)
		{
			List<object> list = new List<object>();
			list.AddRange(items);
			foreach (object item in items)
			{
				UndoHelper.AddGraphDependents(item, list);
			}
			list.Sort(UndoHelper.UndoGraphDependentSort);
			store = new List<UndoHelper.UndoInfo>();
			foreach (object item2 in list)
			{
				FlowGraphNodeStyleAttribute attribute = ReflectionHelper.GetAttribute<FlowGraphNodeStyleAttribute>(item2);
				if (attribute == null || (attribute.Style & FlowGraphNodeStyle.NoDelete) == 0)
				{
					store.Add(new UndoHelper.UndoInfo(graph, item2));
				}
			}
		}

		public void PerformUndo()
		{
			foreach (UndoHelper.UndoInfo item in store.ReverseIterator())
			{
				object obj = item.Descriptor.Create();
				UndoHelper.ApplyFields(obj, item.Fields);
				item.Descriptor.Add(obj);
			}
		}

		public void PerformRedo()
		{
			store.ForEach(delegate(UndoHelper.UndoInfo a)
			{
				a.Descriptor.Remove();
			});
		}

		public void StoreUndo()
		{
		}

		public void StoreRedo()
		{
		}
	}
}
