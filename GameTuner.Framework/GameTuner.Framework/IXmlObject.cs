using System.IO;

namespace GameTuner.Framework
{
	public interface IXmlObject
	{
		object Root { get; }

		void Load(Stream stream);

		void Load(string filename);

		void Save(string filename);

		void Save(Stream stream);

		void Clear();

		bool IsElement(object node);

		bool IsElement(object node, string name);

		bool IsText(object node);

		object GetChild(object node);

		object GetChild(object node, string name);

		object GetSibling(object node);

		object GetSibling(object node, string name);

		object GetParent(object node);

		object AddRoot(string name);

		object AddNode(object parent, string name);

		object Find(string name);

		object Find(object node, string name);

		string GetText(object node);

		string GetText(object node, string name);

		void SetText(object node, string text);

		void SetText(object node, string name, string text);

		void RemoveText(object node);

		void SetAttrib<T>(object node, string name, T value);

		string GetAttrib(object node, string name);

		T GetAttrib<T>(object node, string name);

		T GetAttrib<T>(object node, string name, T defaultValue);
	}
}
