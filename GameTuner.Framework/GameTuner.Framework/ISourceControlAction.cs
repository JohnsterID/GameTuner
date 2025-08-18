namespace GameTuner.Framework
{
	public interface ISourceControlAction
	{
		void GetLatest();

		void UndoCheckout();

		void Checkout(string changeList);

		void Add(string changeList);
	}
}
