using System;

namespace GameTuner.Framework.Graph
{
	public interface IGraphNub : IUniqueID, ITagProvider, IServiceProviderEx, IServiceProvider
	{
		string Text { get; set; }

		float Order { get; set; }

		IGraphSocket Owner { get; }

		NubType NubType { get; }
	}
}
