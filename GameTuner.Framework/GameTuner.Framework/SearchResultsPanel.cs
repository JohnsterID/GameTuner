using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.Properties;
using GameTuner.Framework.Scrollables;

namespace GameTuner.Framework
{
	public class SearchResultsPanel : UserControl
	{
		private IContainer components;

		private ScrollableList scrollableList;

		private ToolStrip toolStrip;

		private ToolStripButton buttonClearResults;

		public SearchResultsPanel()
		{
			InitializeComponent();
			if (Context.Has<SearchProvider>())
			{
				SearchProvider searchProvider = Context.Get<SearchProvider>();
				searchProvider.SearchPerformed += provider_SearchPerformed;
				searchProvider.ResultsCleared += provider_ResultsCleared;
			}
		}

		private void provider_ResultsCleared(object sender, EventArgs e)
		{
			scrollableList.Items.Clear();
		}

		private void provider_SearchPerformed(object sender, EventArgs e)
		{
			string text = null;
			scrollableList.Items.Clear();
			SearchProvider searchProvider = Context.Get<SearchProvider>();
			foreach (SearchProvider.ResultInfo result in searchProvider.Results)
			{
				ScrollableItemText scrollableItemText = new ScrollableItemText(result.Location, result.Brief, Font);
				scrollableItemText.Tag = result;
				scrollableItemText.ShowCaption = text == null || string.Compare(result.Location, text) != 0;
				text = result.Location;
				scrollableList.Items.Add(scrollableItemText);
			}
			scrollableList.Refresh();
		}

		private void buttonClearResults_Click(object sender, EventArgs e)
		{
			Context.Get<SearchProvider>().Results.Clear();
		}

		private void scrollableList_DoubleClick(object sender, EventArgs e)
		{
			SearchProvider.ResultInfo resultInfo = scrollableList.SelectedItemTag as SearchProvider.ResultInfo;
			if (resultInfo != null)
			{
				resultInfo.Searcher.Inspect(resultInfo.Context);
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
			this.buttonClearResults = new System.Windows.Forms.ToolStripButton();
			this.scrollableList = new GameTuner.Framework.ScrollableList();
			this.toolStrip.SuspendLayout();
			base.SuspendLayout();
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.buttonClearResults });
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip.Size = new System.Drawing.Size(630, 25);
			this.toolStrip.TabIndex = 1;
			this.toolStrip.Text = "toolStrip";
			this.buttonClearResults.Image = GameTuner.Framework.Properties.Resources.clear_results;
			this.buttonClearResults.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonClearResults.Name = "buttonClearResults";
			this.buttonClearResults.Size = new System.Drawing.Size(94, 22);
			this.buttonClearResults.Text = "Clear Results";
			this.buttonClearResults.Click += new System.EventHandler(buttonClearResults_Click);
			this.scrollableList.BackColor = System.Drawing.Color.White;
			this.scrollableList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scrollableList.Location = new System.Drawing.Point(0, 25);
			this.scrollableList.Name = "scrollableList";
			this.scrollableList.SelectedItem = null;
			this.scrollableList.Size = new System.Drawing.Size(630, 247);
			this.scrollableList.TabIndex = 0;
			this.scrollableList.DoubleClick += new System.EventHandler(scrollableList_DoubleClick);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			base.Controls.Add(this.scrollableList);
			base.Controls.Add(this.toolStrip);
			base.Name = "SearchResultsPanel";
			base.Size = new System.Drawing.Size(630, 272);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
