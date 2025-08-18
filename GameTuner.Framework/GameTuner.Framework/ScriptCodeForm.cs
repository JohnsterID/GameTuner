using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework
{
	public class ScriptCodeForm : Form
	{
		private ScriptCodeHandler compileHandler;

		private IContainer components;

		private ScriptCodeControl scriptCodeControl1;

		private ToolStrip toolStrip1;

		private Button buttonCancel;

		private Button buttonOK;

		private ToolStripButton buttonCompile;

		[Browsable(false)]
		public string ScriptCode
		{
			get
			{
				return scriptCodeControl1.ScriptCode;
			}
			set
			{
				scriptCodeControl1.ScriptCode = value;
			}
		}

		[Browsable(false)]
		public ScriptCodeHandler CompileHandler
		{
			get
			{
				return compileHandler;
			}
			set
			{
				compileHandler = value;
				buttonCompile.Enabled = compileHandler != null;
			}
		}

		public ScriptCodeForm()
		{
			InitializeComponent();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.OK;
		}

		private void buttonCompile_Click(object sender, EventArgs e)
		{
			if (compileHandler != null)
			{
				compileHandler(this, new ScriptCodeArgs(ScriptCode));
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
			this.scriptCodeControl1 = new GameTuner.Framework.ScriptCodeControl();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.buttonCompile = new System.Windows.Forms.ToolStripButton();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.toolStrip1.SuspendLayout();
			base.SuspendLayout();
			this.scriptCodeControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.scriptCodeControl1.Location = new System.Drawing.Point(0, 25);
			this.scriptCodeControl1.Name = "scriptCodeControl1";
			this.scriptCodeControl1.ScriptCode = "";
			this.scriptCodeControl1.Size = new System.Drawing.Size(522, 313);
			this.scriptCodeControl1.TabIndex = 0;
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.buttonCompile });
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip1.Size = new System.Drawing.Size(522, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			this.buttonCompile.Enabled = false;
			this.buttonCompile.Image = GameTuner.Framework.Properties.Resources.compile;
			this.buttonCompile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonCompile.Name = "buttonCompile";
			this.buttonCompile.Size = new System.Drawing.Size(72, 22);
			this.buttonCompile.Text = "Compile";
			this.buttonCompile.Click += new System.EventHandler(buttonCompile_Click);
			this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(435, 344);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.buttonOK.Location = new System.Drawing.Point(354, 344);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 3;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(522, 379);
			base.Controls.Add(this.buttonOK);
			base.Controls.Add(this.buttonCancel);
			base.Controls.Add(this.scriptCodeControl1);
			base.Controls.Add(this.toolStrip1);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(237, 198);
			base.Name = "ScriptCodeForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			this.Text = "Script";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
