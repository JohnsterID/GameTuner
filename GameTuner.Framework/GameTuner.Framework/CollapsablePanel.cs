using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public class CollapsablePanel : UserControl
	{
		public delegate bool OnCloseClickedHandler(CollapsablePanel sender);

		private bool m_bWidthLocked;

		private bool m_bMinimized;

		private Control m_Content;

		public OnCloseClickedHandler OnCloseClicked;

		private IContainer components;

		private ToolStrip m_ToolStrip;

		private ToolStripButton m_btnMinimize;

		private ToolStripLabel m_lblTitle;

		private ToolStripButton m_btnClose;

		public string TitleText
		{
			get
			{
				return m_lblTitle.Text;
			}
			set
			{
				m_lblTitle.Text = value;
			}
		}

		public bool Minimized
		{
			get
			{
				return m_bMinimized;
			}
			set
			{
				if (value == m_bMinimized)
				{
					return;
				}
				m_bMinimized = value;
				if (m_bMinimized)
				{
					if (m_Content != null)
					{
						base.Controls.Remove(m_Content);
					}
					base.Height = m_ToolStrip.Height + 1;
					m_btnMinimize.Text = "+";
				}
				else
				{
					if (m_Content != null)
					{
						base.Height = m_ToolStrip.Height + m_Content.Height;
						base.Controls.Add(m_Content);
					}
					m_btnMinimize.Text = "-";
				}
			}
		}

		public bool WidthLocked
		{
			get
			{
				return m_bWidthLocked;
			}
			set
			{
				m_bWidthLocked = value;
			}
		}

		private Control Content
		{
			get
			{
				return m_Content;
			}
			set
			{
				if (m_Content != null)
				{
					base.Controls.Remove(m_Content);
				}
				m_Content = value;
				if (m_Content != null)
				{
					m_Content.Location = new Point(0, m_ToolStrip.Height);
					if (!m_bWidthLocked && base.Width < m_Content.Width)
					{
						base.Width = m_Content.Width;
					}
					if (!m_bMinimized)
					{
						base.Height = m_ToolStrip.Height + m_Content.Height;
						base.Controls.Add(m_Content);
					}
				}
				else
				{
					base.Height = m_ToolStrip.Height + 1;
				}
			}
		}

		public void SetContent(Control content)
		{
			Content = content;
		}

		public Control GetContent()
		{
			return Content;
		}

		public CollapsablePanel()
		{
			InitializeComponent();
		}

		private void m_btnMinimize_Click(object sender, EventArgs e)
		{
			Minimized = !Minimized;
		}

		private void m_btnClose_Click(object sender, EventArgs e)
		{
			if ((OnCloseClicked == null || OnCloseClicked(this)) && base.Parent != null)
			{
				base.Parent.Controls.Remove(this);
			}
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameTuner.Framework.CollapsablePanel));
			this.m_ToolStrip = new System.Windows.Forms.ToolStrip();
			this.m_btnMinimize = new System.Windows.Forms.ToolStripButton();
			this.m_lblTitle = new System.Windows.Forms.ToolStripLabel();
			this.m_btnClose = new System.Windows.Forms.ToolStripButton();
			this.m_ToolStrip.SuspendLayout();
			base.SuspendLayout();
			this.m_ToolStrip.BackColor = System.Drawing.Color.CornflowerBlue;
			this.m_ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.m_btnClose, this.m_btnMinimize, this.m_lblTitle });
			this.m_ToolStrip.Location = new System.Drawing.Point(0, 0);
			this.m_ToolStrip.Name = "m_ToolStrip";
			this.m_ToolStrip.Size = new System.Drawing.Size(503, 25);
			this.m_ToolStrip.TabIndex = 0;
			this.m_ToolStrip.Text = "toolStrip1";
			this.m_btnMinimize.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.m_btnMinimize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_btnMinimize.Image = (System.Drawing.Image)resources.GetObject("m_btnMinimize.Image");
			this.m_btnMinimize.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_btnMinimize.Name = "m_btnMinimize";
			this.m_btnMinimize.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.m_btnMinimize.Size = new System.Drawing.Size(23, 22);
			this.m_btnMinimize.Text = "-";
			this.m_btnMinimize.ToolTipText = "Hide / Show";
			this.m_btnMinimize.Click += new System.EventHandler(m_btnMinimize_Click);
			this.m_lblTitle.Name = "m_lblTitle";
			this.m_lblTitle.Size = new System.Drawing.Size(0, 22);
			this.m_btnClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.m_btnClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_btnClose.Image = (System.Drawing.Image)resources.GetObject("m_btnClose.Image");
			this.m_btnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_btnClose.Name = "m_btnClose";
			this.m_btnClose.Size = new System.Drawing.Size(23, 22);
			this.m_btnClose.Text = "X";
			this.m_btnClose.ToolTipText = "Close";
			this.m_btnClose.Click += new System.EventHandler(m_btnClose_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			base.Controls.Add(this.m_ToolStrip);
			base.Name = "CollapsablePanel";
			base.Size = new System.Drawing.Size(503, 24);
			this.m_ToolStrip.ResumeLayout(false);
			this.m_ToolStrip.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
