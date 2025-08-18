namespace GameTuner.Framework
{
	public interface ICurveKey
	{
		ListEvent<IKey> Keys { get; }

		IKey Add(float time, float value);

		IKey FindNearestKey(float time);

		bool LowerBound(float time);

		bool UpperBound(float time);
	}
}
