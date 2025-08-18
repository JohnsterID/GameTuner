namespace GameTuner.Framework
{
	public interface IVirtualDirectory
	{
		string Name { get; }

		VirtualItemCollection Items { get; }
	}
}
