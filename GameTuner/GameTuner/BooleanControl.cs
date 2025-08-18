using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner
{
	public class BooleanControl : ValueControl
	{
		private CheckBox m_chkValue = new CheckBox();

		public bool Value
		{
			get
			{
				return m_chkValue.Checked;
			}
			set
			{
				m_chkValue.Checked = value;
			}
		}

		public BooleanControl(MouseEventHandler onMouseClick, MouseEventHandler onMouseMove)
			: base(onMouseClick, onMouseMove)
		{
			m_txt.Hide();
			m_chkValue.Location = new Point(m_lbl.Width + 4, 0);
			m_chkValue.Width = 16;
			m_chkValue.CheckedChanged += m_chkValue_CheckedChanged;
			base.Controls.Add(m_chkValue);
		}

		protected override void OnLabelResized()
		{
			m_chkValue.Location = new Point(m_lbl.Width + 4, 0);
			base.Width = m_lbl.Width + m_chkValue.Width + 12;
		}

		protected override void ResizeToFitText()
		{
		}

		private void m_chkValue_CheckedChanged(object sender, EventArgs e)
		{
			m_txt.Text = (Value ? "true" : "false");
		}

		protected override void ValueUpdated(string sNewVal)
		{
			Value = sNewVal == "true";
		}

		protected override string GetValueType()
		{
			return "bool";
		}

		protected override string GetValue()
		{
			if (!Value)
			{
				return "false";
			}
			return "true";
		}

		protected override void HideControls()
		{
			m_lbl.Hide();
			m_chkValue.Hide();
		}

		protected override void ShowControls()
		{
			m_lbl.Show();
			m_chkValue.Show();
		}

		protected override void EditControl()
		{
			CustomUI customUI = (CustomUI)base.Parent;
			customUI.EditBooleanControl(this);
		}
	}
}
