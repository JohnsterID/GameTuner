using System;
using System.Collections;
using System.Collections.Generic;

namespace GameTuner.Framework
{
	public static class ListExtension
	{
		public static IEnumerable<T> ReverseIterator<T>(this List<T> list)
		{
			int count = list.Count;
			for (int i = count - 1; i >= 0; i--)
			{
				yield return list[i];
			}
		}

		public static void ForEach<T>(this IEnumerable list, Action<T> action)
		{
			foreach (T item in list)
			{
				action(item);
			}
		}

		public static T Find<T>(this IEnumerable list, Predicate<T> match)
		{
			foreach (T item in list)
			{
				if (match(item))
				{
					return item;
				}
			}
			return default(T);
		}
	}
}
