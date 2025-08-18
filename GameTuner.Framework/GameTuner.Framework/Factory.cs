using System.Collections.Generic;

namespace GameTuner.Framework
{
	public class Factory<T> : ListEvent<IMaker>
	{
		public IMaker Find(string name)
		{
			return Find((IMaker a) => a.Name.CompareTo(name) == 0);
		}

		public T Make(string name)
		{
			return Make(name, null);
		}

		public T Make(string name, params object[] args)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IMaker current = enumerator.Current;
					if (string.Compare(name, current.Name, true) == 0)
					{
						return (T)current.Make(args);
					}
				}
			}
			return default(T);
		}
	}
}
