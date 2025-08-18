using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework.Web
{
	public class WebServer
	{
		private EventWaitHandle exitHandle;

		private EventWaitHandle doneHandle;

		private Thread workerThread;

		private WebServerHandler prePageServe;

		private WebServerHandler postPageServe;

		private WebServerHandler noPageServed;

		public WebServerHandler PrePageServeHandler
		{
			get
			{
				return prePageServe;
			}
			set
			{
				prePageServe = value ?? new WebServerHandler(defaultPrePage);
			}
		}

		public WebServerHandler PostPageServeHandler
		{
			get
			{
				return postPageServe;
			}
			set
			{
				postPageServe = value ?? new WebServerHandler(defaultPostPage);
			}
		}

		public WebServerHandler NoPageServedHandler
		{
			get
			{
				return noPageServed;
			}
			set
			{
				noPageServed = value ?? new WebServerHandler(defaultNoPage);
			}
		}

		public string RootFolder { get; set; }

		private HttpListener Listener { get; set; }

		public Dictionary<string, IWebPage> WebPages { get; private set; }

		public bool IsListening
		{
			get
			{
				return Listener.IsListening;
			}
		}

		public WebServer()
			: this(GameTuner.Framework.Properties.Resources.WebPort, ApplicationHelper.LocalUserCommonAppDataPath)
		{
		}

		public WebServer(string uriPrefix, string rootFolder)
		{
			ThreadPool.SetMaxThreads(50, 1000);
			ThreadPool.SetMinThreads(50, 50);
			PrePageServeHandler = null;
			PostPageServeHandler = null;
			NoPageServedHandler = null;
			exitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
			doneHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
			WebPages = new Dictionary<string, IWebPage>();
			Listener = new HttpListener();
			Listener.Prefixes.Add(uriPrefix);
			RootFolder = RootFolder;
		}

		public void Start()
		{
			if (!IsListening)
			{
				exitHandle.Reset();
				doneHandle.Reset();
				workerThread = new Thread(WorkerThread);
				workerThread.Start();
			}
		}

		public void Stop()
		{
			if (IsListening && workerThread != null && workerThread.IsAlive)
			{
				exitHandle.Set();
				doneHandle.WaitOne();
			}
		}

		private void ListenerCallback(IAsyncResult result)
		{
			if (Listener.IsListening)
			{
				HttpListenerContext state = Listener.EndGetContext(result);
				ThreadPool.QueueUserWorkItem(ProcessRequest, state);
			}
		}

		private void ProcessRequest(object param)
		{
			HttpListenerContext httpListenerContext = (HttpListenerContext)param;
			HttpListenerRequest request = httpListenerContext.Request;
			HttpListenerResponse response = httpListenerContext.Response;
			WebServerEventArgs e = new WebServerEventArgs(request, response);
			PrePageServeHandler(this, e);
			string requestFile = WebHelper.GetRequestFile(request);
			IWebPage value;
			if (WebPages.TryGetValue(requestFile, out value))
			{
				value.ServePage(this, request.HttpMethod, e);
			}
			else
			{
				NoPageServedHandler(this, e);
			}
			PostPageServeHandler(this, e);
		}

		private void WorkerThread()
		{
			try
			{
				Listener.Start();
				bool flag = false;
				while (!flag)
				{
					IAsyncResult asyncResult = Listener.BeginGetContext(ListenerCallback, Listener);
					WaitHandle[] waitHandles = new WaitHandle[2] { exitHandle, asyncResult.AsyncWaitHandle };
					flag = WaitHandle.WaitAny(waitHandles) == 0;
				}
				Listener.Stop();
			}
			catch (Exception e)
			{
				ExceptionLogger.Log(e);
			}
			finally
			{
				doneHandle.Set();
			}
		}

		private void defaultPrePage(object sender, WebServerEventArgs e)
		{
		}

		private void defaultPostPage(object sender, WebServerEventArgs e)
		{
		}

		private void defaultNoPage(object sender, WebServerEventArgs e)
		{
			WebHelper.SendResponse(e.Response, GameTuner.Framework.Properties.Resources.WebResultNoPage);
		}
	}
}
