using System.Collections;
using System.Collections.Generic;

namespace GameTuner.Framework
{
	public class LocalEnvironment : IEnumerable<KeyValuePair<string, string>>, IEnumerable
	{
		public Dictionary<string, string> Map { get; private set; }

		public string this[string key]
		{
			get
			{
				if (Map.ContainsKey(key))
				{
					return Map[key];
				}
				return "";
			}
			set
			{
				Map[key] = value;
			}
		}

		public LocalEnvironment()
		{
			Map = new Dictionary<string, string>();
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return Map.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Map.GetEnumerator();
		}
	}
}
