namespace GameTuner.Framework
{
	public class UndoField
	{
		public string Field { get; private set; }

		public string Value { get; private set; }

		public UndoField(string field, string value)
		{
			Field = field;
			Value = value;
		}

		public override string ToString()
		{
			return string.Format("{0} = '{1}'", Field, Value);
		}
	}
}
