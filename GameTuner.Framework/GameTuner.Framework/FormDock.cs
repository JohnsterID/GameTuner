using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public class FormDock : UserControl
	{
		private Form m_Form;

		private FormBorderStyle m_PrevBorderStyle;

		private IContainer components;

		public Form Form
		{
			get
			{
				return m_Form;
			}
			set
			{
				if (m_Form != value)
				{
					if (m_Form != null)
					{
						m_Form.Hide();
						base.Controls.Remove(m_Form);
						m_Form.TopLevel = true;
						m_Form.FormBorderStyle = m_PrevBorderStyle;
						m_Form.Show();
					}
					m_Form = value;
					if (m_Form != null)
					{
						m_Form.Hide();
						m_PrevBorderStyle = m_Form.FormBorderStyle;
						m_Form.FormBorderStyle = FormBorderStyle.None;
						m_Form.TopLevel = false;
						m_Form.Dock = DockStyle.Fill;
						base.Controls.Add(m_Form);
						m_Form.Show();
					}
				}
			}
		}

		public FormDock()
		{
			InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Name = "FormDock";
			base.Size = new System.Drawing.Size(759, 477);
			base.ResumeLayout(false);
		}
	}
}
