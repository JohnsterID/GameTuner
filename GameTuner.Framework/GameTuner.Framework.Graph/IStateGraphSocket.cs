namespace GameTuner.Framework.Graph
{
	public interface IStateGraphSocket
	{
		bool Bidirectional { get; set; }

		int Order { get; set; }

		IGraphNode Node { get; }

		Factory<IGraphNub> NubFactory { get; }

		void Connect(IGraphNode node);

		void Disconnect();

		IGraphNub CreateNub();
	}
}
