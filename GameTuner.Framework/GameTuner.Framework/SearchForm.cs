using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework
{
	public class SearchForm : Form
	{
		private bool allowUserClose;

		private IContainer components;

		private ToolStrip toolStrip;

		private ToolStripButton buttonQuickFind;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripButton buttonQuickReplace;

		private ShowOnlyOnePanel showOnlyOnePanel;

		private SearchPanel searchPanel1;

		private SearchReplacePanel searchReplacePanel1;

		public bool AllowUserClose
		{
			get
			{
				return allowUserClose;
			}
			set
			{
				allowUserClose = value;
			}
		}

		public SearchForm()
		{
			allowUserClose = false;
			InitializeComponent();
			buttonQuickReplace.Enabled = showOnlyOnePanel.HasControl<SearchReplacePanel>();
			buttonQuickFind_Click(this, EventArgs.Empty);
		}

		private void buttonQuickFind_Click(object sender, EventArgs e)
		{
			buttonQuickFind.Checked = true;
			buttonQuickReplace.Checked = false;
			showOnlyOnePanel.ActivateControl<SearchPanel>();
		}

		private void buttonQuickReplace_Click(object sender, EventArgs e)
		{
			buttonQuickFind.Checked = false;
			buttonQuickReplace.Checked = true;
			showOnlyOnePanel.ActivateControl<SearchReplacePanel>();
		}

		public void Search()
		{
			buttonQuickFind_Click(this, EventArgs.Empty);
			SearchPanel searchPanel = showOnlyOnePanel.ActiveControl as SearchPanel;
			if (searchPanel != null)
			{
				searchPanel.PerformSearch();
			}
		}

		private void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!allowUserClose && e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				Hide();
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
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.buttonQuickFind = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonQuickReplace = new System.Windows.Forms.ToolStripButton();
			this.showOnlyOnePanel = new GameTuner.Framework.ShowOnlyOnePanel();
			this.searchReplacePanel1 = new GameTuner.Framework.SearchReplacePanel();
			this.searchPanel1 = new GameTuner.Framework.SearchPanel();
			this.toolStrip.SuspendLayout();
			this.showOnlyOnePanel.SuspendLayout();
			base.SuspendLayout();
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.buttonQuickFind, this.toolStripSeparator1, this.buttonQuickReplace });
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip.Size = new System.Drawing.Size(304, 25);
			this.toolStrip.TabIndex = 1;
			this.toolStrip.Text = "toolStrip1";
			this.buttonQuickFind.Checked = true;
			this.buttonQuickFind.CheckState = System.Windows.Forms.CheckState.Checked;
			this.buttonQuickFind.Image = GameTuner.Framework.Properties.Resources.quick_find;
			this.buttonQuickFind.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonQuickFind.Name = "buttonQuickFind";
			this.buttonQuickFind.Size = new System.Drawing.Size(84, 22);
			this.buttonQuickFind.Text = "Quick Find";
			this.buttonQuickFind.Click += new System.EventHandler(buttonQuickFind_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			this.buttonQuickReplace.Enabled = false;
			this.buttonQuickReplace.Image = GameTuner.Framework.Properties.Resources.quick_replace;
			this.buttonQuickReplace.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonQuickReplace.Name = "buttonQuickReplace";
			this.buttonQuickReplace.Size = new System.Drawing.Size(102, 22);
			this.buttonQuickReplace.Text = "Quick Replace";
			this.buttonQuickReplace.Click += new System.EventHandler(buttonQuickReplace_Click);
			this.showOnlyOnePanel.ActiveControl = null;
			this.showOnlyOnePanel.BackColor = System.Drawing.SystemColors.Control;
			this.showOnlyOnePanel.Controls.Add(this.searchReplacePanel1);
			this.showOnlyOnePanel.Controls.Add(this.searchPanel1);
			this.showOnlyOnePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.showOnlyOnePanel.Location = new System.Drawing.Point(0, 25);
			this.showOnlyOnePanel.Name = "showOnlyOnePanel";
			this.showOnlyOnePanel.Size = new System.Drawing.Size(304, 319);
			this.showOnlyOnePanel.TabIndex = 2;
			this.searchReplacePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.searchReplacePanel1.Location = new System.Drawing.Point(0, 0);
			this.searchReplacePanel1.Name = "searchReplacePanel1";
			this.searchReplacePanel1.Size = new System.Drawing.Size(304, 319);
			this.searchReplacePanel1.TabIndex = 1;
			this.searchPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.searchPanel1.Location = new System.Drawing.Point(0, 0);
			this.searchPanel1.Name = "searchPanel1";
			this.searchPanel1.Size = new System.Drawing.Size(304, 319);
			this.searchPanel1.TabIndex = 0;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
			base.ClientSize = new System.Drawing.Size(304, 344);
			base.Controls.Add(this.showOnlyOnePanel);
			base.Controls.Add(this.toolStrip);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(320, 378);
			base.Name = "SearchForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Find and Replace";
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(SearchForm_FormClosing);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.showOnlyOnePanel.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
