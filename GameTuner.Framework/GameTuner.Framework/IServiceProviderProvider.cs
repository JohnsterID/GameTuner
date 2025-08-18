using System;

namespace GameTuner.Framework
{
	public interface IServiceProviderProvider
	{
		IServiceProvider ServiceProvider { get; set; }

		T GetService<T>();
	}
}
