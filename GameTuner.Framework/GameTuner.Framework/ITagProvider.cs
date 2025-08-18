namespace GameTuner.Framework
{
	public interface ITagProvider
	{
		[NoSerialize]
		object Tag { get; set; }
	}
}
