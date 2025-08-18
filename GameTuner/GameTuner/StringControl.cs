using System.Windows.Forms;

namespace GameTuner
{
	public class StringControl : ValueControl
	{
		public string Value
		{
			get
			{
				return m_txt.Text;
			}
			set
			{
				m_txt.Text = value;
			}
		}

		public StringControl(MouseEventHandler onMouseClick, MouseEventHandler onMouseMove)
			: base(onMouseClick, onMouseMove)
		{
		}

		protected override void EditControl()
		{
			CustomUI customUI = (CustomUI)base.Parent;
			customUI.EditStringControl(this);
		}

		protected override void ValueUpdated(string sNewVal)
		{
			if (sNewVal != m_txt.Text)
			{
				m_txt.Text = sNewVal;
				m_txt.SelectionStart = m_txt.Text.Length;
			}
		}
	}
}
