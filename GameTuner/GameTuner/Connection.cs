using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using GameTuner.Framework;

namespace GameTuner
{
	internal class Connection : SocketConnection
	{
		private delegate void ListenForMessagesDelegate();

		private struct RequestResponse
		{
			public int Listener;

			public List<string> Messages;
		}

		public delegate void OutputMsgHandler(string sOutputMsg);

		public delegate void RequestListener(List<string> response);

		private int m_iReadBufferOffset;

		private byte[] m_ReadBuffer = new byte[1048576];

		private ListenForMessagesDelegate m_ListenForMessagesDelegate;

		private SocketAsyncEventArgs m_RecieveArgs;

		private bool m_bPendingRecieve;

		private byte[] m_Message;

		private int m_iMessageIndex;

		private object m_ResponseLock = new object();

		private List<RequestResponse> m_ResponsesToProcess = new List<RequestResponse>();

		private bool m_bRoutingMessages;

		private List<RequestResponse> m_Responses = new List<RequestResponse>();

		public OutputMsgHandler OnOutputMsgRecieved;

		private RequestListener m_DefaultRequestHandler;

		private List<RequestListener> m_RequestListeners = new List<RequestListener>();

		private Dictionary<RequestListener, int> m_RequestListenersIndexMap = new Dictionary<RequestListener, int>();

		public static Connection Instance { get; private set; }

		public static void Init(Control invokeTarget)
		{
			if (Instance != null)
			{
				Instance.CloseConnection();
			}
			Instance = new Connection(invokeTarget);
		}

		private Connection(Control invokeTarget)
			: base(invokeTarget, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4318))
		{
			base.ConnectionEstablished += OnConnected;
			m_DefaultRequestHandler = DefaultRequestHandler;
			m_ListenForMessagesDelegate = ListenForMessages;
		}

		public void ConnectionPrompt()
		{
			ConnectionPrompt(base.ConnectionTarget.Address, base.ConnectionTarget.Port);
		}

		public void ConnectionPrompt(string sIP, int iPort)
		{
			ConnectionPrompt(IPAddress.Parse(sIP), iPort);
		}

		public void ConnectionPrompt(IPAddress ip, int iPort)
		{
			ConnectionWnd connectionWnd = new ConnectionWnd(ip, iPort);
			DialogResult dialogResult = connectionWnd.ShowDialog();
			if (dialogResult == DialogResult.OK)
			{
				SetConnection(connectionWnd.IP, connectionWnd.Port);
				OpenConnection();
			}
		}

		public void SetConnection(IPAddress ip, int iPort)
		{
			CloseConnection();
			base.ConnectionTarget = new IPEndPoint(ip, iPort);
		}

		private void OnConnected(object sender, EventArgs args)
		{
			m_iReadBufferOffset = 0;
			m_Message = null;
			m_iMessageIndex = 0;
			m_bPendingRecieve = false;
			ListenForMessages();
		}

		private void ListenForMessages()
		{
			if (m_RecieveArgs == null)
			{
				m_RecieveArgs = new SocketAsyncEventArgs();
				m_RecieveArgs.Completed += RecieveAsyncCompleted;
			}
			if (!m_bPendingRecieve)
			{
				m_bPendingRecieve = true;
				m_RecieveArgs.SetBuffer(m_ReadBuffer, m_iReadBufferOffset, m_ReadBuffer.Length - m_iReadBufferOffset);
				RecieveAsync(m_RecieveArgs);
			}
		}

		private void RecieveAsyncCompleted(object sender, SocketAsyncEventArgs e)
		{
			m_bPendingRecieve = false;
			if (e.SocketError == SocketError.Success)
			{
				if (e.BytesTransferred == 0)
				{
					SafeInvoke(base.OnConnectionLostDelegate);
					return;
				}
				OnRecievedData(e.Buffer, e.BytesTransferred);
				SafeInvoke(m_ListenForMessagesDelegate);
			}
		}

		private void OnRecievedData(byte[] data, int iTransfered)
		{
			int num = m_iReadBufferOffset + iTransfered;
			if (num > data.Length)
			{
				throw new Exception("Invalid transfered size");
			}
			m_iReadBufferOffset = 0;
			int num2 = 0;
			while (num2 < num)
			{
				if (m_Message == null)
				{
					int num3 = num - num2;
					if (num3 < 4)
					{
						for (int i = 0; i < num3; i++)
						{
							m_ReadBuffer[i] = m_ReadBuffer[num2 + i];
						}
						m_iReadBufferOffset = num3;
						break;
					}
					uint num4 = BitConverter.ToUInt32(m_ReadBuffer, num2);
					if (num4 > 524288)
					{
						SafeInvoke(base.OnConnectionLostDelegate);
					}
					uint num5 = num4 + 4;
					num2 += 4;
					m_Message = new byte[num5];
					m_iMessageIndex = 0;
				}
				else
				{
					while (m_iMessageIndex < m_Message.Length && num2 < num)
					{
						m_Message[m_iMessageIndex++] = data[num2++];
					}
					if (m_iMessageIndex == m_Message.Length)
					{
						OnMessageRecieved(m_Message);
						m_Message = null;
					}
				}
			}
		}

		private void OnMessageRecieved(byte[] message)
		{
			MemoryStream input = new MemoryStream(message);
			BinaryReader binaryReader = new BinaryReader(input);
			uint listener = binaryReader.ReadUInt32();
			char[] array = binaryReader.ReadChars(message.Length - 4);
			RequestResponse item = default(RequestResponse);
			item.Listener = (int)listener;
			item.Messages = new List<string>();
			string text = string.Empty;
			foreach (char c in array)
			{
				if (c == '\0')
				{
					item.Messages.Add(text);
					text = string.Empty;
				}
				else
				{
					text += c;
				}
			}
			lock (m_ResponseLock)
			{
				m_ResponsesToProcess.Add(item);
			}
		}

		public void RouteMessages()
		{
			if (m_bRoutingMessages)
			{
				return;
			}
			m_bRoutingMessages = true;
			lock (m_ResponseLock)
			{
				m_Responses.AddRange(m_ResponsesToProcess);
				m_ResponsesToProcess.Clear();
			}
			foreach (RequestResponse response in m_Responses)
			{
				RequestListener requestListener = GetRequestListener(response.Listener);
				if (requestListener != null)
				{
					requestListener(response.Messages);
				}
			}
			m_Responses.Clear();
			m_bRoutingMessages = false;
		}

		private void DefaultRequestHandler(List<string> results)
		{
			if (results.Count <= 0)
			{
				return;
			}
			if (results[0] == "L")
			{
				results.RemoveAt(0);
				LuaStateManager.Instance.OnLuaStatesRecieved(results);
			}
			else if (results[0] == "O")
			{
				if (OnOutputMsgRecieved != null)
				{
					for (int i = 1; i < results.Count; i++)
					{
						OnOutputMsgRecieved(results[i]);
					}
				}
			}
			else if (results[0] == "Closing")
			{
				CloseConnection();
			}
		}

		public int AddRequestListener(RequestListener listener)
		{
			int num = m_RequestListeners.IndexOf(listener);
			if (num < 0)
			{
				num = m_RequestListeners.Count;
				m_RequestListeners.Add(listener);
				m_RequestListenersIndexMap[listener] = num;
			}
			return num;
		}

		public void RemoveRequestListener(RequestListener listener)
		{
			int value;
			if (listener != null && m_RequestListenersIndexMap.TryGetValue(listener, out value))
			{
				m_RequestListeners[value] = null;
				m_RequestListenersIndexMap.Remove(listener);
			}
		}

		public void RemoveRequestListener(int iIndex)
		{
			RequestListener requestListener = GetRequestListener(iIndex);
			RemoveRequestListener(requestListener);
		}

		private RequestListener GetRequestListener(int iIndex)
		{
			if (iIndex < m_RequestListeners.Count && iIndex >= 0)
			{
				return m_RequestListeners[iIndex];
			}
			return m_DefaultRequestHandler;
		}

		public bool Request(string sMessage, RequestListener listener)
		{
			return Request(sMessage, AddRequestListener(listener));
		}

		public bool Request(string s, int iSender)
		{
			if (s == null)
			{
				return false;
			}
			bool result = false;
			try
			{
				uint num = (uint)(s.Length + 1);
				MemoryStream memoryStream = new MemoryStream((int)(num + 8));
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write(num);
				binaryWriter.Write((uint)iSender);
				foreach (char ch in s)
				{
					binaryWriter.Write(ch);
				}
				binaryWriter.Write('\0');
				byte[] buffer = memoryStream.GetBuffer();
				int num2 = (int)memoryStream.Position;
				uint num3 = (uint)((ulong)num2 - 8uL);
				if (num != num3)
				{
					memoryStream.Position = 0L;
					binaryWriter.Write(num3);
				}
				SocketAsyncEventArgs e = new SocketAsyncEventArgs();
				e.Completed += SendAsyncCompleted;
				e.SetBuffer(buffer, 0, num2);
				result = SendAsync(e);
			}
			catch (Exception exception)
			{
				ErrorHandling.Error(exception, ErrorLevel.SendReport);
			}
			return result;
		}

		private void SendAsyncCompleted(object sender, SocketAsyncEventArgs e)
		{
			if (e.SocketError != SocketError.Success)
			{
				SafeInvoke(base.OnConnectionLostDelegate);
			}
		}
	}
}
