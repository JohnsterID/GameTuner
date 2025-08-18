using System;
using System.Collections.Generic;

namespace GameTuner
{
	internal class LuaStateManager
	{
		public class UpdatedEventArgs : EventArgs
		{
			private List<LuaState> m_LuaStates;

			public List<LuaState> LuaStates
			{
				get
				{
					return m_LuaStates;
				}
			}

			public UpdatedEventArgs(List<LuaState> luaStates)
			{
				m_LuaStates = luaStates;
			}
		}

		public delegate void UpdatedHandler(object sender, UpdatedEventArgs e);

		private static LuaStateManager m_Instance;

		private List<LuaState> m_LuaStates = new List<LuaState>();

		private int m_iNetworkListener;

		public static LuaStateManager Instance
		{
			get
			{
				return m_Instance;
			}
		}

		public List<LuaState> LuaStates
		{
			get
			{
				return m_LuaStates;
			}
		}

		public List<string> LuaStateNames
		{
			get
			{
				List<string> list = new List<string>();
				foreach (LuaState luaState in m_LuaStates)
				{
					list.Add(luaState.Name);
				}
				return list;
			}
		}

		public event UpdatedHandler OnLuaStatesUpdated;

		private LuaStateManager()
		{
			m_iNetworkListener = Connection.Instance.AddRequestListener(OnLuaStatesRecieved);
		}

		public static void Init()
		{
			m_Instance = new LuaStateManager();
		}

		public void OnDisconnected()
		{
			m_LuaStates.Clear();
			if (frmMainForm.MainForm != null && this.OnLuaStatesUpdated != null)
			{
				this.OnLuaStatesUpdated(this, new UpdatedEventArgs(m_LuaStates));
			}
		}

		public void QueryLuaStates()
		{
			if (Connection.Instance.Connected)
			{
				Connection.Instance.Request("LSQ:", m_iNetworkListener);
				return;
			}
			m_LuaStates.Clear();
			if (this.OnLuaStatesUpdated != null)
			{
				this.OnLuaStatesUpdated(this, new UpdatedEventArgs(m_LuaStates));
			}
		}

		public void OnLuaStatesRecieved(List<string> luaStateStrings)
		{
			m_LuaStates.Clear();
			for (int i = 0; i + 1 < luaStateStrings.Count; i += 2)
			{
				uint uiID = uint.Parse(luaStateStrings[i].ToString());
				string sName = luaStateStrings[i + 1].ToString();
				LuaState item = new LuaState(sName, uiID);
				m_LuaStates.Add(item);
			}
			if (this.OnLuaStatesUpdated != null)
			{
				this.OnLuaStatesUpdated(this, new UpdatedEventArgs(m_LuaStates));
			}
		}

		public LuaState GetLuaStateByName(string sName)
		{
			foreach (LuaState luaState in LuaStates)
			{
				if (luaState.Name == sName)
				{
					return luaState;
				}
			}
			return null;
		}
	}
}
