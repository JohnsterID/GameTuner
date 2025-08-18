namespace GameTuner.Framework
{
	public interface IRadioWatcher
	{
		object SelectedItem { get; set; }

		event RadioWatcherHandler SelectionChanged;
	}
}
