using System.Windows.Forms;

namespace GameTuner
{
	public class IntegerControl : ValueControl
	{
		private int m_iDefault;

		public int DefaultValue
		{
			get
			{
				return m_iDefault;
			}
			set
			{
				m_iDefault = value;
			}
		}

		public int Value
		{
			get
			{
				int result = m_iDefault;
				int.TryParse(m_txt.Text, out result);
				return result;
			}
			set
			{
				m_txt.Text = value.ToString();
			}
		}

		public IntegerControl(MouseEventHandler onMouseClick, MouseEventHandler onMouseMove)
			: base(onMouseClick, onMouseMove)
		{
		}

		protected override string ValidateInput(string s)
		{
			if (s != string.Empty)
			{
				int result = m_iDefault;
				if (!int.TryParse(s, out result))
				{
					float result2;
					if (float.TryParse(s, out result2))
					{
						result = (int)result2;
					}
					return result.ToString();
				}
			}
			return s;
		}

		protected override string GetValueType()
		{
			return "num";
		}

		protected override string GetValue()
		{
			return Value.ToString();
		}

		protected override void EditControl()
		{
			CustomUI customUI = (CustomUI)base.Parent;
			customUI.EditIntegerControl(this);
		}
	}
}
