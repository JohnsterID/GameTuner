using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework.Controls.Wizard
{
	public class WizardForm : Form
	{
		private int m_iCurrentPageIndex;

		private IContainer components;

		private Panel panel1;

		private Button ButtonCancelFinish;

		private Button ButtonNext;

		private Button ButtonPrevious;

		private Panel panel2;

		private CaptionControl captionControl1;

		public List<WizardPage> PageList { get; private set; }

		public int CurrentPageIndex
		{
			get
			{
				return m_iCurrentPageIndex;
			}
			private set
			{
				if (value >= 0 && value < PageList.Count)
				{
					PageList[m_iCurrentPageIndex].OnHide();
					m_iCurrentPageIndex = value;
					SetPageCurrent(PageList[m_iCurrentPageIndex]);
					m_iCurrentPageIndex = value;
					PageList[m_iCurrentPageIndex].OnShow();
					OnUpdateCaption();
					OnCurrentPageChanged(new WizardPageEventArgs(PageList[m_iCurrentPageIndex]));
				}
			}
		}

		public WizardPage CurrentPage
		{
			get
			{
				return PageList[CurrentPageIndex];
			}
		}

		public event WizardPageEventHandler CurrentPageChanged;

		public event WizardPageEventHandler ButtonStateUpdated;

		public WizardForm()
		{
			InitializeComponent();
			PageList = new List<WizardPage>();
			AutoSize = false;
			CurrentPageIndex = 0;
		}

		public void SetPageCurrent(WizardPage CurrentPage)
		{
			SuspendLayout();
			panel1.Controls.Clear();
			panel1.Controls.Add(CurrentPage);
			OnButtonStateUpdated(new WizardPageEventArgs(CurrentPage));
			Size size = CurrentPage.Size - panel1.Size;
			panel1.Size = CurrentPage.Size;
			base.Size += size;
			ResumeLayout();
			OnUpdateCaption();
		}

		public bool BeginWizard()
		{
			if (PageList.Count == 0)
			{
				return false;
			}
			foreach (WizardPage page in PageList)
			{
				page.PageStatusChanged += Page_PageStatusChanged;
			}
			SetPageCurrent(PageList[CurrentPageIndex]);
			PageList[CurrentPageIndex].OnShow();
			return true;
		}

		private void Page_PageStatusChanged(object sender, WizardPageEventArgs e)
		{
			OnButtonStateUpdated(e);
		}

		public virtual bool OnCancelFinish()
		{
			PageList[CurrentPageIndex].OnHide();
			foreach (WizardPage page in PageList)
			{
				page.PageStatusChanged -= Page_PageStatusChanged;
			}
			Close();
			return true;
		}

		protected virtual void OnButtonStateUpdated(WizardPageEventArgs e)
		{
			WizardPage page = e.Page;
			if (page != null)
			{
				page.DictateButtonStatus(ButtonPrevious, ButtonNext, ButtonCancelFinish);
				WizardPageEventHandler buttonStateUpdated = this.ButtonStateUpdated;
				if (buttonStateUpdated != null)
				{
					buttonStateUpdated(this, e);
				}
			}
		}

		protected virtual void OnUpdateCaption()
		{
			captionControl1.Caption = string.Format("Step {0} of {1}", m_iCurrentPageIndex + 1, PageList.Count);
		}

		protected virtual void OnCurrentPageChanged(WizardPageEventArgs e)
		{
			e.Page.DictateButtonStatus(ButtonPrevious, ButtonNext, ButtonCancelFinish);
			WizardPageEventHandler currentPageChanged = this.CurrentPageChanged;
			if (currentPageChanged != null)
			{
				currentPageChanged(this, e);
			}
		}

		private void ButtonPrevious_Click(object sender, EventArgs e)
		{
			CurrentPageIndex--;
		}

		private void ButtonNext_Click(object sender, EventArgs e)
		{
			CurrentPageIndex++;
		}

		private void ButtonCancelFinish_Click(object sender, EventArgs e)
		{
			OnCancelFinish();
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
			this.ButtonCancelFinish = new System.Windows.Forms.Button();
			this.ButtonNext = new System.Windows.Forms.Button();
			this.ButtonPrevious = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.captionControl1 = new GameTuner.Framework.CaptionControl();
			this.panel2.SuspendLayout();
			base.SuspendLayout();
			this.ButtonCancelFinish.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.ButtonCancelFinish.Location = new System.Drawing.Point(289, 3);
			this.ButtonCancelFinish.Name = "ButtonCancelFinish";
			this.ButtonCancelFinish.Size = new System.Drawing.Size(74, 23);
			this.ButtonCancelFinish.TabIndex = 3;
			this.ButtonCancelFinish.Text = "Cancel";
			this.ButtonCancelFinish.UseVisualStyleBackColor = true;
			this.ButtonCancelFinish.Click += new System.EventHandler(ButtonCancelFinish_Click);
			this.ButtonNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this.ButtonNext.Location = new System.Drawing.Point(83, 3);
			this.ButtonNext.Name = "ButtonNext";
			this.ButtonNext.Size = new System.Drawing.Size(74, 23);
			this.ButtonNext.TabIndex = 1;
			this.ButtonNext.Text = "Next >";
			this.ButtonNext.UseVisualStyleBackColor = true;
			this.ButtonNext.Click += new System.EventHandler(ButtonNext_Click);
			this.ButtonPrevious.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this.ButtonPrevious.Location = new System.Drawing.Point(3, 3);
			this.ButtonPrevious.Name = "ButtonPrevious";
			this.ButtonPrevious.Size = new System.Drawing.Size(74, 23);
			this.ButtonPrevious.TabIndex = 0;
			this.ButtonPrevious.Text = "< Previous";
			this.ButtonPrevious.UseVisualStyleBackColor = true;
			this.ButtonPrevious.Click += new System.EventHandler(ButtonPrevious_Click);
			this.panel1.AutoSize = true;
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 27);
			this.panel1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 5);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(366, 148);
			this.panel1.TabIndex = 0;
			this.panel2.BackColor = System.Drawing.SystemColors.ControlLight;
			this.panel2.Controls.Add(this.ButtonCancelFinish);
			this.panel2.Controls.Add(this.ButtonPrevious);
			this.panel2.Controls.Add(this.ButtonNext);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 175);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(366, 29);
			this.panel2.TabIndex = 4;
			this.captionControl1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.captionControl1.Caption = "Step 1 of X";
			this.captionControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.captionControl1.Image = GameTuner.Framework.Properties.Resources.InsertPage;
			this.captionControl1.Location = new System.Drawing.Point(0, 0);
			this.captionControl1.Name = "captionControl1";
			this.captionControl1.Size = new System.Drawing.Size(366, 27);
			this.captionControl1.TabIndex = 5;
			this.captionControl1.Transparent = System.Drawing.Color.Magenta;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			base.ClientSize = new System.Drawing.Size(366, 204);
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.captionControl1);
			base.Controls.Add(this.panel2);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(324, 162);
			base.Name = "WizardForm";
			this.Text = "WizardForm";
			this.panel2.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
