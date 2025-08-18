namespace GameTuner.Framework.Scrollables
{
	public interface IScrollableItemKeys
	{
		IKey FindKey(int x, int y, TimeLineHitArgs e);

		void MoveKey(object sender, IKey key, int delta);

		IKey AddKey(object sender, int x, int y);

		void RemoveKey(object sender, IKey key);
	}
}
