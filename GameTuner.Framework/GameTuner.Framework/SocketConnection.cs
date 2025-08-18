using System;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public abstract class SocketConnection
	{
		public class OpenConnectionFailedArgs : EventArgs
		{
			public SocketError SocketError;
		}

		public delegate void OpenConnectionFailedHandler(object sender, OpenConnectionFailedArgs args);

		private delegate void ConnectAsyncCompletedHandler(SocketAsyncEventArgs e);

		protected delegate void OnConnectionLostHandler();

		private IPEndPoint m_ConnectionTarget;

		private Socket m_Socket;

		private ConnectAsyncCompletedHandler ConnectAsyncCompletedDelegate;

		private System.Timers.Timer m_tmrStatusUpdate = new System.Timers.Timer();

		public IPEndPoint ConnectionTarget
		{
			get
			{
				return m_ConnectionTarget;
			}
			set
			{
				if (m_Socket == null)
				{
					m_ConnectionTarget = value;
					return;
				}
				throw new Exception("Cannot change connection target while connection is open.");
			}
		}

		public Control InvokeTarget { get; set; }

		public bool Connected
		{
			get
			{
				if (m_Socket != null)
				{
					return m_Socket.Connected;
				}
				return false;
			}
		}

		public bool Connecting
		{
			get
			{
				if (m_Socket != null)
				{
					return !m_Socket.Connected;
				}
				return false;
			}
		}

		protected OnConnectionLostHandler OnConnectionLostDelegate { get; private set; }

		public event EventHandler ConnectionEstablished;

		public event EventHandler ConnectionLost;

		public event OpenConnectionFailedHandler OpenConnectionFailed;

		public SocketConnection(Control ctrlInvokeTarget, IPEndPoint connectionTarget)
		{
			InvokeTarget = ctrlInvokeTarget;
			ConnectionTarget = connectionTarget;
			ConnectAsyncCompletedDelegate = ConnectAsyncCompleted;
			OnConnectionLostDelegate = OnConnectionLost;
			m_tmrStatusUpdate.Elapsed += m_tmrStatusUpdate_Elapsed;
			m_tmrStatusUpdate.Interval = 500.0;
		}

		public void OpenConnection()
		{
			m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
			SocketAsyncEventArgs e = new SocketAsyncEventArgs();
			e.RemoteEndPoint = m_ConnectionTarget;
			e.Completed += ConnectAsyncCompleted;
			m_Socket.ConnectAsync(e);
		}

		private void ConnectAsyncCompleted(object sender, SocketAsyncEventArgs e)
		{
			if (e.SocketError != SocketError.OperationAborted)
			{
				SafeInvoke(ConnectAsyncCompletedDelegate, e);
			}
		}

		private void ConnectAsyncCompleted(SocketAsyncEventArgs e)
		{
			if (e.SocketError == SocketError.Success)
			{
				EventHandler connectionEstablished = this.ConnectionEstablished;
				if (connectionEstablished != null)
				{
					connectionEstablished(this, EventArgs.Empty);
				}
				m_tmrStatusUpdate.Enabled = true;
				return;
			}
			if (e.SocketError == SocketError.ConnectionRefused)
			{
				if (m_Socket != null)
				{
					m_Socket.ConnectAsync(e);
				}
				return;
			}
			OpenConnectionFailedArgs openConnectionFailedArgs = new OpenConnectionFailedArgs();
			openConnectionFailedArgs.SocketError = e.SocketError;
			OpenConnectionFailedHandler openConnectionFailed = this.OpenConnectionFailed;
			if (openConnectionFailed != null)
			{
				openConnectionFailed(this, openConnectionFailedArgs);
			}
		}

		public void CloseConnection()
		{
			if (m_Socket != null)
			{
				m_tmrStatusUpdate.Enabled = false;
				Socket socket = m_Socket;
				m_Socket = null;
				socket.Close();
			}
		}

		private void m_tmrStatusUpdate_Elapsed(object sender, ElapsedEventArgs e)
		{
			Socket socket = m_Socket;
			if (socket != null && !socket.Connected)
			{
				SafeInvoke(OnConnectionLostDelegate);
			}
		}

		private void OnConnectionLost()
		{
			if (m_Socket != null)
			{
				CloseConnection();
				EventHandler connectionLost = this.ConnectionLost;
				if (connectionLost != null)
				{
					connectionLost(this, EventArgs.Empty);
				}
			}
		}

		protected bool SendAsync(SocketAsyncEventArgs args)
		{
			Socket socket = m_Socket;
			if (socket != null)
			{
				return socket.SendAsync(args);
			}
			return false;
		}

		protected bool RecieveAsync(SocketAsyncEventArgs args)
		{
			Socket socket = m_Socket;
			if (socket != null)
			{
				return socket.ReceiveAsync(args);
			}
			return false;
		}

		protected bool SafeInvoke(Delegate d)
		{
			Control invokeTarget = InvokeTarget;
			if (invokeTarget != null)
			{
				try
				{
					invokeTarget.Invoke(d);
					return true;
				}
				catch (ObjectDisposedException exception)
				{
					ErrorHandling.Error(exception, ErrorLevel.Log);
				}
				catch (InvalidOperationException exception2)
				{
					ErrorHandling.Error(exception2, "An invoke target may have been assigned before its window handle had been created.\nDo not assign a control as an invoke target in that controls constructor!", ErrorLevel.SendReport);
				}
			}
			return false;
		}

		protected bool SafeInvoke(Delegate d, params object[] args)
		{
			Control invokeTarget = InvokeTarget;
			if (invokeTarget != null)
			{
				try
				{
					invokeTarget.Invoke(d, args);
					return true;
				}
				catch (ObjectDisposedException exception)
				{
					ErrorHandling.Error(exception, ErrorLevel.Log);
				}
				catch (InvalidOperationException exception2)
				{
					ErrorHandling.Error(exception2, "An invoke target may have been assigned before its window handle had been created.\nDo not assign a control as an invoke target in that controls constructor!", ErrorLevel.SendReport);
				}
			}
			return false;
		}
	}
}
