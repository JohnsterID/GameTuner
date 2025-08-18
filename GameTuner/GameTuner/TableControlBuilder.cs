using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner
{
	public class TableControlBuilder : Form
	{
		private IContainer components;

		private TextBox txtTable;

		private Label lblTable;

		private Button btnCancel;

		private Button btnOK;

		private CheckBox chkFixed;

		private CodeEditBox txtRefresh;

		private Label lblRefresh;

		public string Table
		{
			get
			{
				return txtTable.Text;
			}
			set
			{
				txtTable.Text = value;
			}
		}

		public bool Fixed
		{
			get
			{
				return chkFixed.Checked;
			}
			set
			{
				chkFixed.Checked = value;
			}
		}

		public string OnRefresh
		{
			get
			{
				return txtRefresh.Text;
			}
			set
			{
				txtRefresh.Text = value;
			}
		}

		public TableControlBuilder()
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
			this.txtTable = new System.Windows.Forms.TextBox();
			this.lblTable = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.chkFixed = new System.Windows.Forms.CheckBox();
			this.txtRefresh = new CodeEditBox();
			this.lblRefresh = new System.Windows.Forms.Label();
			base.SuspendLayout();
			this.txtTable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtTable.BackColor = System.Drawing.Color.Black;
			this.txtTable.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.txtTable.ForeColor = System.Drawing.Color.White;
			this.txtTable.Location = new System.Drawing.Point(50, 12);
			this.txtTable.Name = "txtTable";
			this.txtTable.Size = new System.Drawing.Size(522, 22);
			this.txtTable.TabIndex = 1;
			this.txtTable.WordWrap = false;
			this.lblTable.AutoSize = true;
			this.lblTable.BackColor = System.Drawing.Color.Transparent;
			this.lblTable.ForeColor = System.Drawing.Color.White;
			this.lblTable.Location = new System.Drawing.Point(6, 14);
			this.lblTable.Name = "lblTable";
			this.lblTable.Size = new System.Drawing.Size(37, 13);
			this.lblTable.TabIndex = 3;
			this.lblTable.Text = "Table:";
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnCancel.Location = new System.Drawing.Point(516, 213);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(60, 26);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = false;
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnOK.BackColor = System.Drawing.SystemColors.Control;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnOK.Location = new System.Drawing.Point(450, 213);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(60, 26);
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = false;
			this.chkFixed.AutoSize = true;
			this.chkFixed.Location = new System.Drawing.Point(50, 40);
			this.chkFixed.Name = "chkFixed";
			this.chkFixed.Size = new System.Drawing.Size(51, 17);
			this.chkFixed.TabIndex = 16;
			this.chkFixed.Text = "Fixed";
			this.chkFixed.UseVisualStyleBackColor = true;
			this.txtRefresh.AcceptsTab = true;
			this.txtRefresh.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtRefresh.BackColor = System.Drawing.Color.Black;
			this.txtRefresh.Font = new System.Drawing.Font("Courier New", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtRefresh.ForeColor = System.Drawing.Color.White;
			this.txtRefresh.Location = new System.Drawing.Point(50, 61);
			this.txtRefresh.Name = "txtRefresh";
			this.txtRefresh.Size = new System.Drawing.Size(522, 146);
			this.txtRefresh.TabIndex = 17;
			this.txtRefresh.Text = "";
			this.txtRefresh.WordWrap = false;
			this.lblRefresh.AutoSize = true;
			this.lblRefresh.BackColor = System.Drawing.Color.Transparent;
			this.lblRefresh.ForeColor = System.Drawing.Color.White;
			this.lblRefresh.Location = new System.Drawing.Point(6, 61);
			this.lblRefresh.Name = "lblRefresh";
			this.lblRefresh.Size = new System.Drawing.Size(47, 13);
			this.lblRefresh.TabIndex = 18;
			this.lblRefresh.Text = "Refresh:";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new System.Drawing.Size(584, 251);
			base.Controls.Add(this.txtRefresh);
			base.Controls.Add(this.lblRefresh);
			base.Controls.Add(this.chkFixed);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOK);
			base.Controls.Add(this.txtTable);
			base.Controls.Add(this.lblTable);
			this.ForeColor = System.Drawing.Color.White;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "TableControlBuilder";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Table Control Builder";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
