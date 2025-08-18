namespace GameTuner.Framework.Trigger
{
	[UndoStyle(UndoStyle.Property)]
	public class UndoTriggerField : IUndo
	{
		private ITriggerSystem trigsys;

		private int id;

		private string oldValue;

		private string newValue;

		private string field;

		private ITrigger Trigger
		{
			get
			{
				return trigsys.Triggers.Find(id);
			}
		}

		public UndoTriggerField(ITrigger trigger, string field, object oldValue)
		{
			trigsys = trigger.Owner;
			id = trigger.ID;
			this.field = field;
			this.oldValue = Transpose.ToString(oldValue);
		}

		public void PerformUndo()
		{
			UndoHelper.ApplyValue(Trigger, field, oldValue);
		}

		public void PerformRedo()
		{
			UndoHelper.ApplyValue(Trigger, field, newValue);
		}

		public void StoreUndo()
		{
		}

		public void StoreRedo()
		{
			newValue = UndoHelper.AcquireValue(Trigger, field);
		}
	}
}
