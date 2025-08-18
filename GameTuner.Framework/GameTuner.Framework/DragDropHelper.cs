using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public static class DragDropHelper
	{
		public static List<T> GetListOfType<T>(DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(List<object>)))
			{
				List<object> list = (List<object>)e.Data.GetData(typeof(List<object>));
				if (list.Count > 0)
				{
					List<T> list2 = new List<T>();
					{
						foreach (object item in list)
						{
							if (typeof(T).IsInstanceOfType(item))
							{
								list2.Add((T)item);
							}
						}
						return list2;
					}
				}
			}
			return null;
		}

		public static bool IsListOfType(DragEventArgs e, Type type)
		{
			if (e.Data.GetDataPresent(typeof(List<object>)))
			{
				List<object> list = (List<object>)e.Data.GetData(typeof(List<object>));
				if (list.Count > 0)
				{
					foreach (object item in list)
					{
						if (!type.IsInstanceOfType(item))
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}
	}
}
