using System.IO;

namespace GameTuner.Framework
{
	public interface IXmlObjectCreator
	{
		IXmlObject CreateObject();

		IXmlObject LoadObject(Stream stream);

		IXmlObject LoadObject(string filename);
	}
}
