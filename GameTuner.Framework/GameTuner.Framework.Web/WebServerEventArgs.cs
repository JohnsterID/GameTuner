using System;
using System.Net;

namespace GameTuner.Framework.Web
{
	public class WebServerEventArgs : EventArgs
	{
		public bool Handled { get; set; }

		public HttpListenerRequest Request { get; private set; }

		public HttpListenerResponse Response { get; private set; }

		public WebServerEventArgs(HttpListenerRequest request, HttpListenerResponse response)
		{
			Request = request;
			Response = response;
			Handled = false;
		}
	}
}
