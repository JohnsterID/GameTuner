using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework.BuildProcess
{
	public class BuildResultsForm : Form
	{
		private IBuildJob job;

		private IContainer components;

		private ToolStrip toolStrip1;

		private SplitterControl splitterControl1;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripButton toolStripButton1;

		private ToolStripButton buttonExportResults;

		private BuildResultsPanel buildResultPanel1;

		private ListBox listBox1;

		private CaptionControl captionControl1;

		private Button buttonOk;

		private SaveFileDialog saveFileDialog1;

		public string CaptionText
		{
			get
			{
				return captionControl1.Caption;
			}
			set
			{
				captionControl1.Caption = value;
			}
		}

		public BuildResultsForm(IBuildJob job)
		{
			this.job = job;
			InitializeComponent();
			buildResultPanel1.Job = job;
			foreach (string item in job.BuildResults.Log)
			{
				listBox1.Items.Add(item);
			}
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			splitterControl1.Panel2Collapsed = !splitterControl1.Panel2Collapsed;
		}

		private void buttonExportResults_Click(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog(this) != DialogResult.OK)
			{
				return;
			}
			XmlDoc xmlDoc = new XmlDoc();
			XmlNode node = xmlDoc.AddRoot("buildlog");
			xmlDoc.SetAttrib(node, "title", CaptionText);
			XmlNode xmlNode = xmlDoc.AddNode(node, "results");
			foreach (BuildResultsArgs.ResultInfo result in job.BuildResults.Results)
			{
				XmlNode node2 = xmlDoc.AddNode(xmlNode, "result");
				xmlDoc.SetAttrib(node2, "level", (int)result.Level);
				xmlDoc.SetAttrib(node2, "step", ReflectionHelper.GetDisplayName(result.BuildStep));
				xmlDoc.SetAttrib(node2, "args", result.BuildStep.Arguments);
				xmlDoc.SetText(node2, result.Brief);
			}
			XmlNode xmlNode2 = xmlDoc.AddNode(node, "logs");
			foreach (string item in job.BuildResults.Log)
			{
				XmlNode node2 = xmlDoc.AddNode(xmlNode2, "log");
				xmlDoc.SetText(node2, item);
			}
			xmlDoc.Save(saveFileDialog1.FileName);
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
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.buttonExportResults = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.splitterControl1 = new GameTuner.Framework.SplitterControl();
			this.buildResultPanel1 = new GameTuner.Framework.BuildProcess.BuildResultsPanel();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.captionControl1 = new GameTuner.Framework.CaptionControl();
			this.buttonOk = new System.Windows.Forms.Button();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.toolStrip1.SuspendLayout();
			this.splitterControl1.Panel1.SuspendLayout();
			this.splitterControl1.Panel2.SuspendLayout();
			this.splitterControl1.SuspendLayout();
			base.SuspendLayout();
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.buttonExportResults, this.toolStripSeparator1, this.toolStripButton1 });
			this.toolStrip1.Location = new System.Drawing.Point(0, 33);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip1.Size = new System.Drawing.Size(420, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			this.buttonExportResults.Image = GameTuner.Framework.Properties.Resources.Control_SaveFileDialog;
			this.buttonExportResults.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonExportResults.Name = "buttonExportResults";
			this.buttonExportResults.Size = new System.Drawing.Size(100, 22);
			this.buttonExportResults.Text = "Export Results";
			this.buttonExportResults.ToolTipText = "Export Results";
			this.buttonExportResults.Click += new System.EventHandler(buttonExportResults_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			this.toolStripButton1.Image = GameTuner.Framework.Properties.Resources.EditTable;
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(79, 22);
			this.toolStripButton1.Text = "Show Log";
			this.toolStripButton1.Click += new System.EventHandler(toolStripButton1_Click);
			this.splitterControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.splitterControl1.Location = new System.Drawing.Point(0, 58);
			this.splitterControl1.Name = "splitterControl1";
			this.splitterControl1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitterControl1.Panel1.Controls.Add(this.buildResultPanel1);
			this.splitterControl1.Panel2.Controls.Add(this.listBox1);
			this.splitterControl1.Panel2Collapsed = true;
			this.splitterControl1.Size = new System.Drawing.Size(420, 205);
			this.splitterControl1.SplitterDistance = 99;
			this.splitterControl1.TabIndex = 1;
			this.buildResultPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buildResultPanel1.Location = new System.Drawing.Point(0, 0);
			this.buildResultPanel1.Name = "buildResultPanel1";
			this.buildResultPanel1.Size = new System.Drawing.Size(420, 205);
			this.buildResultPanel1.TabIndex = 0;
			this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(0, 0);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(150, 43);
			this.listBox1.TabIndex = 0;
			this.captionControl1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.captionControl1.Caption = "Installer Results";
			this.captionControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.captionControl1.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.captionControl1.Image = GameTuner.Framework.Properties.Resources.access_level;
			this.captionControl1.Location = new System.Drawing.Point(0, 0);
			this.captionControl1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.captionControl1.Name = "captionControl1";
			this.captionControl1.Size = new System.Drawing.Size(420, 33);
			this.captionControl1.TabIndex = 2;
			this.captionControl1.Transparent = System.Drawing.Color.Magenta;
			this.buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(333, 269);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 23);
			this.buttonOk.TabIndex = 3;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.saveFileDialog1.DefaultExt = "xml";
			this.saveFileDialog1.Filter = "Xml Files (*.xml)|*.xml|All Files (*.*)|*.*";
			this.saveFileDialog1.Title = "Export Results";
			base.AcceptButton = this.buttonOk;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(420, 304);
			base.Controls.Add(this.buttonOk);
			base.Controls.Add(this.splitterControl1);
			base.Controls.Add(this.toolStrip1);
			base.Controls.Add(this.captionControl1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(293, 243);
			base.Name = "BuildResultsForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			this.Text = "Results";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.splitterControl1.Panel1.ResumeLayout(false);
			this.splitterControl1.Panel2.ResumeLayout(false);
			this.splitterControl1.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
