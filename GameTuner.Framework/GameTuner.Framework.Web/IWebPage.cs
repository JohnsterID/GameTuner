namespace GameTuner.Framework.Web
{
	public interface IWebPage
	{
		bool Hidden { get; }

		void ServePage(object sender, string method, WebServerEventArgs args);
	}
}
