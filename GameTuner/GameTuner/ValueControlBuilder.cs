using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner
{
	public class ValueControlBuilder : Form
	{
		public class SavedSettings
		{
			public Size WindowSize;

			public int SplitterPosition;
		}

		public static SavedSettings Settings;

		private IContainer components;

		private TextBox txtName;

		private Label lblName;

		private Button btnCancel;

		private Button btnOK;

		private CodeEditBox txtSetAction;

		private Label lblSetAction;

		private CodeEditBox txtGetAction;

		private Label lblGetAction;

		private TextBox txtDefaultValue;

		private Label lblDefaultValue;

		private SplitContainer spliter;

		public string ControlName
		{
			get
			{
				return txtName.Text;
			}
			set
			{
				txtName.Text = value;
			}
		}

		public string DefaultValue
		{
			get
			{
				return txtDefaultValue.Text;
			}
			set
			{
				txtDefaultValue.Text = value;
			}
		}

		public string SetFunction
		{
			get
			{
				return txtSetAction.Text;
			}
			set
			{
				txtSetAction.Text = value;
			}
		}

		public string GetFunction
		{
			get
			{
				return txtGetAction.Text;
			}
			set
			{
				txtGetAction.Text = value;
			}
		}

		public ValueControlBuilder()
		{
			InitializeComponent();
			if (Settings != null)
			{
				base.Size = Settings.WindowSize;
				spliter.SplitterDistance = Settings.SplitterPosition;
			}
		}

		private void ValueControlBuilder_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (Settings == null)
			{
				Settings = new SavedSettings();
			}
			Settings.WindowSize = base.Size;
			Settings.SplitterPosition = spliter.SplitterDistance;
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
			this.txtName = new System.Windows.Forms.TextBox();
			this.lblName = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.txtSetAction = new CodeEditBox();
			this.lblSetAction = new System.Windows.Forms.Label();
			this.txtGetAction = new CodeEditBox();
			this.lblGetAction = new System.Windows.Forms.Label();
			this.txtDefaultValue = new System.Windows.Forms.TextBox();
			this.lblDefaultValue = new System.Windows.Forms.Label();
			this.spliter = new System.Windows.Forms.SplitContainer();
			this.spliter.Panel1.SuspendLayout();
			this.spliter.Panel2.SuspendLayout();
			this.spliter.SuspendLayout();
			base.SuspendLayout();
			this.txtName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtName.BackColor = System.Drawing.Color.Black;
			this.txtName.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.txtName.ForeColor = System.Drawing.Color.White;
			this.txtName.Location = new System.Drawing.Point(50, 12);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(522, 22);
			this.txtName.TabIndex = 1;
			this.txtName.WordWrap = false;
			this.lblName.AutoSize = true;
			this.lblName.BackColor = System.Drawing.Color.Transparent;
			this.lblName.ForeColor = System.Drawing.Color.White;
			this.lblName.Location = new System.Drawing.Point(6, 14);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(38, 13);
			this.lblName.TabIndex = 3;
			this.lblName.Text = "Name:";
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnCancel.Location = new System.Drawing.Point(516, 376);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(60, 26);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = false;
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnOK.BackColor = System.Drawing.SystemColors.Control;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnOK.Location = new System.Drawing.Point(450, 376);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(60, 26);
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = false;
			this.txtSetAction.AcceptsTab = true;
			this.txtSetAction.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtSetAction.BackColor = System.Drawing.Color.Black;
			this.txtSetAction.ForeColor = System.Drawing.Color.White;
			this.txtSetAction.Location = new System.Drawing.Point(35, 0);
			this.txtSetAction.Name = "txtSetAction";
			this.txtSetAction.Size = new System.Drawing.Size(526, 146);
			this.txtSetAction.TabIndex = 3;
			this.txtSetAction.Text = "function(value)\n\nend";
			this.txtSetAction.WordWrap = false;
			this.lblSetAction.AutoSize = true;
			this.lblSetAction.BackColor = System.Drawing.Color.Transparent;
			this.lblSetAction.ForeColor = System.Drawing.Color.White;
			this.lblSetAction.Location = new System.Drawing.Point(3, 0);
			this.lblSetAction.Name = "lblSetAction";
			this.lblSetAction.Size = new System.Drawing.Size(26, 13);
			this.lblSetAction.TabIndex = 9;
			this.lblSetAction.Text = "Set:";
			this.txtGetAction.AcceptsTab = true;
			this.txtGetAction.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtGetAction.BackColor = System.Drawing.Color.Black;
			this.txtGetAction.ForeColor = System.Drawing.Color.White;
			this.txtGetAction.Location = new System.Drawing.Point(35, 3);
			this.txtGetAction.Name = "txtGetAction";
			this.txtGetAction.Size = new System.Drawing.Size(526, 143);
			this.txtGetAction.TabIndex = 4;
			this.txtGetAction.Text = "function()\n\treturn 0;\nend";
			this.txtGetAction.WordWrap = false;
			this.lblGetAction.AutoSize = true;
			this.lblGetAction.BackColor = System.Drawing.Color.Transparent;
			this.lblGetAction.ForeColor = System.Drawing.Color.White;
			this.lblGetAction.Location = new System.Drawing.Point(3, 3);
			this.lblGetAction.Name = "lblGetAction";
			this.lblGetAction.Size = new System.Drawing.Size(27, 13);
			this.lblGetAction.TabIndex = 11;
			this.lblGetAction.Text = "Get:";
			this.txtDefaultValue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtDefaultValue.BackColor = System.Drawing.Color.Black;
			this.txtDefaultValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
			this.txtDefaultValue.ForeColor = System.Drawing.Color.White;
			this.txtDefaultValue.Location = new System.Drawing.Point(50, 40);
			this.txtDefaultValue.Name = "txtDefaultValue";
			this.txtDefaultValue.Size = new System.Drawing.Size(522, 20);
			this.txtDefaultValue.TabIndex = 2;
			this.txtDefaultValue.WordWrap = false;
			this.lblDefaultValue.AutoSize = true;
			this.lblDefaultValue.BackColor = System.Drawing.Color.Transparent;
			this.lblDefaultValue.ForeColor = System.Drawing.Color.White;
			this.lblDefaultValue.Location = new System.Drawing.Point(6, 42);
			this.lblDefaultValue.Name = "lblDefaultValue";
			this.lblDefaultValue.Size = new System.Drawing.Size(44, 13);
			this.lblDefaultValue.TabIndex = 13;
			this.lblDefaultValue.Text = "Default:";
			this.spliter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.spliter.Location = new System.Drawing.Point(12, 66);
			this.spliter.Name = "spliter";
			this.spliter.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.spliter.Panel1.Controls.Add(this.txtSetAction);
			this.spliter.Panel1.Controls.Add(this.lblSetAction);
			this.spliter.Panel2.Controls.Add(this.txtGetAction);
			this.spliter.Panel2.Controls.Add(this.lblGetAction);
			this.spliter.Size = new System.Drawing.Size(564, 302);
			this.spliter.SplitterDistance = 149;
			this.spliter.TabIndex = 14;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new System.Drawing.Size(584, 414);
			base.Controls.Add(this.spliter);
			base.Controls.Add(this.txtDefaultValue);
			base.Controls.Add(this.lblDefaultValue);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOK);
			base.Controls.Add(this.txtName);
			base.Controls.Add(this.lblName);
			this.ForeColor = System.Drawing.Color.White;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ValueControlBuilder";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Value Control Builder";
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ValueControlBuilder_FormClosing);
			this.spliter.Panel1.ResumeLayout(false);
			this.spliter.Panel1.PerformLayout();
			this.spliter.Panel2.ResumeLayout(false);
			this.spliter.Panel2.PerformLayout();
			this.spliter.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
