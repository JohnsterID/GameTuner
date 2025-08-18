using System.Collections.Generic;

namespace GameTuner.Framework.Trigger
{
	[UndoStyle(UndoStyle.Property)]
	public class UndoTriggerFields : IUndo
	{
		private List<UndoTriggerField> undoFields;

		public UndoTriggerFields(List<ITrigger> triggers, string field, object oldValue)
		{
			undoFields = new List<UndoTriggerField>();
			if (oldValue is List<object>)
			{
				List<object> list = (List<object>)oldValue;
				int count = triggers.Count;
				for (int i = 0; i < count; i++)
				{
					undoFields.Add(new UndoTriggerField(triggers[i], field, list[i]));
				}
				return;
			}
			foreach (ITrigger trigger in triggers)
			{
				undoFields.Add(new UndoTriggerField(trigger, field, oldValue));
			}
		}

		public void PerformUndo()
		{
			foreach (UndoTriggerField undoField in undoFields)
			{
				undoField.PerformUndo();
			}
		}

		public void PerformRedo()
		{
			foreach (UndoTriggerField undoField in undoFields)
			{
				undoField.PerformRedo();
			}
		}

		public void StoreUndo()
		{
			foreach (UndoTriggerField undoField in undoFields)
			{
				undoField.StoreUndo();
			}
		}

		public void StoreRedo()
		{
			foreach (UndoTriggerField undoField in undoFields)
			{
				undoField.StoreRedo();
			}
		}
	}
}
