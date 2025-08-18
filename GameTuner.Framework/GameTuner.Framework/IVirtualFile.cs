using System.IO;

namespace GameTuner.Framework
{
	public interface IVirtualFile
	{
		string FullPath { get; }

		string Name { get; }

		Stream Stream { get; }
	}
}
