using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework
{
	public class ScriptCodeControl : UserControl
	{
		private IContainer components;

		private RichTextBox richTextBox;

		private CaptionControl captionControl;

		[Browsable(false)]
		public string ScriptCode
		{
			get
			{
				return richTextBox.Text;
			}
			set
			{
				richTextBox.Text = value;
			}
		}

		public ScriptCodeControl()
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
			this.richTextBox = new System.Windows.Forms.RichTextBox();
			this.captionControl = new GameTuner.Framework.CaptionControl();
			base.SuspendLayout();
			this.richTextBox.AcceptsTab = true;
			this.richTextBox.AutoWordSelection = true;
			this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox.Font = new System.Drawing.Font("Courier New", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.richTextBox.Location = new System.Drawing.Point(0, 0);
			this.richTextBox.Name = "richTextBox";
			this.richTextBox.Size = new System.Drawing.Size(506, 298);
			this.richTextBox.TabIndex = 0;
			this.richTextBox.Text = "";
			this.richTextBox.WordWrap = false;
			this.captionControl.BackColor = System.Drawing.SystemColors.ControlDark;
			this.captionControl.Caption = "Script";
			this.captionControl.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.captionControl.Image = GameTuner.Framework.Properties.Resources.VSProject_script;
			this.captionControl.Location = new System.Drawing.Point(0, 298);
			this.captionControl.Name = "captionControl";
			this.captionControl.Size = new System.Drawing.Size(506, 27);
			this.captionControl.TabIndex = 1;
			this.captionControl.Transparent = System.Drawing.Color.Magenta;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.richTextBox);
			base.Controls.Add(this.captionControl);
			base.Name = "ScriptCodeControl";
			base.Size = new System.Drawing.Size(506, 325);
			base.ResumeLayout(false);
		}
	}
}
