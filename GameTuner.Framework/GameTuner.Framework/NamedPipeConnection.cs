using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace GameTuner.Framework
{
	public class NamedPipeConnection : IDisposable
	{
		private class InputHandler
		{
			public bool ConnectionOpen;

			public bool Server;

			public string Name = string.Empty;

			public bool Reading;

			public int BytesRead;

			public byte[] ReadBuffer = new byte[2048];

			public object InputBufferLock = new object();

			public List<string> InputBuffer = new List<string>();

			public Thread InputThread;

			public PipeStream InputStream;

			public void InputThreadMain()
			{
				while (ConnectionOpen)
				{
					try
					{
						if (InputStream == null)
						{
							if (Server)
							{
								InputStream = new NamedPipeServerStream(Name + "Input", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
								((NamedPipeServerStream)InputStream).WaitForConnection();
							}
							else
							{
								InputStream = new NamedPipeClientStream(Name + "Output");
								((NamedPipeClientStream)InputStream).Connect();
							}
						}
						if (!InputStream.IsConnected)
						{
							InputStream.Dispose();
							InputStream = null;
						}
						else if (!Reading)
						{
							Reading = true;
							InputStream.BeginRead(ReadBuffer, BytesRead, ReadBuffer.Length - BytesRead, ReadComplete, null);
						}
						else
						{
							Thread.Sleep(100);
						}
					}
					catch (Exception exception)
					{
						ErrorHandling.Error(exception, ErrorLevel.Log);
					}
				}
				if (InputStream != null && InputStream.IsConnected && Server)
				{
					((NamedPipeServerStream)InputStream).Disconnect();
				}
			}

			private void ReadComplete(IAsyncResult result)
			{
				if (InputStream == null)
				{
					return;
				}
				BytesRead += InputStream.EndRead(result);
				if (BytesRead > 0 && ReadBuffer[BytesRead - 1] == 0)
				{
					string text = string.Empty;
					for (int i = 0; i < BytesRead - 1; i++)
					{
						text += (char)ReadBuffer[i];
					}
					lock (InputBufferLock)
					{
						InputBuffer.Add(text);
					}
					BytesRead = 0;
				}
				Reading = false;
			}
		}

		private class OutputHandler
		{
			public bool ConnectionOpen;

			public bool Server;

			public string Name = string.Empty;

			public object OutputBufferLock = new object();

			public List<string> OutputBuffer = new List<string>();

			public List<string> WorkingOutput = new List<string>();

			public Thread OutputThread;

			public PipeStream OutputStream;

			public void OutputThreadMain()
			{
				BinaryWriter binaryWriter = null;
				while (ConnectionOpen)
				{
					try
					{
						if (OutputStream == null)
						{
							if (Server)
							{
								OutputStream = new NamedPipeServerStream(Name + "Output", PipeDirection.InOut);
								((NamedPipeServerStream)OutputStream).WaitForConnection();
								binaryWriter = new BinaryWriter(OutputStream);
							}
							else
							{
								OutputStream = new NamedPipeClientStream(Name + "Input");
								((NamedPipeClientStream)OutputStream).Connect();
								binaryWriter = new BinaryWriter(OutputStream);
							}
						}
						if (!OutputStream.IsConnected)
						{
							OutputStream.Dispose();
							OutputStream = null;
							continue;
						}
						lock (OutputBufferLock)
						{
							List<string> workingOutput = WorkingOutput;
							WorkingOutput = OutputBuffer;
							OutputBuffer = workingOutput;
						}
						if (WorkingOutput.Count == 0)
						{
							Thread.Sleep(100);
							continue;
						}
						foreach (string item in WorkingOutput)
						{
							string text = item;
							foreach (char c in text)
							{
								binaryWriter.Write((byte)c);
							}
							binaryWriter.Write((byte)0);
						}
						WorkingOutput.Clear();
					}
					catch (Exception exception)
					{
						ErrorHandling.Error(exception, ErrorLevel.Log);
					}
				}
				if (OutputStream.IsConnected && Server)
				{
					((NamedPipeServerStream)OutputStream).Disconnect();
				}
			}
		}

		private bool m_bOpen;

		private InputHandler InputHandlerInstance = new InputHandler();

		private OutputHandler OutputHandlerInstance = new OutputHandler();

		public string Name { get; private set; }

		public bool Server { get; private set; }

		public bool Open
		{
			get
			{
				return m_bOpen;
			}
			set
			{
				if (m_bOpen == value)
				{
					return;
				}
				m_bOpen = value;
				InputHandlerInstance.ConnectionOpen = m_bOpen;
				OutputHandlerInstance.ConnectionOpen = m_bOpen;
				if (m_bOpen)
				{
					if (InputHandlerInstance.InputThread != null && InputHandlerInstance.InputThread.IsAlive)
					{
						InputHandlerInstance.InputThread.Abort();
					}
					if (OutputHandlerInstance.OutputThread != null && OutputHandlerInstance.OutputThread.IsAlive)
					{
						OutputHandlerInstance.OutputThread.Abort();
					}
					InputHandlerInstance.InputThread = new Thread(InputHandlerInstance.InputThreadMain);
					InputHandlerInstance.InputThread.Name = "Pipe Input Thread";
					OutputHandlerInstance.OutputThread = new Thread(OutputHandlerInstance.OutputThreadMain);
					OutputHandlerInstance.OutputThread.Name = "Pipe Output Thread";
					InputHandlerInstance.InputThread.Start();
					OutputHandlerInstance.OutputThread.Start();
				}
			}
		}

		public NamedPipeConnection(string sName, bool bServer)
		{
			Name = sName;
			Server = bServer;
			InputHandlerInstance.Name = Name;
			InputHandlerInstance.Server = Server;
			OutputHandlerInstance.Name = Name;
			OutputHandlerInstance.Server = Server;
		}

		public void Dispose()
		{
			Open = false;
		}

		public void Send(string s)
		{
			lock (OutputHandlerInstance.OutputBufferLock)
			{
				OutputHandlerInstance.OutputBuffer.Add(s);
			}
		}

		public bool Recieve(out IEnumerable<string> list)
		{
			lock (InputHandlerInstance.InputBufferLock)
			{
				if (InputHandlerInstance.InputBuffer.Count > 0)
				{
					list = InputHandlerInstance.InputBuffer;
					InputHandlerInstance.InputBuffer = new List<string>();
				}
				else
				{
					list = null;
				}
			}
			return list != null;
		}
	}
}
