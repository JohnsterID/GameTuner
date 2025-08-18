using System;

namespace GameTuner.Framework
{
	public class UndoEventArgs : EventArgs
	{
		public UndoStyle Style { get; private set; }

		public IUndo Undo { get; private set; }

		public UndoEventArgs(IUndo undo)
		{
			Undo = undo;
			UndoStyleAttribute attribute = ReflectionHelper.GetAttribute<UndoStyleAttribute>(undo);
			Style = ((attribute != null) ? attribute.Style : UndoStyle.None);
		}
	}
}
