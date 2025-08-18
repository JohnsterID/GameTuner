using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner.Framework.Controls.Wizard
{
	public class WizardPage : UserControl
	{
		public enum EPageType
		{
			Start,
			Normal,
			Finish
		}

		public EPageType PageType = EPageType.Normal;

		private IContainer components;

		public event WizardPageEventHandler PageStatusChanged;

		protected void OnPageStatusChanged()
		{
			WizardPageEventHandler pageStatusChanged = this.PageStatusChanged;
			if (pageStatusChanged != null)
			{
				pageStatusChanged(this, new WizardPageEventArgs(this));
			}
		}

		public WizardPage()
		{
			InitializeComponent();
		}

		public virtual void OnShow()
		{
		}

		public virtual void OnHide()
		{
		}

		public virtual void DictateButtonStatus(Control ButtonPrevious, Control ButtonNext, Control ButtonCancelFinish)
		{
			switch (PageType)
			{
			case EPageType.Start:
				ButtonPrevious.Visible = false;
				ButtonNext.Visible = true;
				ButtonNext.Enabled = true;
				ButtonCancelFinish.Enabled = true;
				ButtonCancelFinish.Text = "Cancel";
				break;
			default:
				ButtonPrevious.Visible = true;
				ButtonNext.Visible = true;
				ButtonNext.Enabled = true;
				ButtonCancelFinish.Enabled = true;
				ButtonCancelFinish.Text = "Cancel";
				break;
			case EPageType.Finish:
				ButtonPrevious.Enabled = true;
				ButtonPrevious.Visible = true;
				ButtonNext.Visible = false;
				ButtonCancelFinish.Text = "Finish";
				break;
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
			base.SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			base.Name = "WizardPage";
			base.Size = new System.Drawing.Size(282, 185);
			base.ResumeLayout(false);
		}
	}
}
