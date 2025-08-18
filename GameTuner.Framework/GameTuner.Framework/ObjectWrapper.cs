namespace GameTuner.Framework
{
	public class ObjectWrapper
	{
		private string toString;

		public object Object { get; set; }

		public ObjectWrapper(object value)
		{
			Object = value;
		}

		public ObjectWrapper(object value, string toString)
		{
			Object = value;
			this.toString = toString;
		}

		public override string ToString()
		{
			return toString ?? base.ToString();
		}
	}
}
