using System;

namespace GameTuner.Framework
{
	public class ScriptCodeArgs : EventArgs
	{
		public string Code { get; private set; }

		public ScriptCodeArgs(string code)
		{
			Code = code;
		}
	}
}
