using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GameTuner.Framework.Controls
{
	public class OpenVirtualFileForm : Form
	{
		public enum SearchType
		{
			[Description("Regular Expression")]
			RegularExpression,
			[Description("Substring Match")]
			SubstringMatch
		}

		private class Filter
		{
			public string Description { get; set; }

			public string FilterStr { get; set; }

			public Filter(string _Desc, string _Filter)
			{
				Description = _Desc;
				FilterStr = _Filter;
			}

			public override string ToString()
			{
				return Description;
			}
		}

		private class SearchQuery
		{
			public string szString;

			public bool CaseSensitive;

			public SearchType SearchType;
		}

		private List<Filter> m_kCurrentFilters = new List<Filter>();

		private int m_iFilterIndex;

		private IContainer components;

		private BackgroundWorker FileFilterer;

		private TableLayoutPanel tableLayoutPanel1;

		private ListView VirtualItemList;

		private ColumnHeader Filename;

		private ColumnHeader Folder;

		private TableLayoutPanel tableLayoutPanel2;

		private CheckBox CaseSensitiveCheck;

		private ComboBox FilterCombo;

		private ComboBox SearchTypeCombo;

		private MaskedTextBox FilenameSearch;

		private Label label1;

		private Button Cancel;

		private Button Open;

		[Browsable(false)]
		public ICollection<IVirtualItem> SelectedItems { get; private set; }

		[Category("Behavior")]
		[DefaultValue(SearchType.SubstringMatch)]
		public SearchType MatchType
		{
			get
			{
				if (SearchTypeCombo != null && SearchTypeCombo.SelectedValue is SearchType)
				{
					return (SearchType)SearchTypeCombo.SelectedValue;
				}
				return SearchType.SubstringMatch;
			}
			set
			{
				SearchTypeCombo.SelectedValue = value;
			}
		}

		[Category("Behavior")]
		[DefaultValue(false)]
		public bool CaseSensitive
		{
			get
			{
				return CaseSensitiveCheck.Checked;
			}
			set
			{
				CaseSensitiveCheck.Checked = value;
			}
		}

		[DefaultValue(false)]
		[Category("Behavior")]
		public bool AllowMultiSelect
		{
			get
			{
				return VirtualItemList.MultiSelect;
			}
			set
			{
				VirtualItemList.MultiSelect = value;
			}
		}

		[Category("Behavior")]
		[DefaultValue("All files|*.*")]
		public string FileFilters
		{
			get
			{
				string text = "";
				foreach (Filter kCurrentFilter in m_kCurrentFilters)
				{
					Filter filter = kCurrentFilter as Filter;
					text += string.Format("{0}|{1}|", filter.Description, filter.FilterStr);
				}
				if (text.Length == 0)
				{
					return "";
				}
				return text.Remove(text.Length - 1);
			}
			set
			{
				m_kCurrentFilters.Clear();
				IList<string> list = Tokenizer.Tokenize(value, "|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				bool flag = false;
				string desc = null;
				foreach (string item in list)
				{
					if (!flag)
					{
						desc = item;
					}
					else
					{
						m_kCurrentFilters.Add(new Filter(desc, item));
					}
					flag = !flag;
				}
				if (!Context.InDesignMode)
				{
					FilterCombo.Items.Clear();
					FilterCombo.Items.AddRange(m_kCurrentFilters.ToArray());
				}
			}
		}

		[DefaultValue(0)]
		[Category("Behavior")]
		public int FilterIndex
		{
			get
			{
				return m_iFilterIndex;
			}
			set
			{
				if (value < m_kCurrentFilters.Count)
				{
					m_iFilterIndex = value;
					if (!Context.InDesignMode)
					{
						FilterCombo.SelectedIndex = value;
					}
				}
			}
		}

		private SearchQuery QueuedQuery { get; set; }

		private IVirtualFile[] Results { get; set; }

		public OpenVirtualFileForm()
		{
			InitializeComponent();
			SearchTypeCombo.DataSource = Enum.GetValues(typeof(SearchType));
			SelectedItems = new List<IVirtualItem>();
			VirtualItemList.View = View.Details;
			Open.Enabled = false;
			AllowMultiSelect = false;
			FileFilters = "All files|*.*";
			FilterIndex = 0;
			CaseSensitive = false;
		}

		private void FilenameSearch_TextChanged(object sender, EventArgs e)
		{
			MakeQuery();
		}

		private void FilterCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			FilterIndex = FilterCombo.SelectedIndex;
			MakeQuery();
		}

		private void MakeQuery()
		{
			QueuedQuery = new SearchQuery();
			QueuedQuery.CaseSensitive = CaseSensitive;
			QueuedQuery.szString = FilenameSearch.Text;
			QueuedQuery.SearchType = MatchType;
			if (FileFilterer.IsBusy)
			{
				FileFilterer.CancelAsync();
			}
			else
			{
				FileFilterer.RunWorkerAsync(QueuedQuery);
			}
		}

		private void FileFilterer_DoWork(object sender, DoWorkEventArgs e)
		{
			e.Result = null;
			QueuedQuery = null;
			BackgroundWorker kWorker = sender as BackgroundWorker;
			IVirtualSpace virtualSpace = Context.Get<IVirtualSpace>();
			ICollection<IVirtualItem> items = virtualSpace.Items;
			IEnumerable<IVirtualFile> source = null;
			SearchQuery searchQuery = e.Argument as SearchQuery;
			if (searchQuery.SearchType == SearchType.RegularExpression)
			{
				Regex kRE = new Regex(searchQuery.szString, (!searchQuery.CaseSensitive) ? RegexOptions.IgnoreCase : RegexOptions.None);
				source = from kItem in items
					where kItem is IVirtualFile && kRE.IsMatch(((IVirtualFile)kItem).Name) && !kWorker.CancellationPending
					select (IVirtualFile)kItem;
			}
			else if (searchQuery.SearchType == SearchType.SubstringMatch)
			{
				string szSearch = searchQuery.szString;
				if (searchQuery.CaseSensitive)
				{
					source = from kItem in items
						where kItem is IVirtualFile && ((IVirtualFile)kItem).Name.Contains(szSearch) && !kWorker.CancellationPending
						select (IVirtualFile)kItem;
				}
				else
				{
					szSearch = szSearch.ToLower();
					source = from kItem in items
						where kItem is IVirtualFile && ((IVirtualFile)kItem).Name.ToLower().Contains(szSearch) && !kWorker.CancellationPending
						select (IVirtualFile)kItem;
				}
			}
			if (FilterCombo.Items.Count > FilterIndex && FilterIndex >= 0)
			{
				string pattern = "^" + Regex.Escape((FilterCombo.Items[FilterIndex] as Filter).FilterStr).Replace("\\*", ".*").Replace("\\?", ".") + "$";
				Regex kRE2 = new Regex(pattern, RegexOptions.IgnoreCase);
				source = from kItem in source
					where kItem != null && kRE2.IsMatch(kItem.Name) && !kWorker.CancellationPending
					select (kItem);
			}
			Results = source.Select((IVirtualFile kItem) => kItem).ToArray();
		}

		private void FileFilterer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (QueuedQuery != null)
			{
				FileFilterer.RunWorkerAsync(QueuedQuery);
			}
			else if (!e.Cancelled)
			{
				VirtualItemList.VirtualListSize = Results.Count();
				VirtualItemList.Invalidate();
			}
		}

		private void VirtualItemList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			if (Results == null || Results.Count() <= e.ItemIndex)
			{
				e.Item = new ListViewItem();
				return;
			}
			IVirtualFile virtualFile = Results[e.ItemIndex];
			e.Item = new ListViewItem(Path.GetFileName(virtualFile.FullPath));
			e.Item.SubItems.Add(Path.GetDirectoryName(virtualFile.FullPath));
			e.Item.Tag = virtualFile;
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			Close();
		}

		private void Open_Click(object sender, EventArgs e)
		{
			SelectedItems.Clear();
			foreach (int selectedIndex in VirtualItemList.SelectedIndices)
			{
				SelectedItems.Add(Results[selectedIndex] as IVirtualItem);
			}
			base.DialogResult = DialogResult.OK;
			Close();
		}

		private void VirtualItemList_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListView listView = sender as ListView;
			Open.Enabled = listView.SelectedIndices.Count > 0;
		}

		private void VirtualItemList_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ListView listView = sender as ListView;
			ListViewHitTestInfo listViewHitTestInfo = listView.HitTest(e.Location);
			if (listViewHitTestInfo.Item != null)
			{
				SelectedItems.Clear();
				SelectedItems.Add(listViewHitTestInfo.Item.Tag as IVirtualItem);
				base.DialogResult = DialogResult.OK;
				Close();
			}
		}

		private void OpenVirtualFileForm_Shown(object sender, EventArgs e)
		{
			FilenameSearch.Focus();
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
			this.FileFilterer = new System.ComponentModel.BackgroundWorker();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.VirtualItemList = new System.Windows.Forms.ListView();
			this.Filename = new System.Windows.Forms.ColumnHeader();
			this.Folder = new System.Windows.Forms.ColumnHeader();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.CaseSensitiveCheck = new System.Windows.Forms.CheckBox();
			this.FilterCombo = new System.Windows.Forms.ComboBox();
			this.Open = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.FilenameSearch = new System.Windows.Forms.MaskedTextBox();
			this.SearchTypeCombo = new System.Windows.Forms.ComboBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			base.SuspendLayout();
			this.FileFilterer.WorkerSupportsCancellation = true;
			this.FileFilterer.DoWork += new System.ComponentModel.DoWorkEventHandler(FileFilterer_DoWork);
			this.FileFilterer.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(FileFilterer_RunWorkerCompleted);
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
			this.tableLayoutPanel1.Controls.Add(this.VirtualItemList, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70f));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20f));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(734, 481);
			this.tableLayoutPanel1.TabIndex = 4;
			this.VirtualItemList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.Filename, this.Folder });
			this.tableLayoutPanel1.SetColumnSpan(this.VirtualItemList, 10);
			this.VirtualItemList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.VirtualItemList.FullRowSelect = true;
			this.VirtualItemList.GridLines = true;
			this.VirtualItemList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.VirtualItemList.HideSelection = false;
			this.VirtualItemList.Location = new System.Drawing.Point(3, 3);
			this.VirtualItemList.Name = "VirtualItemList";
			this.VirtualItemList.Size = new System.Drawing.Size(728, 405);
			this.VirtualItemList.TabIndex = 2;
			this.VirtualItemList.UseCompatibleStateImageBehavior = false;
			this.VirtualItemList.View = System.Windows.Forms.View.Details;
			this.VirtualItemList.VirtualMode = true;
			this.VirtualItemList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(VirtualItemList_MouseDoubleClick);
			this.VirtualItemList.SelectedIndexChanged += new System.EventHandler(VirtualItemList_SelectedIndexChanged);
			this.VirtualItemList.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(VirtualItemList_RetrieveVirtualItem);
			this.Filename.Text = "Filename";
			this.Filename.Width = 249;
			this.Folder.Text = "Folder";
			this.Folder.Width = 474;
			this.tableLayoutPanel2.ColumnCount = 5;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.82609f));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.17391f));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170f));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 99f));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90f));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20f));
			this.tableLayoutPanel2.Controls.Add(this.SearchTypeCombo, 2, 0);
			this.tableLayoutPanel2.Controls.Add(this.FilenameSearch, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.FilterCombo, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.Cancel, 4, 0);
			this.tableLayoutPanel2.Controls.Add(this.Open, 3, 0);
			this.tableLayoutPanel2.Controls.Add(this.CaseSensitiveCheck, 2, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 414);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32f));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 99f));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(728, 64);
			this.tableLayoutPanel2.TabIndex = 3;
			this.CaseSensitiveCheck.AutoSize = true;
			this.CaseSensitiveCheck.Location = new System.Drawing.Point(371, 35);
			this.CaseSensitiveCheck.Name = "CaseSensitiveCheck";
			this.CaseSensitiveCheck.Size = new System.Drawing.Size(96, 17);
			this.CaseSensitiveCheck.TabIndex = 7;
			this.CaseSensitiveCheck.Text = "Case Sensitive";
			this.CaseSensitiveCheck.UseVisualStyleBackColor = true;
			this.CaseSensitiveCheck.CheckedChanged += new System.EventHandler(FilenameSearch_TextChanged);
			this.FilterCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.FilterCombo.FormattingEnabled = true;
			this.FilterCombo.Location = new System.Drawing.Point(87, 35);
			this.FilterCombo.Name = "FilterCombo";
			this.FilterCombo.Size = new System.Drawing.Size(278, 21);
			this.FilterCombo.TabIndex = 11;
			this.FilterCombo.SelectedIndexChanged += new System.EventHandler(FilterCombo_SelectedIndexChanged);
			this.Open.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Open.Location = new System.Drawing.Point(541, 3);
			this.Open.Name = "Open";
			this.Open.Size = new System.Drawing.Size(93, 26);
			this.Open.TabIndex = 5;
			this.Open.Text = "Open";
			this.Open.UseVisualStyleBackColor = true;
			this.Open.Click += new System.EventHandler(Open_Click);
			this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Cancel.Location = new System.Drawing.Point(640, 3);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(85, 26);
			this.Cancel.TabIndex = 6;
			this.Cancel.Text = "Cancel";
			this.Cancel.UseVisualStyleBackColor = true;
			this.Cancel.Click += new System.EventHandler(Cancel_Click);
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(52, 26);
			this.label1.TabIndex = 10;
			this.label1.Text = "Filename Search";
			this.FilenameSearch.Location = new System.Drawing.Point(87, 3);
			this.FilenameSearch.Name = "FilenameSearch";
			this.FilenameSearch.Size = new System.Drawing.Size(278, 20);
			this.FilenameSearch.TabIndex = 12;
			this.FilenameSearch.TextChanged += new System.EventHandler(FilenameSearch_TextChanged);
			this.SearchTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.SearchTypeCombo.FormattingEnabled = true;
			this.SearchTypeCombo.Location = new System.Drawing.Point(371, 3);
			this.SearchTypeCombo.Name = "SearchTypeCombo";
			this.SearchTypeCombo.Size = new System.Drawing.Size(153, 21);
			this.SearchTypeCombo.TabIndex = 13;
			this.SearchTypeCombo.SelectedIndexChanged += new System.EventHandler(FilenameSearch_TextChanged);
			base.AcceptButton = this.Open;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.Cancel;
			base.ClientSize = new System.Drawing.Size(734, 481);
			base.Controls.Add(this.tableLayoutPanel1);
			base.Location = new System.Drawing.Point(100, 200);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(324, 199);
			base.Name = "OpenVirtualFileForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Open File From Virtual Space";
			base.Shown += new System.EventHandler(OpenVirtualFileForm_Shown);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
