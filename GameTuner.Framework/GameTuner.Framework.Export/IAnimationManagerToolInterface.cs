using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace GameTuner.Framework.Export
{
	public abstract class IAnimationManagerToolInterface
	{
		private const string AnimationXmlFilter = "Animation Definition (*.xml)|*.xml|GameTuner XML Files (*.fxsxml)|*.fxsxml";

		public abstract List<AnimationEntry> AnimationList { get; set; }

		public abstract List<LayerEntry> LayerList { get; set; }

		public abstract int StartFrame { get; set; }

		public abstract int EndFrame { get; set; }

		public abstract int CurrentFrame { get; set; }

		public abstract List<string> BoneList { get; }

		public abstract string DefaultExportSettings { get; }

		public virtual string ImportFilter
		{
			get
			{
				return "Animation Definition (*.xml)|*.xml|GameTuner XML Files (*.fxsxml)|*.fxsxml";
			}
		}

		public virtual string ExportFilter
		{
			get
			{
				return "Animation Definition (*.xml)|*.xml|GameTuner XML Files (*.fxsxml)|*.fxsxml";
			}
		}

		public abstract string GetAnimationFilename(AnimationEntry kEntry);

		public abstract float GetBoneWeight(string szLayerName, string szBoneName);

		public abstract void SetBoneWeight(string szLayerName, string szBoneName, float fWeight);

		public virtual bool ImportFromFile(string szFilename)
		{
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(szFilename);
			}
			catch (XmlException ex)
			{
				MessageBox.Show(string.Format("Error reading {0}: {1} at position {2},{3} ", szFilename, ex.Message, ex.LineNumber, ex.LinePosition));
				return false;
			}
			if (string.Compare(Path.GetExtension(szFilename), ".xml", true) == 0)
			{
				XPathNavigator xPathNavigator = xmlDocument.CreateNavigator();
				XPathNodeIterator xPathNodeIterator = xPathNavigator.Select("//AnimationDef");
				while (xPathNodeIterator.MoveNext())
				{
					AnimationEntry animationEntry = new AnimationEntry(this);
					animationEntry.AnimationName = xPathNodeIterator.Current.GetAttribute("name", "");
					animationEntry.EventCode = Convert.ToInt32(xPathNodeIterator.Current.GetAttribute("ec", ""));
					string attribute = xPathNodeIterator.Current.GetAttribute("start", "");
					string attribute2 = xPathNodeIterator.Current.GetAttribute("end", "");
					attribute = attribute.TrimEnd("fF".ToCharArray());
					attribute2 = attribute2.TrimEnd("fF".ToCharArray());
					animationEntry.StartFrame = Convert.ToInt32(attribute);
					animationEntry.EndFrame = Convert.ToInt32(attribute2);
					animationEntry.ExportSettings = xPathNodeIterator.Current.GetAttribute("settings", "");
					AnimationList.Add(animationEntry);
				}
			}
			else if (string.Compare(Path.GetExtension(szFilename), ".fxsxml", true) == 0)
			{
				XPathNavigator xPathNavigator2 = xmlDocument.CreateNavigator();
				XPathNodeIterator xPathNodeIterator2 = xPathNavigator2.Select("//Animation");
				while (xPathNodeIterator2.MoveNext())
				{
					AnimationEntry animationEntry2 = new AnimationEntry(this);
					animationEntry2.AnimationName = xPathNodeIterator2.Current.GetAttribute("name", "");
					animationEntry2.EventCode = Convert.ToInt32(xPathNodeIterator2.Current.GetAttribute("ec", ""));
					animationEntry2.StartFrame = StartFrame;
					animationEntry2.EndFrame = EndFrame;
					animationEntry2.ExportSettings = DefaultExportSettings;
					AnimationList.Add(animationEntry2);
				}
			}
			return true;
		}

		public virtual void ExportToFile(string szFilename)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "AnimationDefinitions", "");
			xmlDocument.AppendChild(xmlNode);
			foreach (AnimationEntry animation in AnimationList)
			{
				XmlElement xmlElement = xmlDocument.CreateNode(XmlNodeType.Element, "AnimationDef", "") as XmlElement;
				xmlElement.SetAttribute("name", animation.AnimationName);
				xmlElement.SetAttribute("ec", animation.EventCode.ToString());
				xmlElement.SetAttribute("start", animation.StartFrame.ToString());
				xmlElement.SetAttribute("end", animation.EndFrame.ToString());
				xmlElement.SetAttribute("settings", animation.ExportSettings);
				xmlNode.AppendChild(xmlElement);
			}
			xmlDocument.Save(szFilename);
		}

		public virtual void PlayAnimation()
		{
		}
	}
}
