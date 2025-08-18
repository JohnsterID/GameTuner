namespace GameTuner.Framework
{
	public interface IUndo
	{
		void PerformUndo();

		void PerformRedo();

		void StoreUndo();

		void StoreRedo();
	}
}
