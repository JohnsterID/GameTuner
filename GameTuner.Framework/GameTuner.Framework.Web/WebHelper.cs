using System.IO;
using System.Net;
using System.Text;

namespace GameTuner.Framework.Web
{
	public static class WebHelper
	{
		public static void SendResponse(HttpListenerResponse response, string message)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(message);
			response.ContentLength64 = bytes.Length;
			Stream outputStream = response.OutputStream;
			outputStream.Write(bytes, 0, bytes.Length);
			outputStream.Close();
		}

		public static string GetRequestFile(HttpListenerRequest request)
		{
			string text = request.RawUrl;
			int startIndex;
			if ((startIndex = text.IndexOf('?')) != -1)
			{
				text = text.Remove(startIndex);
			}
			if (text.StartsWith("\\") || text.StartsWith("/"))
			{
				text = text.Substring(1);
			}
			return text;
		}
	}
}
