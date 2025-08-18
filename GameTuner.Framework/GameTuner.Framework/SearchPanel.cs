using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public class SearchPanel : UserControl
	{
		private IContainer components;

		private Label label1;

		private TextBox textBox;

		private Label label2;

		private ComboBox comboBoxLook;

		private GroupBox groupBox1;

		private ComboBox comboBoxUse;

		private CheckBox checkUse;

		private CheckBox checkMatchWholeWord;

		private CheckBox checkMatchCase;

		private Button buttonFindNext;

		public SearchPanel()
		{
			InitializeComponent();
			if (!Context.Has<SearchProvider>())
			{
				return;
			}
			SearchProvider searchProvider = Context.Get<SearchProvider>();
			comboBoxLook.Items.Add("All Items");
			foreach (ISearcher searcher in searchProvider.Searchers)
			{
				comboBoxLook.Items.Add(searcher);
			}
			comboBoxLook.SelectedIndex = 0;
			foreach (IMatcher matcher in searchProvider.Matchers)
			{
				comboBoxUse.Items.Add(matcher);
			}
			comboBoxUse.SelectedIndex = 0;
		}

		private void checkUse_CheckedChanged(object sender, EventArgs e)
		{
			comboBoxUse.Enabled = checkUse.Checked;
		}

		private SearchSettings MakeSettings()
		{
			SearchSettings searchSettings = new SearchSettings(textBox.Text);
			searchSettings.Matcher = comboBoxUse.SelectedItem as IMatcher;
			searchSettings.UseMatcher = checkUse.Checked;
			searchSettings.MatchCase = checkMatchCase.Checked;
			searchSettings.MatchWholeWord = checkMatchWholeWord.Checked;
			return searchSettings;
		}

		public void PerformSearch()
		{
			if (textBox.Text.Length > 0)
			{
				SearchProvider searchProvider = Context.Get<SearchProvider>();
				searchProvider.Search(MakeSettings(), comboBoxLook.SelectedItem as ISearcher, SearchMethod.Search);
			}
		}

		private void buttonFindNext_Click(object sender, EventArgs e)
		{
			PerformSearch();
		}

		private void textBox_TextChanged(object sender, EventArgs e)
		{
			buttonFindNext.Enabled = textBox.Text.Length > 0;
		}

		private void textBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				PerformSearch();
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
			this.label1 = new System.Windows.Forms.Label();
			this.textBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxLook = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboBoxUse = new System.Windows.Forms.ComboBox();
			this.checkUse = new System.Windows.Forms.CheckBox();
			this.checkMatchWholeWord = new System.Windows.Forms.CheckBox();
			this.checkMatchCase = new System.Windows.Forms.CheckBox();
			this.buttonFindNext = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Fi&nd what:";
			this.textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.textBox.Location = new System.Drawing.Point(6, 16);
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(301, 20);
			this.textBox.TabIndex = 1;
			this.textBox.TextChanged += new System.EventHandler(textBox_TextChanged);
			this.textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(textBox_KeyDown);
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(45, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "&Look in:";
			this.comboBoxLook.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.comboBoxLook.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxLook.FormattingEnabled = true;
			this.comboBoxLook.Location = new System.Drawing.Point(6, 65);
			this.comboBoxLook.Name = "comboBoxLook";
			this.comboBoxLook.Size = new System.Drawing.Size(301, 21);
			this.comboBoxLook.TabIndex = 3;
			this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.groupBox1.Controls.Add(this.comboBoxUse);
			this.groupBox1.Controls.Add(this.checkUse);
			this.groupBox1.Controls.Add(this.checkMatchWholeWord);
			this.groupBox1.Controls.Add(this.checkMatchCase);
			this.groupBox1.Location = new System.Drawing.Point(6, 92);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(301, 122);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Find &options";
			this.comboBoxUse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.comboBoxUse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxUse.Enabled = false;
			this.comboBoxUse.FormattingEnabled = true;
			this.comboBoxUse.Location = new System.Drawing.Point(23, 89);
			this.comboBoxUse.Name = "comboBoxUse";
			this.comboBoxUse.Size = new System.Drawing.Size(272, 21);
			this.comboBoxUse.TabIndex = 3;
			this.checkUse.AutoSize = true;
			this.checkUse.Location = new System.Drawing.Point(6, 65);
			this.checkUse.Name = "checkUse";
			this.checkUse.Size = new System.Drawing.Size(48, 17);
			this.checkUse.TabIndex = 2;
			this.checkUse.Text = "Us&e:";
			this.checkUse.UseVisualStyleBackColor = true;
			this.checkUse.CheckedChanged += new System.EventHandler(checkUse_CheckedChanged);
			this.checkMatchWholeWord.AutoSize = true;
			this.checkMatchWholeWord.Location = new System.Drawing.Point(6, 42);
			this.checkMatchWholeWord.Name = "checkMatchWholeWord";
			this.checkMatchWholeWord.Size = new System.Drawing.Size(107, 17);
			this.checkMatchWholeWord.TabIndex = 1;
			this.checkMatchWholeWord.Text = "Match &wole word";
			this.checkMatchWholeWord.UseVisualStyleBackColor = true;
			this.checkMatchCase.AutoSize = true;
			this.checkMatchCase.Location = new System.Drawing.Point(6, 19);
			this.checkMatchCase.Name = "checkMatchCase";
			this.checkMatchCase.Size = new System.Drawing.Size(82, 17);
			this.checkMatchCase.TabIndex = 0;
			this.checkMatchCase.Text = "Match &case";
			this.checkMatchCase.UseVisualStyleBackColor = true;
			this.buttonFindNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.buttonFindNext.Enabled = false;
			this.buttonFindNext.Location = new System.Drawing.Point(232, 220);
			this.buttonFindNext.Name = "buttonFindNext";
			this.buttonFindNext.Size = new System.Drawing.Size(75, 23);
			this.buttonFindNext.TabIndex = 13;
			this.buttonFindNext.Text = "Find Next";
			this.buttonFindNext.UseVisualStyleBackColor = true;
			this.buttonFindNext.Click += new System.EventHandler(buttonFindNext_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.buttonFindNext);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.comboBoxLook);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.textBox);
			base.Controls.Add(this.label1);
			base.Name = "SearchPanel";
			base.Size = new System.Drawing.Size(310, 246);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
