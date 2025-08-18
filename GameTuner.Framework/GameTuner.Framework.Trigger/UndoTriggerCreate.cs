using System.Collections.Generic;

namespace GameTuner.Framework.Trigger
{
	[UndoStyle(UndoStyle.Create)]
	public class UndoTriggerCreate : IUndo
	{
		private ITriggerSystem trigsys;

		private int id;

		private string name;

		private List<UndoField> fields;

		public UndoTriggerCreate(ITrigger trigger)
		{
			trigsys = trigger.Owner;
			id = trigger.ID;
			DataNameAttribute attribute = ReflectionHelper.GetAttribute<DataNameAttribute>(trigger);
			name = ((attribute != null) ? attribute.Name : trigger.GetType().Name);
			fields = UndoHelper.AcquireFields(trigger);
		}

		public void PerformUndo()
		{
			trigsys.Triggers.Remove(trigsys.Triggers.Find(id));
		}

		public void PerformRedo()
		{
			ITrigger trigger = trigsys.TriggerFactory.Make(name);
			UndoHelper.ApplyFields(trigger, fields);
			trigsys.Triggers.Add(trigger);
		}

		public void StoreUndo()
		{
		}

		public void StoreRedo()
		{
		}
	}
}
