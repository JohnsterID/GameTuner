using System.Windows.Forms;

namespace GameTuner
{
	public class FloatControl : ValueControl
	{
		private float m_fDefault;

		public float DefaultValue
		{
			get
			{
				return m_fDefault;
			}
			set
			{
				m_fDefault = value;
			}
		}

		public float Value
		{
			get
			{
				float result = m_fDefault;
				float.TryParse(m_txt.Text, out result);
				return result;
			}
			set
			{
				m_txt.Text = value.ToString();
			}
		}

		public FloatControl(MouseEventHandler onMouseClick, MouseEventHandler onMouseMove)
			: base(onMouseClick, onMouseMove)
		{
		}

		protected override string ValidateInput(string s)
		{
			float result;
			if (s != string.Empty && !float.TryParse(s, out result))
			{
				return DefaultValue.ToString();
			}
			return s;
		}

		protected override void ValueUpdated(string sNewVal)
		{
			float result;
			float result2;
			if (!float.TryParse(m_txt.Text, out result) || !float.TryParse(sNewVal, out result2) || result != result2)
			{
				m_txt.Text = sNewVal;
			}
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
			customUI.EditFloatControl(this);
		}
	}
}
