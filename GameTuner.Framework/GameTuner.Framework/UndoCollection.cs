using System;

namespace GameTuner.Framework
{
	public class UndoCollection
	{
		private class UndoStack
		{
			public IUndo Undo;

			public bool DidUndo;

			public UndoStack(IUndo undo)
			{
				Undo = undo;
				DidUndo = false;
			}
		}

		private class UndoStackCollection : ListEvent<UndoStack>
		{
		}

		private UndoStackCollection undoStack;

		private int undoPoint;

		public bool CanUndo
		{
			get
			{
				if (!IsEmpty)
				{
					return undoPoint != -1;
				}
				return false;
			}
		}

		public bool CanRedo
		{
			get
			{
				if (!IsEmpty)
				{
					return undoPoint < undoStack.Count - 1;
				}
				return false;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return undoStack.Count == 0;
			}
		}

		public IUndo Current
		{
			get
			{
				if (!CanUndo)
				{
					return null;
				}
				return undoStack[undoPoint].Undo;
			}
		}

		public event UndoEventHandler PerformedUndo;

		public event UndoEventHandler PerformedRedo;

		public event EventHandler ClearedUndo;

		public event EventHandler StackChanged;

		public UndoCollection()
		{
			undoStack = new UndoStackCollection();
			undoPoint = -1;
		}

		private void NotifyStackChanged()
		{
			EventHandler stackChanged = this.StackChanged;
			if (stackChanged != null)
			{
				stackChanged(this, EventArgs.Empty);
			}
		}

		public virtual IUndo Add(IUndo undo)
		{
			if (CanRedo)
			{
				undoStack.RemoveRange(undoPoint + 1, undoStack.Count - (undoPoint + 1));
			}
			undoStack.Add(new UndoStack(undo));
			undo.StoreUndo();
			undoPoint = undoStack.Count - 1;
			NotifyStackChanged();
			return undo;
		}

		public virtual void PerformUndo()
		{
			if (CanUndo)
			{
				UndoStack undoStack = this.undoStack[undoPoint];
				if (!undoStack.DidUndo)
				{
					undoStack.DidUndo = true;
					undoStack.Undo.StoreRedo();
				}
				IUndo undo = undoStack.Undo;
				undo.PerformUndo();
				undoPoint--;
				NotifyStackChanged();
				UndoEventHandler performedRedo = this.PerformedRedo;
				if (performedRedo != null)
				{
					performedRedo(this, new UndoEventArgs(undo));
				}
			}
		}

		public virtual void PerformRedo()
		{
			if (CanRedo)
			{
				undoPoint++;
				IUndo undo = undoStack[undoPoint].Undo;
				undo.PerformRedo();
				NotifyStackChanged();
				UndoEventHandler performedUndo = this.PerformedUndo;
				if (performedUndo != null)
				{
					performedUndo(this, new UndoEventArgs(undo));
				}
			}
		}

		public void Clear()
		{
			undoStack.Clear();
			undoPoint = -1;
			NotifyStackChanged();
			EventHandler clearedUndo = this.ClearedUndo;
			if (clearedUndo != null)
			{
				clearedUndo(this, EventArgs.Empty);
			}
		}
	}
}
