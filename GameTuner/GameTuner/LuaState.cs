namespace GameTuner
{
	public class LuaState
	{
		private string m_sName;

		private uint m_uiID;

		public string Name
		{
			get
			{
				return m_sName;
			}
		}

		public uint ID
		{
			get
			{
				return m_uiID;
			}
		}

		public LuaState(string sName, uint uiID)
		{
			m_sName = sName;
			m_uiID = uiID;
		}

		public override string ToString()
		{
			return m_sName;
		}
	}
}
