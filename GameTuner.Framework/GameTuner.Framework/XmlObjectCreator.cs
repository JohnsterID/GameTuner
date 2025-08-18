using System.IO;

namespace GameTuner.Framework
{
	public class XmlObjectCreator : IXmlObjectCreator
	{
		public IXmlObject CreateObject()
		{
			return new XmlObject();
		}

		public IXmlObject LoadObject(Stream stream)
		{
			return new XmlObject(stream);
		}

		public IXmlObject LoadObject(string filename)
		{
			return new XmlObject(filename);
		}
	}
}
