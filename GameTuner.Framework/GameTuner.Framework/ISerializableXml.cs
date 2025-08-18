using System.Xml;

namespace GameTuner.Framework
{
	public interface ISerializableXml
	{
		void Load(XmlDoc doc, XmlNode node);

		void Save(XmlDoc doc, XmlNode node);
	}
}
