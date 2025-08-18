namespace GameTuner.Framework
{
	public class RadioWatcher : IRadioWatcher
	{
		private object selectedItem;

		public object SelectedItem
		{
			get
			{
				return selectedItem;
			}
			set
			{
				selectedItem = value;
				RadioWatcherHandler selectionChanged = this.SelectionChanged;
				if (selectionChanged != null)
				{
					selectionChanged(this, new RadioWatcherEventArgs(selectedItem));
				}
			}
		}

		public event RadioWatcherHandler SelectionChanged;
	}
}
