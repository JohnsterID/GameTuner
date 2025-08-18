using System.Collections.Generic;

namespace GameTuner
{
	public interface ICustomControl
	{
		void Release();

		void LuaStateChanged(LuaState state, LuaState lastState);

		void CompletedAction(List<string> luaMessages);

		void StartDrag();

		void EndDrag();

		void TabEntered();

		void TabLeft();
	}
}
