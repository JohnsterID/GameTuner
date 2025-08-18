using System.Collections.Generic;

namespace GameTuner.Framework.Trigger
{
	[UndoStyle(UndoStyle.Delete)]
	public class UndoTriggerDelete : IUndo
	{
		private ITriggerSystem trigsys;

		private int id;

		private string name;

		private List<UndoField> fields;

		public UndoTriggerDelete(ITrigger trigger)
		{
			trigsys = trigger.Owner;
			id = trigger.ID;
			DataNameAttribute attribute = ReflectionHelper.GetAttribute<DataNameAttribute>(trigger);
			name = ((attribute != null) ? attribute.Name : trigger.GetType().Name);
			fields = UndoHelper.AcquireFields(trigger);
		}

		public void PerformUndo()
		{
			ITrigger trigger = trigsys.TriggerFactory.Make(name);
			UndoHelper.ApplyFields(trigger, fields);
			trigsys.Triggers.Add(trigger);
		}

		public void PerformRedo()
		{
			trigsys.Triggers.Remove(trigsys.Triggers.Find(id));
		}

		public void StoreUndo()
		{
		}

		public void StoreRedo()
		{
		}
	}
}
