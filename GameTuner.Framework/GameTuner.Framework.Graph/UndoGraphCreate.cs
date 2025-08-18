using System;
using System.Collections.Generic;

namespace GameTuner.Framework.Graph
{
	[UndoStyle(UndoStyle.Create)]
	public class UndoGraphCreate : IUndo
	{
		private List<UndoHelper.UndoInfo> store;

		public UndoGraphCreate(IGraph graph, object item)
		{
			UndoGraphCreate undoGraphCreate = this;
			store = new List<UndoHelper.UndoInfo>();
			List<object> list = new List<object>();
			UndoHelper.AddGraphDependents(item, list);
			list.Sort(UndoHelper.UndoGraphDependentSort);
			Action<object> action = delegate(object a)
			{
				undoGraphCreate.store.Add(new UndoHelper.UndoInfo(graph, a));
			};
			list.ForEach(action);
		}

		public void PerformUndo()
		{
			store.ForEach(delegate(UndoHelper.UndoInfo a)
			{
				a.Descriptor.Remove();
			});
		}

		public void PerformRedo()
		{
			foreach (UndoHelper.UndoInfo item in store.ReverseIterator())
			{
				object obj = item.Descriptor.Create();
				UndoHelper.ApplyFields(obj, item.Fields);
				item.Descriptor.Add(obj);
			}
		}

		public void StoreUndo()
		{
		}

		public void StoreRedo()
		{
		}
	}
}
