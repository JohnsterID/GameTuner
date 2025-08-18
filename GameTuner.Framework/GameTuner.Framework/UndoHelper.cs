using System.Collections.Generic;
using System.Reflection;
using GameTuner.Framework.Graph;

namespace GameTuner.Framework
{
	public static class UndoHelper
	{
		public class UndoInfo
		{
			public GraphItemDescriptor Descriptor { get; private set; }

			public List<UndoField> Fields { get; private set; }

			public UndoInfo(IGraph graph, object item)
			{
				Descriptor = new GraphItemDescriptor(graph, item);
				Fields = new List<UndoField>();
				Fields = AcquireFields(item);
			}
		}

		public static int UndoGraphDependentSort(object a, object b)
		{
			int num = StateGraphControl.GetTypeOrder(a) - StateGraphControl.GetTypeOrder(b);
			if (num == 0)
			{
				if (a is IFlowGraphSocket)
				{
					num = ((IFlowGraphSocket)a).FlowType - ((IFlowGraphSocket)b).FlowType;
				}
				if (num == 0)
				{
					num = ((IUniqueID)b).ID - ((IUniqueID)a).ID;
				}
			}
			return num;
		}

		public static void AddGraphDependents(object item, List<object> inclist)
		{
			if (!inclist.Contains(item))
			{
				inclist.Add(item);
			}
			IGraphSocket graphSocket = item as IGraphSocket;
			if (graphSocket != null && graphSocket.Nubs != null)
			{
				foreach (IGraphNub nub in graphSocket.Nubs)
				{
					AddGraphDependents(nub, inclist);
				}
			}
			IGraphNode graphNode = item as IGraphNode;
			if (graphNode == null)
			{
				return;
			}
			foreach (IGraphSocket socket in graphNode.Sockets)
			{
				AddGraphDependents(socket, inclist);
			}
			foreach (IGraphNode node in graphNode.Owner.Nodes)
			{
				if (node == graphNode)
				{
					continue;
				}
				foreach (IGraphSocket socket2 in node.Sockets)
				{
					IStateGraphSocket stateGraphSocket = socket2 as IStateGraphSocket;
					if (stateGraphSocket != null && stateGraphSocket.Node == graphNode)
					{
						AddGraphDependents(socket2, inclist);
					}
					IFlowGraphSocket flowGraphSocket = socket2 as IFlowGraphSocket;
					if (flowGraphSocket != null && flowGraphSocket.Node == graphNode)
					{
						AddGraphDependents(socket2, inclist);
					}
				}
			}
		}

		public static string AcquireValue(object obj, string field)
		{
			List<PropertyInfo> list = new List<PropertyInfo>(ReflectionHelper.CollectProperties(obj));
			PropertyInfo propertyInfo = list.Find((PropertyInfo a) => a.Name == field);
			if (propertyInfo != null)
			{
				return Transpose.ToString(propertyInfo.GetValue(obj, null));
			}
			return null;
		}

		public static void ApplyValue(object obj, string field, string value)
		{
			List<PropertyInfo> list = new List<PropertyInfo>(ReflectionHelper.CollectProperties(obj));
			PropertyInfo propertyInfo = list.Find((PropertyInfo a) => a.Name == field);
			if (propertyInfo != null)
			{
				propertyInfo.SetValue(obj, Transpose.FromString(value, propertyInfo.PropertyType), null);
			}
		}

		public static List<UndoField> AcquireFields(object obj)
		{
			List<UndoField> list = new List<UndoField>();
			List<PropertyInfo> list2 = new List<PropertyInfo>(ReflectionHelper.CollectProperties(obj));
			foreach (PropertyInfo item in list2)
			{
				string value = Transpose.ToString(item.GetValue(obj, null));
				list.Add(new UndoField(item.Name, value));
			}
			return list;
		}

		public static void ApplyFields(object obj, List<UndoField> fields)
		{
			List<PropertyInfo> list = new List<PropertyInfo>(ReflectionHelper.CollectProperties(obj));
			PropertyInfo p;
			foreach (PropertyInfo item in list)
			{
				p = item;
				UndoField undoField = fields.Find((UndoField a) => a.Field == p.Name);
				if (undoField != null)
				{
					p.SetValue(obj, Transpose.FromString(undoField.Value, p.PropertyType), null);
				}
			}
		}
	}
}
