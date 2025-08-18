using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework.BuildProcess
{
	public class BuildResultsPanel : UserControl
	{
		private IBuildJob job;

		private IContainer components;

		private CaptionControl captionControl;

		private ListView listView;

		private ColumnHeader columnHeader1;

		private ColumnHeader columnHeader2;

		private ImageList imageList;

		public IBuildJob Job
		{
			private get
			{
				return job;
			}
			set
			{
				job = value;
				RebuildList();
				UpdateStatusCaption();
			}
		}

		public BuildResultsPanel()
		{
			InitializeComponent();
		}

		private void UpdateStatusCaption()
		{
			switch (job.BuildResults.HighestResultLevel)
			{
			case ValidationResultLevel.None:
				captionControl.Image = GameTuner.Framework.Properties.Resources.ver_none;
				captionControl.Caption = GameTuner.Framework.Properties.Resources.BuildNone;
				break;
			case ValidationResultLevel.Success:
				captionControl.Image = GameTuner.Framework.Properties.Resources.ver_ok;
				captionControl.Caption = GameTuner.Framework.Properties.Resources.BuildSuccess;
				break;
			case ValidationResultLevel.Warning:
				captionControl.Image = GameTuner.Framework.Properties.Resources.ver_old;
				captionControl.Caption = GameTuner.Framework.Properties.Resources.BuildWarning;
				break;
			case ValidationResultLevel.Error:
				captionControl.Image = GameTuner.Framework.Properties.Resources.ver_new;
				captionControl.Caption = GameTuner.Framework.Properties.Resources.BuildError;
				break;
			}
		}

		private void RebuildList()
		{
			listView.Items.Clear();
			foreach (BuildResultsArgs.ResultInfo result in job.BuildResults.Results)
			{
				ListViewItem listViewItem = listView.Items.Add(result.Level.ToString(), (int)result.Level);
				listViewItem.SubItems.Add(result.Brief);
				listViewItem.Tag = result;
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameTuner.Framework.ValidatePanel));
			this.listView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.captionControl = new GameTuner.Framework.CaptionControl();
			base.SuspendLayout();
			this.listView.AllowColumnReorder = true;
			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.columnHeader1, this.columnHeader2 });
			this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView.FullRowSelect = true;
			this.listView.GridLines = true;
			this.listView.Location = new System.Drawing.Point(0, 0);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(426, 193);
			this.listView.SmallImageList = this.imageList;
			this.listView.TabIndex = 2;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			this.columnHeader1.Text = "Condition";
			this.columnHeader1.Width = 80;
			this.columnHeader2.Text = "Description";
			this.columnHeader2.Width = 240;
			this.imageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList.ImageStream");
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "ver_none.png");
			this.imageList.Images.SetKeyName(1, "ver_ok.png");
			this.imageList.Images.SetKeyName(2, "ver_old.png");
			this.imageList.Images.SetKeyName(3, "ver_new.png");
			this.captionControl.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
			this.captionControl.Caption = "Results";
			this.captionControl.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.captionControl.ForeColor = System.Drawing.Color.White;
			this.captionControl.Image = GameTuner.Framework.Properties.Resources.ver_none;
			this.captionControl.Location = new System.Drawing.Point(0, 193);
			this.captionControl.Name = "captionControl";
			this.captionControl.Size = new System.Drawing.Size(426, 22);
			this.captionControl.TabIndex = 1;
			this.captionControl.Transparent = System.Drawing.Color.Magenta;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.listView);
			base.Controls.Add(this.captionControl);
			base.Name = "ValidatePanel";
			base.Size = new System.Drawing.Size(426, 215);
			base.ResumeLayout(false);
		}
	}
}
