using System;
using System.IO;
using System.Xml;

namespace GameTuner.Framework
{
	internal class XmlObject : IXmlObject
	{
		private XmlDocument doc;

		public object Root
		{
			get
			{
				return doc;
			}
		}

		public XmlObject()
		{
			doc = new XmlDocument();
		}

		public XmlObject(Stream stream)
		{
			doc = new XmlDocument();
			Load(stream);
		}

		public XmlObject(string filename)
		{
			doc = new XmlDocument();
			Load(filename);
		}

		public void Load(Stream stream)
		{
			doc.Load(stream);
		}

		public void Load(string filename)
		{
			doc.Load(filename);
		}

		public void Save(string filename)
		{
			doc.Save(filename);
		}

		public void Save(Stream stream)
		{
			doc.Save(stream);
		}

		public void Clear()
		{
			doc.RemoveAll();
		}

		public bool IsElement(object node)
		{
			if (node != null && node is XmlNode)
			{
				return ((XmlNode)node).NodeType == XmlNodeType.Element;
			}
			return false;
		}

		public bool IsElement(object node, string name)
		{
			if (IsElement(node))
			{
				return ((XmlNode)node).Name.CompareTo(name) == 0;
			}
			return false;
		}

		public bool IsText(object node)
		{
			if (node != null && node is XmlNode)
			{
				return ((XmlNode)node).NodeType == XmlNodeType.Text;
			}
			return false;
		}

		public object GetChild(object node)
		{
			if (node != null && node is XmlNode)
			{
				XmlNode xmlNode = (XmlNode)node;
				xmlNode = xmlNode.FirstChild;
				while (xmlNode != null && !IsElement(xmlNode))
				{
					xmlNode = xmlNode.NextSibling;
				}
				return xmlNode;
			}
			return null;
		}

		public object GetChild(object node, string name)
		{
			if (node != null && node is XmlNode)
			{
				XmlNode xmlNode = (XmlNode)node;
				xmlNode = xmlNode.FirstChild;
				while (xmlNode != null && !IsElement(xmlNode, name))
				{
					xmlNode = xmlNode.NextSibling;
				}
				return xmlNode;
			}
			return null;
		}

		public object GetSibling(object node)
		{
			if (node != null && node is XmlNode)
			{
				XmlNode xmlNode = (XmlNode)node;
				xmlNode = xmlNode.NextSibling;
				while (xmlNode != null && !IsElement(xmlNode))
				{
					xmlNode = xmlNode.NextSibling;
				}
				return xmlNode;
			}
			return null;
		}

		public object GetSibling(object node, string name)
		{
			if (node != null && node is XmlNode)
			{
				XmlNode xmlNode = (XmlNode)node;
				xmlNode = xmlNode.NextSibling;
				while (xmlNode != null && !IsElement(xmlNode, name))
				{
					xmlNode = xmlNode.NextSibling;
				}
				return xmlNode;
			}
			return null;
		}

		public object GetParent(object node)
		{
			if (node == null || !(node is XmlNode))
			{
				return null;
			}
			return ((XmlNode)node).ParentNode;
		}

		public object AddRoot(string name)
		{
			if (GetChild(doc) != null)
			{
				throw new Exception("Root already exists");
			}
			return AddNode(doc, name);
		}

		public object AddNode(object parent, string name)
		{
			if (parent != null && parent is XmlNode)
			{
				return ((XmlNode)parent).AppendChild(doc.CreateElement(name));
			}
			return null;
		}

		public object Find(string name)
		{
			return IterateFind(doc, name);
		}

		public object Find(object node, string name)
		{
			return IterateFind(node, name);
		}

		public string GetText(object node)
		{
			if (node != null && node is XmlNode)
			{
				XmlNode xmlNode = (XmlNode)node;
				for (xmlNode = xmlNode.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
				{
					if (xmlNode.NodeType == XmlNodeType.Text)
					{
						return xmlNode.Value;
					}
				}
			}
			return "";
		}

		public string GetText(object node, string name)
		{
			return GetText(GetChild(node, name));
		}

		public void SetText(object node, string text)
		{
			if (node != null && node is XmlNode)
			{
				RemoveText(node);
				((XmlNode)node).AppendChild(doc.CreateTextNode(text));
			}
		}

		public void SetText(object node, string name, string text)
		{
			if (node != null && node is XmlNode)
			{
				object obj = Find(node, name);
				if (obj == null)
				{
					obj = AddNode(node, name);
				}
				SetText(obj, text);
			}
		}

		public void RemoveText(object node)
		{
			if (node == null || !(node is XmlNode))
			{
				return;
			}
			XmlNode xmlNode = (XmlNode)node;
			for (XmlNode xmlNode2 = xmlNode.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
			{
				if (xmlNode2.NodeType == XmlNodeType.Text)
				{
					xmlNode.RemoveChild(xmlNode2);
					break;
				}
			}
		}

		public void SetAttrib<T>(object node, string name, T value)
		{
			if (node != null && node is XmlNode)
			{
				XmlAttribute xmlAttribute = doc.CreateAttribute(name);
				xmlAttribute.Value = value.ToString();
				((XmlNode)node).Attributes.SetNamedItem(xmlAttribute);
			}
		}

		public string GetAttrib(object node, string name)
		{
			return GetAttrib(node, name, "");
		}

		public T GetAttrib<T>(object node, string name)
		{
			return GetAttrib(node, name, default(T));
		}

		public T GetAttrib<T>(object node, string name, T defaultValue)
		{
			if (node != null && node is XmlNode)
			{
				XmlAttribute xmlAttribute = FindAttribute(node, name);
				if (xmlAttribute != null)
				{
					return Transpose.FromString<T>(xmlAttribute.Value);
				}
			}
			return defaultValue;
		}

		private XmlAttribute FindAttribute(object node, string name)
		{
			if (node != null && node is XmlNode)
			{
				XmlAttributeCollection attributes = ((XmlNode)node).Attributes;
				return (XmlAttribute)attributes.GetNamedItem(name);
			}
			return null;
		}

		private object IterateFind(object node, string tag)
		{
			if (IsElement(node, tag))
			{
				return node;
			}
			for (node = GetChild(node, tag); node != null; node = GetSibling(node, tag))
			{
				object obj = IterateFind(node, tag);
				if (obj != null)
				{
					return obj;
				}
			}
			return null;
		}
	}
}
