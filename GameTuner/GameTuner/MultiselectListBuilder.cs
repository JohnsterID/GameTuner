using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner
{
	public class MultiselectListBuilder : Form
	{
		private IContainer components;

		private Button btnCancel;

		private Button btnOK;

		private Label lblName;

		private TextBox txtName;

		private CodeEditBox txtPopulateList;

		private Label lblPopulateList;

		private CodeEditBox txtOnSelected;

		private Label lblOnSelection;

		private CodeEditBox txtOnDeselected;

		private Label lblOnDeslected;

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

		public string PopulateList
		{
			get
			{
				return txtPopulateList.Text;
			}
			set
			{
				txtPopulateList.Text = value;
			}
		}

		public string OnSelected
		{
			get
			{
				return txtOnSelected.Text;
			}
			set
			{
				txtOnSelected.Text = value;
			}
		}

		public string OnDeslected
		{
			get
			{
				return txtOnDeselected.Text;
			}
			set
			{
				txtOnDeselected.Text = value;
			}
		}

		public MultiselectListBuilder()
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
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.lblName = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.lblPopulateList = new System.Windows.Forms.Label();
			this.lblOnSelection = new System.Windows.Forms.Label();
			this.txtOnSelected = new CodeEditBox();
			this.txtPopulateList = new CodeEditBox();
			this.txtOnDeselected = new CodeEditBox();
			this.lblOnDeslected = new System.Windows.Forms.Label();
			base.SuspendLayout();
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnCancel.Location = new System.Drawing.Point(473, 502);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(60, 26);
			this.btnCancel.TabIndex = 10;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = false;
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnOK.BackColor = System.Drawing.SystemColors.Control;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnOK.Location = new System.Drawing.Point(407, 502);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(60, 26);
			this.btnOK.TabIndex = 9;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = false;
			this.lblName.AutoSize = true;
			this.lblName.BackColor = System.Drawing.Color.Transparent;
			this.lblName.ForeColor = System.Drawing.Color.White;
			this.lblName.Location = new System.Drawing.Point(45, 14);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(38, 13);
			this.lblName.TabIndex = 8;
			this.lblName.Text = "Name:";
			this.txtName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtName.BackColor = System.Drawing.Color.Black;
			this.txtName.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.txtName.ForeColor = System.Drawing.Color.White;
			this.txtName.Location = new System.Drawing.Point(89, 9);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(444, 22);
			this.txtName.TabIndex = 7;
			this.txtName.WordWrap = false;
			this.lblPopulateList.AutoSize = true;
			this.lblPopulateList.BackColor = System.Drawing.Color.Transparent;
			this.lblPopulateList.ForeColor = System.Drawing.Color.White;
			this.lblPopulateList.Location = new System.Drawing.Point(12, 37);
			this.lblPopulateList.Name = "lblPopulateList";
			this.lblPopulateList.Size = new System.Drawing.Size(71, 13);
			this.lblPopulateList.TabIndex = 12;
			this.lblPopulateList.Text = "Populate List:";
			this.lblOnSelection.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this.lblOnSelection.AutoSize = true;
			this.lblOnSelection.BackColor = System.Drawing.Color.Transparent;
			this.lblOnSelection.ForeColor = System.Drawing.Color.White;
			this.lblOnSelection.Location = new System.Drawing.Point(12, 231);
			this.lblOnSelection.Name = "lblOnSelection";
			this.lblOnSelection.Size = new System.Drawing.Size(69, 13);
			this.lblOnSelection.TabIndex = 14;
			this.lblOnSelection.Text = "On Selected:";
			this.txtOnSelected.AcceptsTab = true;
			this.txtOnSelected.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtOnSelected.BackColor = System.Drawing.Color.Black;
			this.txtOnSelected.Font = new System.Drawing.Font("Courier New", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtOnSelected.ForeColor = System.Drawing.Color.White;
			this.txtOnSelected.Location = new System.Drawing.Point(89, 228);
			this.txtOnSelected.Name = "txtOnSelected";
			this.txtOnSelected.Size = new System.Drawing.Size(444, 131);
			this.txtOnSelected.TabIndex = 13;
			this.txtOnSelected.Text = "function(selection)\n\nend\n";
			this.txtOnSelected.WordWrap = false;
			this.txtPopulateList.AcceptsTab = true;
			this.txtPopulateList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtPopulateList.BackColor = System.Drawing.Color.Black;
			this.txtPopulateList.Font = new System.Drawing.Font("Courier New", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtPopulateList.ForeColor = System.Drawing.Color.White;
			this.txtPopulateList.Location = new System.Drawing.Point(89, 37);
			this.txtPopulateList.Name = "txtPopulateList";
			this.txtPopulateList.Size = new System.Drawing.Size(444, 185);
			this.txtPopulateList.TabIndex = 11;
			this.txtPopulateList.Text = "function()\n   local listItems = {};\n\n   return listItems;\nend\n";
			this.txtPopulateList.WordWrap = false;
			this.txtOnDeselected.AcceptsTab = true;
			this.txtOnDeselected.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtOnDeselected.BackColor = System.Drawing.Color.Black;
			this.txtOnDeselected.Font = new System.Drawing.Font("Courier New", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtOnDeselected.ForeColor = System.Drawing.Color.White;
			this.txtOnDeselected.Location = new System.Drawing.Point(89, 365);
			this.txtOnDeselected.Name = "txtOnDeselected";
			this.txtOnDeselected.Size = new System.Drawing.Size(444, 131);
			this.txtOnDeselected.TabIndex = 15;
			this.txtOnDeselected.Text = "function(selection)\n\nend\n";
			this.txtOnDeselected.WordWrap = false;
			this.lblOnDeslected.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this.lblOnDeslected.AutoSize = true;
			this.lblOnDeslected.BackColor = System.Drawing.Color.Transparent;
			this.lblOnDeslected.ForeColor = System.Drawing.Color.White;
			this.lblOnDeslected.Location = new System.Drawing.Point(2, 368);
			this.lblOnDeslected.Name = "lblOnDeslected";
			this.lblOnDeslected.Size = new System.Drawing.Size(81, 13);
			this.lblOnDeslected.TabIndex = 16;
			this.lblOnDeslected.Text = "On Deselected:";
			base.AcceptButton = this.btnOK;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new System.Drawing.Size(545, 535);
			base.Controls.Add(this.txtOnDeselected);
			base.Controls.Add(this.lblOnDeslected);
			base.Controls.Add(this.txtOnSelected);
			base.Controls.Add(this.lblOnSelection);
			base.Controls.Add(this.txtPopulateList);
			base.Controls.Add(this.lblPopulateList);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOK);
			base.Controls.Add(this.txtName);
			base.Controls.Add(this.lblName);
			this.ForeColor = System.Drawing.Color.White;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "MultiselectListBuilder";
			this.Text = "Multiselect List Builder";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
