using System;

namespace GameTuner.Framework
{
	public interface IServiceProviderEx : IServiceProvider
	{
		T GetService<T>();
	}
}
