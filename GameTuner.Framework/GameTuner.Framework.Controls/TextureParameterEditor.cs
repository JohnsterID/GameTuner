using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GameTuner.Framework.Controls
{
	public class TextureParameterEditor : UserControl, IParameterEditor
	{
		private string m_szTextureFilename;

		private IContainer components;

		private SplitContainer splitContainer1;

		private Label label1;

		private CheckBox TextureEnabled;

		private Button TextureButton;

		private OpenFileDialog TextureOpenFile;

		string IParameterEditor.Name
		{
			get
			{
				return label1.Text;
			}
			set
			{
				label1.Text = value;
			}
		}

		object IParameterEditor.Value
		{
			get
			{
				return m_szTextureFilename;
			}
			set
			{
				m_szTextureFilename = value as string;
				if (m_szTextureFilename != null)
				{
					TextureButton.Text = Path.GetFileName(m_szTextureFilename);
				}
				else
				{
					TextureButton.Text = "None";
				}
			}
		}

		public event EventHandler ValueChanged;

		public TextureParameterEditor()
		{
			InitializeComponent();
		}

		private void TextureButton_Click(object sender, EventArgs e)
		{
			if (TextureOpenFile.ShowDialog() == DialogResult.OK)
			{
				m_szTextureFilename = TextureOpenFile.FileName;
				TextureButton.Text = Path.GetFileName(m_szTextureFilename);
				if (this.ValueChanged != null)
				{
					this.ValueChanged(this, EventArgs.Empty);
				}
			}
		}

		private void TextureButton_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				m_szTextureFilename = null;
				TextureButton.Text = "None";
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.label1 = new System.Windows.Forms.Label();
			this.TextureEnabled = new System.Windows.Forms.CheckBox();
			this.TextureButton = new System.Windows.Forms.Button();
			this.TextureOpenFile = new System.Windows.Forms.OpenFileDialog();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			base.SuspendLayout();
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel2.Controls.Add(this.TextureEnabled);
			this.splitContainer1.Panel2.Controls.Add(this.TextureButton);
			this.splitContainer1.Size = new System.Drawing.Size(312, 25);
			this.splitContainer1.SplitterDistance = 96;
			this.splitContainer1.TabIndex = 0;
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "label1";
			this.TextureEnabled.AutoSize = true;
			this.TextureEnabled.Location = new System.Drawing.Point(191, 5);
			this.TextureEnabled.Name = "TextureEnabled";
			this.TextureEnabled.Size = new System.Drawing.Size(15, 14);
			this.TextureEnabled.TabIndex = 1;
			this.TextureEnabled.UseVisualStyleBackColor = true;
			this.TextureButton.Location = new System.Drawing.Point(3, 0);
			this.TextureButton.Name = "TextureButton";
			this.TextureButton.Size = new System.Drawing.Size(182, 23);
			this.TextureButton.TabIndex = 0;
			this.TextureButton.Text = "None";
			this.TextureButton.UseVisualStyleBackColor = true;
			this.TextureButton.Click += new System.EventHandler(TextureButton_Click);
			this.TextureButton.MouseDown += new System.Windows.Forms.MouseEventHandler(TextureButton_MouseDown);
			this.TextureOpenFile.Title = "Open Texture File";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.splitContainer1);
			base.Name = "TextureParameterEditor";
			base.Size = new System.Drawing.Size(312, 25);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		object IParameterEditor.Tag
		{
			get
			{
				return base.Tag;
			}
			set
			{
				base.Tag = value;
			}
		}
	}
}
