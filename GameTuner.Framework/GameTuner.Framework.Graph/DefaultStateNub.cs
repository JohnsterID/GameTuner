using System;

namespace GameTuner.Framework.Graph
{
	public class DefaultStateNub : IGraphNub, IUniqueID, ITagProvider, IServiceProviderEx, IServiceProvider
	{
		public string Text { get; set; }

		public float Order { get; set; }

		public IGraphSocket Owner { get; private set; }

		public int ID { get; set; }

		public object Tag { get; set; }

		public NubType NubType { get; private set; }

		public DefaultStateNub(IGraphSocket owner)
			: this(owner, NubType.Condition)
		{
		}

		public DefaultStateNub(IGraphSocket owner, NubType type)
		{
			Owner = owner;
			Text = "";
			NubType = type;
		}

		public T GetService<T>()
		{
			return Owner.Owner.Owner.GetService<T>();
		}

		public object GetService(Type serviceType)
		{
			return Owner.Owner.Owner.ServiceProvider.GetService(serviceType);
		}
	}
}
