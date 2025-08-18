using System;

namespace GameTuner.Framework.Graph
{
	[UndoStyle(UndoStyle.Property)]
	public class UndoGraphLink : IUndo
	{
		private GraphItemDescriptor inSocket;

		private GraphItemDescriptor outSocket;

		private GraphItemDescriptor lnkSocket;

		private GraphItemDescriptor origSocket;

		public UndoGraphLink(IGraph graph, IGraphSocket inSocket, IGraphSocket outSocket, IGraphSocket origSocket)
		{
			IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)inSocket;
			if (flowGraphSocket.FlowType != FlowGraphSocketType.Input)
			{
				throw new ArgumentException("Attempt to undo a non-input socket");
			}
			if (flowGraphSocket.OutputSocket != null)
			{
				lnkSocket = new GraphItemDescriptor(graph, flowGraphSocket.OutputSocket);
			}
			if (origSocket != null)
			{
				this.origSocket = new GraphItemDescriptor(graph, origSocket);
			}
			this.inSocket = new GraphItemDescriptor(graph, inSocket);
			this.outSocket = new GraphItemDescriptor(graph, outSocket);
		}

		public void PerformUndo()
		{
			if (origSocket != null)
			{
				IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)origSocket.Socket;
				flowGraphSocket.Connect(outSocket.Node, outSocket.Socket);
				flowGraphSocket = (IFlowGraphSocket)inSocket.Socket;
				if (lnkSocket != null)
				{
					flowGraphSocket.Connect(lnkSocket.Node, lnkSocket.Socket);
				}
				else
				{
					flowGraphSocket.Disconnect();
				}
			}
			else
			{
				IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)inSocket.Socket;
				flowGraphSocket.Disconnect();
				if (lnkSocket != null)
				{
					flowGraphSocket.Connect(lnkSocket.Node, lnkSocket.Socket);
				}
			}
		}

		public void PerformRedo()
		{
			IFlowGraphSocket flowGraphSocket;
			if (origSocket != null)
			{
				flowGraphSocket = (IFlowGraphSocket)origSocket.Socket;
				flowGraphSocket.Disconnect();
			}
			flowGraphSocket = (IFlowGraphSocket)inSocket.Socket;
			flowGraphSocket.Connect(outSocket.Node, outSocket.Socket);
		}

		public void StoreUndo()
		{
		}

		public void StoreRedo()
		{
		}
	}
}
