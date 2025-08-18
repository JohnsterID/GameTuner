using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner
{
	public class ProjectPanelsManager : Form
	{
		private IContainer components;

		private ListView lstPanelFiles;

		private Button btnOk;

		private Button btnCancel;

		public List<string> AvailablePanels
		{
			set
			{
				if (value == null)
				{
					return;
				}
				foreach (string item in value)
				{
					lstPanelFiles.Items.Add(item, item, -1);
				}
			}
		}

		public List<string> ProjectPanels
		{
			get
			{
				List<string> list = new List<string>();
				foreach (ListViewItem item in lstPanelFiles.Items)
				{
					if (item.Checked)
					{
						list.Add(item.Text);
					}
				}
				return list;
			}
			set
			{
				if (value == null)
				{
					return;
				}
				foreach (string item in value)
				{
					ListViewItem[] array = lstPanelFiles.Items.Find(item, true);
					ListViewItem[] array2 = array;
					foreach (ListViewItem listViewItem in array2)
					{
						listViewItem.Checked = true;
					}
				}
			}
		}

		public ProjectPanelsManager()
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
			this.lstPanelFiles = new System.Windows.Forms.ListView();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			base.SuspendLayout();
			this.lstPanelFiles.Alignment = System.Windows.Forms.ListViewAlignment.Left;
			this.lstPanelFiles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.lstPanelFiles.BackColor = System.Drawing.Color.Black;
			this.lstPanelFiles.CheckBoxes = true;
			this.lstPanelFiles.ForeColor = System.Drawing.Color.White;
			this.lstPanelFiles.Location = new System.Drawing.Point(12, 12);
			this.lstPanelFiles.Name = "lstPanelFiles";
			this.lstPanelFiles.Size = new System.Drawing.Size(399, 306);
			this.lstPanelFiles.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.lstPanelFiles.TabIndex = 0;
			this.lstPanelFiles.UseCompatibleStateImageBehavior = false;
			this.lstPanelFiles.View = System.Windows.Forms.View.List;
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnOk.BackColor = System.Drawing.SystemColors.Control;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnOk.Location = new System.Drawing.Point(252, 324);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(79, 28);
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "Ok";
			this.btnOk.UseVisualStyleBackColor = false;
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnCancel.Location = new System.Drawing.Point(337, 325);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(73, 27);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = false;
			base.AcceptButton = this.btnOk;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new System.Drawing.Size(423, 358);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOk);
			base.Controls.Add(this.lstPanelFiles);
			this.ForeColor = System.Drawing.Color.White;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ProjectPanelsManager";
			base.ShowIcon = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Project Panels";
			base.ResumeLayout(false);
		}
	}
}
