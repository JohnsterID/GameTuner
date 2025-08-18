namespace GameTuner.Framework
{
	public struct Bounds<T>
	{
		public T Min;

		public T Max;

		private static Bounds<T> empty = new Bounds<T>(default(T), default(T));

		public static Bounds<T> Empty
		{
			get
			{
				return empty;
			}
		}

		public Bounds(T min, T max)
		{
			Min = min;
			Max = max;
		}
	}
}
