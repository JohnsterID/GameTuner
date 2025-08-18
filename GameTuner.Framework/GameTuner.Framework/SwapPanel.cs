using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public class SwapPanel : UserControl
	{
		private Control m_Content;

		private IContainer components;

		public Control Content
		{
			get
			{
				return m_Content;
			}
			set
			{
				if (m_Content != value)
				{
					if (m_Content != null)
					{
						base.Controls.Remove(m_Content);
					}
					m_Content = value;
					if (m_Content != null)
					{
						m_Content.Dock = DockStyle.Fill;
						m_Content.Hide();
						base.Controls.Add(m_Content);
						m_Content.Show();
					}
					EventHandler contentChanged = this.ContentChanged;
					if (contentChanged != null)
					{
						contentChanged(this, EventArgs.Empty);
					}
				}
			}
		}

		public event EventHandler ContentChanged;

		public SwapPanel()
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
			base.Name = "SwapPanel";
			base.ResumeLayout(false);
		}
	}
}
