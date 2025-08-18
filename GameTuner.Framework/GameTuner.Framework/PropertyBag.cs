using System;
using System.Collections.Generic;

namespace GameTuner.Framework
{
	public class PropertyBag
	{
		private class BagDictonary : Dictionary<string, object>
		{
		}

		private class BagObject : List<object>
		{
		}

		private BagDictonary dictionary;

		public IEnumerable<string> Keys
		{
			get
			{
				foreach (string key in dictionary.Keys)
				{
					yield return key;
				}
			}
		}

		public PropertyBag()
		{
			dictionary = new BagDictonary();
		}

		public void Clear()
		{
			dictionary.Clear();
		}

		public void Remove(string key)
		{
			dictionary.Remove(key);
		}

		public void Remove(string key, Type type)
		{
			object value;
			if (!dictionary.TryGetValue(key, out value))
			{
				return;
			}
			BagObject bagObject = value as BagObject;
			if (bagObject == null)
			{
				return;
			}
			foreach (object item in bagObject)
			{
				if (type.IsInstanceOfType(item))
				{
					bagObject.Remove(item);
					break;
				}
			}
		}

		public bool HasKey(string key)
		{
			return dictionary.ContainsKey(key);
		}

		public void Add(string key, object value)
		{
			if (value == null)
			{
				return;
			}
			object value2;
			if (dictionary.TryGetValue(key, out value2))
			{
				BagObject bagObject = value2 as BagObject;
				if (bagObject == null)
				{
					bagObject = new BagObject();
					bagObject.Add(value2);
					dictionary[key] = bagObject;
				}
				bagObject.Add(value);
			}
			else
			{
				dictionary.Add(key, value);
			}
		}

		public T Get<T>(string key)
		{
			object value;
			if (dictionary.TryGetValue(key, out value))
			{
				if (value is T)
				{
					return (T)value;
				}
				if (value is BagObject)
				{
					BagObject bagObject = (BagObject)value;
					foreach (object item in bagObject)
					{
						if (item is T)
						{
							return (T)item;
						}
					}
				}
			}
			return default(T);
		}
	}
}
