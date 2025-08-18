using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner
{
	public class ProgressWnd : Form
	{
		private IContainer components;

		private ProgressBar ctrlProgress;

		private Label lblCurrentTask;

		public string Title
		{
			get
			{
				return Text;
			}
			set
			{
				Text = value;
			}
		}

		public string CurrentTask
		{
			get
			{
				return lblCurrentTask.Text;
			}
			set
			{
				lblCurrentTask.Text = value;
			}
		}

		public int Progress
		{
			get
			{
				return ctrlProgress.Value;
			}
			set
			{
				ctrlProgress.Value = value;
			}
		}

		public ProgressWnd()
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
			this.ctrlProgress = new System.Windows.Forms.ProgressBar();
			this.lblCurrentTask = new System.Windows.Forms.Label();
			base.SuspendLayout();
			this.ctrlProgress.Location = new System.Drawing.Point(12, 12);
			this.ctrlProgress.Name = "ctrlProgress";
			this.ctrlProgress.Size = new System.Drawing.Size(311, 48);
			this.ctrlProgress.TabIndex = 0;
			this.lblCurrentTask.AutoSize = true;
			this.lblCurrentTask.Location = new System.Drawing.Point(12, 63);
			this.lblCurrentTask.Name = "lblCurrentTask";
			this.lblCurrentTask.Size = new System.Drawing.Size(0, 13);
			this.lblCurrentTask.TabIndex = 1;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(338, 79);
			base.ControlBox = false;
			base.Controls.Add(this.lblCurrentTask);
			base.Controls.Add(this.ctrlProgress);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ProgressWnd";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Progress";
			base.TopMost = true;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
