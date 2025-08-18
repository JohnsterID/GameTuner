using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework.Export
{
	public class GameTunerAnimationManager : Form
	{
		public const int MinEventCode = 1000;

		public const string DefaultAnimName = "NewAnimation";

		public const string DefaultLayerName = "NewLayer";

		private IAnimationManagerToolInterface m_kToolInterface;

		private IContainer components;

		private SplitContainer splitContainer1;

		private GroupBox groupBox1;

		private SplitContainer splitContainer2;

		private GroupBox groupBox2;

		private SplitContainer splitContainer3;

		private GroupBox groupBox3;

		private ListView AnimationListView;

		private ListView LayerListView;

		private GroupBox groupBox4;

		private ToolStrip AnimationToolstrip;

		private ToolStripButton AddAnimation;

		private ToolStripButton DeleteAnimation;

		private ToolStripButton ImportAnimations;

		private ToolStripButton ExportAnimations;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripButton AutoSyncTime;

		private ToolStripButton AutoPlayAnim;

		private ToolStrip LayerToolstrip;

		private ToolStripButton AddLayer;

		private ToolStripButton DeleteLayer;

		private ToolStrip BoneWeightsToolstrip;

		private ToolStripTextBox LayerWeight;

		private ToolStripLabel toolStripLabel1;

		private ToolStripButton ApplyAllWeight;

		private ToolStripButton ApplySelectedWeight;

		private ToolStripButton ApplyChildrenWeight;

		private ToolStripButton AutoSyncSelectionWeight;

		private ListView WeightsListView;

		private ColumnHeader columnHeader1;

		private ColumnHeader columnHeader2;

		private ColumnHeader columnHeader3;

		private ColumnHeader columnHeader4;

		private ColumnHeader columnHeader5;

		private ColumnHeader columnHeader6;

		private ColumnHeader columnHeader7;

		private ColumnHeader columnHeader8;

		private PropertyGridEx AnimationProperty;

		private OpenFileDialog ImportDefinitionsDialog;

		private SaveFileDialog ExportDefinitionsDialog;

		public ProjectConfig ProjectCfg
		{
			get
			{
				return Context.Get<ProjectConfig>();
			}
		}

		public IAnimationManagerToolInterface ToolInterface
		{
			get
			{
				if (m_kToolInterface == null)
				{
					m_kToolInterface = new NullAnimationToolInterface();
				}
				return m_kToolInterface;
			}
			set
			{
				m_kToolInterface = value;
				if (m_kToolInterface == null)
				{
					m_kToolInterface = new NullAnimationToolInterface();
				}
				PopulateAnimationList();
				PopulateLayerList();
				PopulateWeightList();
				UpdateEnabledUI();
			}
		}

		private List<AnimationEntry> AnimationDefinitions
		{
			get
			{
				return ToolInterface.AnimationList;
			}
			set
			{
				ToolInterface.AnimationList = value;
			}
		}

		private List<LayerEntry> LayerDefinitions
		{
			get
			{
				return ToolInterface.LayerList;
			}
			set
			{
				ToolInterface.LayerList = value;
			}
		}

		public GameTunerAnimationManager()
		{
			InitializeComponent();
			if (ProjectCfg != null)
			{
				AutoSyncTime.Checked = ProjectCfg.GetOption("AnimationManager", "AutoSyncAnimation", true);
				AutoPlayAnim.Checked = ProjectCfg.GetOption("AnimationManager", "AutoPlayAnimation", false);
				AutoSyncSelectionWeight.Checked = ProjectCfg.GetOption("AnimationManager", "AutoSyncModel", false);
			}
		}

		private ListViewItem AddAnimationToList(AnimationEntry kEntry)
		{
			ListViewItem listViewItem = AnimationListView.Items.Add(kEntry.AnimationName);
			listViewItem.SubItems.Add(kEntry.EventCode.ToString());
			listViewItem.SubItems.Add(kEntry.StartFrame.ToString());
			listViewItem.SubItems.Add(kEntry.EndFrame.ToString());
			listViewItem.SubItems.Add(kEntry.ExportSettings);
			listViewItem.Tag = kEntry;
			return listViewItem;
		}

		private ListViewItem AddLayerToList(LayerEntry kEntry)
		{
			ListViewItem listViewItem = LayerListView.Items.Add(kEntry.LayerName);
			listViewItem.Tag = kEntry;
			return listViewItem;
		}

		private void PopulateAnimationList()
		{
			AnimationListView.Items.Clear();
			foreach (AnimationEntry animationDefinition in AnimationDefinitions)
			{
				AddAnimationToList(animationDefinition);
			}
		}

		private void PopulateLayerList()
		{
			LayerListView.Items.Clear();
			foreach (LayerEntry layerDefinition in LayerDefinitions)
			{
				AddLayerToList(layerDefinition);
			}
		}

		private void PopulateWeightList()
		{
			string text = null;
			WeightsListView.Items.Clear();
			if (LayerListView.SelectedItems.Count > 0)
			{
				ListViewItem listViewItem = LayerListView.SelectedItems[0];
				LayerEntry layerEntry = listViewItem.Tag as LayerEntry;
				text = layerEntry.LayerName;
			}
			else
			{
				text = null;
			}
			foreach (string bone in ToolInterface.BoneList)
			{
				ListViewItem listViewItem2 = WeightsListView.Items.Add(bone);
				if (text != null)
				{
					listViewItem2.SubItems.Add(ToolInterface.GetBoneWeight(text, bone).ToString());
				}
				else
				{
					listViewItem2.SubItems.Add(Convert.ToString(0f));
				}
			}
		}

		private void UpdateEnabledUI()
		{
			DeleteAnimation.Enabled = AnimationListView.SelectedItems.Count > 0;
			DeleteLayer.Enabled = LayerListView.SelectedItems.Count > 0;
			ApplyAllWeight.Enabled = LayerListView.SelectedItems.Count > 0;
			ApplySelectedWeight.Enabled = WeightsListView.SelectedItems.Count > 0;
			ApplyChildrenWeight.Enabled = WeightsListView.SelectedItems.Count > 0;
			WeightsListView.Enabled = LayerListView.SelectedItems.Count > 0;
		}

		private string GetUnusedAnimationName()
		{
			int num = 0;
			string szUnusedName;
			do
			{
				szUnusedName = string.Format("{0}{1:D4}", "NewAnimation", num++);
			}
			while (AnimationDefinitions.Find((AnimationEntry kE) => kE.AnimationName == szUnusedName) != null);
			return szUnusedName;
		}

		private string GetUnusedLayerName()
		{
			int num = 0;
			string szUnusedName;
			do
			{
				szUnusedName = string.Format("{0}{1:D2}", "NewLayer", num++);
			}
			while (LayerDefinitions.Find((LayerEntry kE) => kE.LayerName == szUnusedName) != null);
			return szUnusedName;
		}

		private int GetUnusedEventCode()
		{
			int iEventCode;
			for (iEventCode = 1000; AnimationDefinitions.Find((AnimationEntry kE) => kE.EventCode == iEventCode) != null; iEventCode++)
			{
			}
			return iEventCode;
		}

		private void AddAnimation_Click(object sender, EventArgs e)
		{
			AnimationEntry animationEntry = new AnimationEntry(ToolInterface);
			animationEntry.AnimationName = GetUnusedAnimationName();
			animationEntry.EventCode = GetUnusedEventCode();
			animationEntry.StartFrame = ToolInterface.StartFrame;
			animationEntry.EndFrame = ToolInterface.EndFrame;
			animationEntry.ExportSettings = AESGlobalVars.AnimExportSettings[0];
			AnimationDefinitions.Add(animationEntry);
			ListViewItem listViewItem = AddAnimationToList(animationEntry);
			AnimationListView.SelectedItems.Clear();
			listViewItem.Selected = true;
		}

		private void AnimationList_SelectedIndexChanged(object sender, EventArgs e)
		{
			ICollection<AnimationEntry> collection = new List<AnimationEntry>();
			foreach (ListViewItem selectedItem in AnimationListView.SelectedItems)
			{
				collection.Add(selectedItem.Tag as AnimationEntry);
			}
			AnimationProperty.BeginSelected();
			AnimationProperty.PushSelected(collection);
			AnimationProperty.EndSelected();
			if (AutoSyncTime.Checked && collection.Count > 0)
			{
				int num = int.MinValue;
				int num2 = int.MaxValue;
				foreach (AnimationEntry item in collection)
				{
					num = Math.Max(num, item.StartFrame);
					num2 = Math.Min(num2, item.EndFrame);
				}
				ToolInterface.StartFrame = num;
				ToolInterface.EndFrame = num2;
			}
			if (AutoPlayAnim.Checked)
			{
				ToolInterface.PlayAnimation();
			}
			UpdateEnabledUI();
		}

		private void DeleteAnimation_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem selectedItem in AnimationListView.SelectedItems)
			{
				AnimationDefinitions.Remove(selectedItem.Tag as AnimationEntry);
			}
			PopulateAnimationList();
			UpdateEnabledUI();
		}

		private void UpdateAnimationEntry(ListViewItem kItem)
		{
			AnimationEntry animationEntry = kItem.Tag as AnimationEntry;
			kItem.SubItems[0].Text = animationEntry.AnimationName;
			kItem.SubItems[1].Text = animationEntry.EventCode.ToString();
			kItem.SubItems[2].Text = animationEntry.StartFrame.ToString();
			kItem.SubItems[3].Text = animationEntry.EndFrame.ToString();
			kItem.SubItems[4].Text = animationEntry.ExportSettings;
		}

		private void AnimationProperty_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			foreach (ListViewItem selectedItem in AnimationListView.SelectedItems)
			{
				UpdateAnimationEntry(selectedItem);
			}
		}

		private void ImportAnimations_Click(object sender, EventArgs e)
		{
			ImportDefinitionsDialog.Filter = ToolInterface.ImportFilter;
			DialogResult dialogResult = ImportDefinitionsDialog.ShowDialog(this);
			if (dialogResult != DialogResult.Cancel)
			{
				dialogResult = MessageBox.Show("Do you wish to retain currently defined animations? (Selecting no will remove all currently defined animations)", "Keep Current Definitions?", MessageBoxButtons.YesNo);
				if (dialogResult == DialogResult.No)
				{
					ToolInterface.AnimationList.Clear();
				}
				string[] fileNames = ImportDefinitionsDialog.FileNames;
				foreach (string szFilename in fileNames)
				{
					ToolInterface.ImportFromFile(szFilename);
				}
				PopulateAnimationList();
			}
		}

		private void ExportAnimations_Click(object sender, EventArgs e)
		{
			ExportDefinitionsDialog.Filter = ToolInterface.ExportFilter;
			DialogResult dialogResult = ExportDefinitionsDialog.ShowDialog(this);
			if (dialogResult != DialogResult.Cancel)
			{
				string[] fileNames = ExportDefinitionsDialog.FileNames;
				foreach (string szFilename in fileNames)
				{
					ToolInterface.ExportToFile(szFilename);
				}
			}
		}

		private void AddLayer_Click(object sender, EventArgs e)
		{
			LayerEntry layerEntry = new LayerEntry();
			layerEntry.LayerName = GetUnusedLayerName();
			LayerDefinitions.Add(layerEntry);
			LayerListView.SelectedItems.Clear();
			AddLayerToList(layerEntry).Selected = true;
		}

		private void DeleteLayer_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem selectedItem in LayerListView.SelectedItems)
			{
				LayerDefinitions.Remove(selectedItem.Tag as LayerEntry);
			}
			PopulateLayerList();
			UpdateEnabledUI();
		}

		private void LayerListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListView listView = sender as ListView;
			if (listView.SelectedItems.Count != 0)
			{
				PopulateWeightList();
				UpdateEnabledUI();
			}
		}

		private void AutoSyncTime_CheckedChanged(object sender, EventArgs e)
		{
			ProjectCfg.SetOption("AnimationManager", "AutoSyncAnimation", AutoSyncTime.Checked);
		}

		private void AutoPlayAnim_CheckedChanged(object sender, EventArgs e)
		{
			ProjectCfg.SetOption("AnimationManager", "AutoPlayAnimation", AutoPlayAnim.Checked);
		}

		private void AutoSyncSelectionWeight_CheckedChanged(object sender, EventArgs e)
		{
			ProjectCfg.SetOption("AnimationManager", "AutoSyncModel", AutoSyncSelectionWeight.Checked);
		}

		private void ApplyAllWeight_Click(object sender, EventArgs e)
		{
			float fWeight = (float)Convert.ToDouble(LayerWeight.Text);
			foreach (ListViewItem selectedItem in LayerListView.SelectedItems)
			{
				foreach (ListViewItem item in WeightsListView.Items)
				{
					ToolInterface.SetBoneWeight(selectedItem.Text, item.Text, fWeight);
					item.SubItems[1].Text = fWeight.ToString();
				}
			}
		}

		private void ApplySelectedWeight_Click(object sender, EventArgs e)
		{
			float fWeight = (float)Convert.ToDouble(LayerWeight.Text);
			foreach (ListViewItem selectedItem in LayerListView.SelectedItems)
			{
				foreach (ListViewItem selectedItem2 in WeightsListView.SelectedItems)
				{
					ToolInterface.SetBoneWeight(selectedItem.Text, selectedItem2.Text, fWeight);
					selectedItem2.SubItems[1].Text = fWeight.ToString();
				}
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.AnimationListView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.AnimationToolstrip = new System.Windows.Forms.ToolStrip();
			this.AddAnimation = new System.Windows.Forms.ToolStripButton();
			this.DeleteAnimation = new System.Windows.Forms.ToolStripButton();
			this.ImportAnimations = new System.Windows.Forms.ToolStripButton();
			this.ExportAnimations = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.AutoSyncTime = new System.Windows.Forms.ToolStripButton();
			this.AutoPlayAnim = new System.Windows.Forms.ToolStripButton();
			this.AnimationProperty = new GameTuner.Framework.PropertyGridEx();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.LayerListView = new System.Windows.Forms.ListView();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.LayerToolstrip = new System.Windows.Forms.ToolStrip();
			this.AddLayer = new System.Windows.Forms.ToolStripButton();
			this.DeleteLayer = new System.Windows.Forms.ToolStripButton();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.WeightsListView = new System.Windows.Forms.ListView();
			this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
			this.BoneWeightsToolstrip = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.LayerWeight = new System.Windows.Forms.ToolStripTextBox();
			this.ApplyAllWeight = new System.Windows.Forms.ToolStripButton();
			this.ApplySelectedWeight = new System.Windows.Forms.ToolStripButton();
			this.ApplyChildrenWeight = new System.Windows.Forms.ToolStripButton();
			this.AutoSyncSelectionWeight = new System.Windows.Forms.ToolStripButton();
			this.ImportDefinitionsDialog = new System.Windows.Forms.OpenFileDialog();
			this.ExportDefinitionsDialog = new System.Windows.Forms.SaveFileDialog();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.AnimationToolstrip.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.LayerToolstrip.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.BoneWeightsToolstrip.SuspendLayout();
			base.SuspendLayout();
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
			this.splitContainer1.Size = new System.Drawing.Size(710, 673);
			this.splitContainer1.SplitterDistance = 447;
			this.splitContainer1.TabIndex = 0;
			this.groupBox1.Controls.Add(this.splitContainer2);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(710, 447);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Animation Definitions";
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(3, 16);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Panel1.Controls.Add(this.AnimationListView);
			this.splitContainer2.Panel1.Controls.Add(this.AnimationToolstrip);
			this.splitContainer2.Panel2.Controls.Add(this.AnimationProperty);
			this.splitContainer2.Size = new System.Drawing.Size(704, 428);
			this.splitContainer2.SplitterDistance = 469;
			this.splitContainer2.TabIndex = 0;
			this.AnimationListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[5] { this.columnHeader1, this.columnHeader2, this.columnHeader3, this.columnHeader4, this.columnHeader5 });
			this.AnimationListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AnimationListView.FullRowSelect = true;
			this.AnimationListView.GridLines = true;
			this.AnimationListView.HideSelection = false;
			this.AnimationListView.Location = new System.Drawing.Point(0, 0);
			this.AnimationListView.Name = "AnimationListView";
			this.AnimationListView.Size = new System.Drawing.Size(469, 403);
			this.AnimationListView.TabIndex = 1;
			this.AnimationListView.UseCompatibleStateImageBehavior = false;
			this.AnimationListView.View = System.Windows.Forms.View.Details;
			this.AnimationListView.SelectedIndexChanged += new System.EventHandler(AnimationList_SelectedIndexChanged);
			this.columnHeader1.Text = "Animation Name";
			this.columnHeader1.Width = 126;
			this.columnHeader2.Text = "EventCode";
			this.columnHeader2.Width = 81;
			this.columnHeader3.Text = "Start Frame";
			this.columnHeader3.Width = 71;
			this.columnHeader4.Text = "End Frame";
			this.columnHeader4.Width = 74;
			this.columnHeader5.Text = "Export Settings";
			this.columnHeader5.Width = 97;
			this.AnimationToolstrip.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.AnimationToolstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.AnimationToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[7] { this.AddAnimation, this.DeleteAnimation, this.ImportAnimations, this.ExportAnimations, this.toolStripSeparator1, this.AutoSyncTime, this.AutoPlayAnim });
			this.AnimationToolstrip.Location = new System.Drawing.Point(0, 403);
			this.AnimationToolstrip.Margin = new System.Windows.Forms.Padding(1);
			this.AnimationToolstrip.Name = "AnimationToolstrip";
			this.AnimationToolstrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.AnimationToolstrip.Size = new System.Drawing.Size(469, 25);
			this.AnimationToolstrip.TabIndex = 2;
			this.AnimationToolstrip.Text = "toolStrip1";
			this.AddAnimation.Image = GameTuner.Framework.Properties.Resources.file_document;
			this.AddAnimation.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.AddAnimation.Name = "AddAnimation";
			this.AddAnimation.Size = new System.Drawing.Size(46, 22);
			this.AddAnimation.Text = "Add";
			this.AddAnimation.ToolTipText = "Adds a new animation to the definition list";
			this.AddAnimation.Click += new System.EventHandler(AddAnimation_Click);
			this.DeleteAnimation.Image = GameTuner.Framework.Properties.Resources.tool_delete;
			this.DeleteAnimation.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.DeleteAnimation.Name = "DeleteAnimation";
			this.DeleteAnimation.Size = new System.Drawing.Size(58, 22);
			this.DeleteAnimation.Text = "Delete";
			this.DeleteAnimation.ToolTipText = "Deletes the selection animation(s)";
			this.DeleteAnimation.Click += new System.EventHandler(DeleteAnimation_Click);
			this.ImportAnimations.Image = GameTuner.Framework.Properties.Resources.import_asset;
			this.ImportAnimations.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ImportAnimations.Name = "ImportAnimations";
			this.ImportAnimations.Size = new System.Drawing.Size(56, 22);
			this.ImportAnimations.Text = "Import";
			this.ImportAnimations.ToolTipText = "Import animation deifinitions from a previously export animation definition list";
			this.ImportAnimations.Click += new System.EventHandler(ImportAnimations_Click);
			this.ExportAnimations.Image = GameTuner.Framework.Properties.Resources.published;
			this.ExportAnimations.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ExportAnimations.Name = "ExportAnimations";
			this.ExportAnimations.Size = new System.Drawing.Size(57, 22);
			this.ExportAnimations.Text = "Export";
			this.ExportAnimations.ToolTipText = "Export the animation definition list, so it may be imported to another definition list";
			this.ExportAnimations.Click += new System.EventHandler(ExportAnimations_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			this.AutoSyncTime.CheckOnClick = true;
			this.AutoSyncTime.Image = GameTuner.Framework.Properties.Resources.time_scale;
			this.AutoSyncTime.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.AutoSyncTime.Name = "AutoSyncTime";
			this.AutoSyncTime.Size = new System.Drawing.Size(99, 22);
			this.AutoSyncTime.Text = "AutoSync Time";
			this.AutoSyncTime.ToolTipText = "Automatically synchronizes the tool's timeline to the ranges of the selected animation(s)";
			this.AutoSyncTime.CheckedChanged += new System.EventHandler(AutoSyncTime_CheckedChanged);
			this.AutoPlayAnim.CheckOnClick = true;
			this.AutoPlayAnim.Image = GameTuner.Framework.Properties.Resources.vcr_play;
			this.AutoPlayAnim.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.AutoPlayAnim.Name = "AutoPlayAnim";
			this.AutoPlayAnim.Size = new System.Drawing.Size(95, 22);
			this.AutoPlayAnim.Text = "AutoPlay Anim";
			this.AutoPlayAnim.ToolTipText = "Automatically plays the animation when selected in the definition list";
			this.AutoPlayAnim.CheckedChanged += new System.EventHandler(AutoPlayAnim_CheckedChanged);
			this.AnimationProperty.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AnimationProperty.Filter = "";
			this.AnimationProperty.Location = new System.Drawing.Point(0, 0);
			this.AnimationProperty.Name = "AnimationProperty";
			this.AnimationProperty.ReadOnly = false;
			this.AnimationProperty.Size = new System.Drawing.Size(231, 428);
			this.AnimationProperty.TabIndex = 0;
			this.AnimationProperty.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(AnimationProperty_PropertyValueChanged);
			this.groupBox2.Controls.Add(this.splitContainer3);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(0, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(710, 222);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Animation Layer Definitions";
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(3, 16);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Panel1.Controls.Add(this.groupBox3);
			this.splitContainer3.Panel2.Controls.Add(this.groupBox4);
			this.splitContainer3.Size = new System.Drawing.Size(704, 203);
			this.splitContainer3.SplitterDistance = 260;
			this.splitContainer3.TabIndex = 0;
			this.groupBox3.Controls.Add(this.LayerListView);
			this.groupBox3.Controls.Add(this.LayerToolstrip);
			this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox3.Location = new System.Drawing.Point(0, 0);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(260, 203);
			this.groupBox3.TabIndex = 0;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Layer List";
			this.LayerListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.columnHeader6 });
			this.LayerListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LayerListView.FullRowSelect = true;
			this.LayerListView.GridLines = true;
			this.LayerListView.HideSelection = false;
			this.LayerListView.LabelEdit = true;
			this.LayerListView.Location = new System.Drawing.Point(3, 16);
			this.LayerListView.Name = "LayerListView";
			this.LayerListView.Size = new System.Drawing.Size(254, 159);
			this.LayerListView.TabIndex = 3;
			this.LayerListView.UseCompatibleStateImageBehavior = false;
			this.LayerListView.View = System.Windows.Forms.View.Details;
			this.LayerListView.SelectedIndexChanged += new System.EventHandler(LayerListView_SelectedIndexChanged);
			this.columnHeader6.Text = "Layer Name";
			this.columnHeader6.Width = 241;
			this.LayerToolstrip.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.LayerToolstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.LayerToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.AddLayer, this.DeleteLayer });
			this.LayerToolstrip.Location = new System.Drawing.Point(3, 175);
			this.LayerToolstrip.Name = "LayerToolstrip";
			this.LayerToolstrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.LayerToolstrip.Size = new System.Drawing.Size(254, 25);
			this.LayerToolstrip.TabIndex = 4;
			this.LayerToolstrip.Text = "toolStrip1";
			this.AddLayer.Image = GameTuner.Framework.Properties.Resources.file_document;
			this.AddLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.AddLayer.Name = "AddLayer";
			this.AddLayer.Size = new System.Drawing.Size(46, 22);
			this.AddLayer.Text = "Add";
			this.AddLayer.ToolTipText = "Adds a layer to the layer definition list";
			this.AddLayer.Click += new System.EventHandler(AddLayer_Click);
			this.DeleteLayer.Image = GameTuner.Framework.Properties.Resources.tool_delete;
			this.DeleteLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.DeleteLayer.Name = "DeleteLayer";
			this.DeleteLayer.Size = new System.Drawing.Size(58, 22);
			this.DeleteLayer.Text = "Delete";
			this.DeleteLayer.Click += new System.EventHandler(DeleteLayer_Click);
			this.groupBox4.Controls.Add(this.WeightsListView);
			this.groupBox4.Controls.Add(this.BoneWeightsToolstrip);
			this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox4.Location = new System.Drawing.Point(0, 0);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(440, 203);
			this.groupBox4.TabIndex = 0;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Bone Weights for Selected Layer";
			this.WeightsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.columnHeader7, this.columnHeader8 });
			this.WeightsListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WeightsListView.FullRowSelect = true;
			this.WeightsListView.GridLines = true;
			this.WeightsListView.Location = new System.Drawing.Point(3, 16);
			this.WeightsListView.Name = "WeightsListView";
			this.WeightsListView.Size = new System.Drawing.Size(313, 184);
			this.WeightsListView.TabIndex = 1;
			this.WeightsListView.UseCompatibleStateImageBehavior = false;
			this.WeightsListView.View = System.Windows.Forms.View.Details;
			this.columnHeader7.Text = "Bone Name";
			this.columnHeader7.Width = 151;
			this.columnHeader8.Text = "Layer Weight";
			this.columnHeader8.Width = 140;
			this.BoneWeightsToolstrip.Dock = System.Windows.Forms.DockStyle.Right;
			this.BoneWeightsToolstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.BoneWeightsToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.toolStripLabel1, this.LayerWeight, this.ApplyAllWeight, this.ApplySelectedWeight, this.ApplyChildrenWeight, this.AutoSyncSelectionWeight });
			this.BoneWeightsToolstrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
			this.BoneWeightsToolstrip.Location = new System.Drawing.Point(316, 16);
			this.BoneWeightsToolstrip.Name = "BoneWeightsToolstrip";
			this.BoneWeightsToolstrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.BoneWeightsToolstrip.Size = new System.Drawing.Size(121, 184);
			this.BoneWeightsToolstrip.TabIndex = 0;
			this.BoneWeightsToolstrip.Text = "toolStrip1";
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(41, 13);
			this.toolStripLabel1.Text = "Weight";
			this.LayerWeight.Name = "LayerWeight";
			this.LayerWeight.Size = new System.Drawing.Size(116, 20);
			this.LayerWeight.Text = "1.0";
			this.ApplyAllWeight.Image = GameTuner.Framework.Properties.Resources.brush04;
			this.ApplyAllWeight.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ApplyAllWeight.Name = "ApplyAllWeight";
			this.ApplyAllWeight.Size = new System.Drawing.Size(67, 20);
			this.ApplyAllWeight.Text = "Apply All";
			this.ApplyAllWeight.ToolTipText = "Applies the weight to all bones in the current layer";
			this.ApplyAllWeight.Click += new System.EventHandler(ApplyAllWeight_Click);
			this.ApplySelectedWeight.Image = GameTuner.Framework.Properties.Resources.brush03;
			this.ApplySelectedWeight.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ApplySelectedWeight.Name = "ApplySelectedWeight";
			this.ApplySelectedWeight.Size = new System.Drawing.Size(98, 20);
			this.ApplySelectedWeight.Text = "Apply Selected";
			this.ApplySelectedWeight.ToolTipText = "Applies weight to selected bones in the list";
			this.ApplySelectedWeight.Click += new System.EventHandler(ApplySelectedWeight_Click);
			this.ApplyChildrenWeight.Image = GameTuner.Framework.Properties.Resources.brush02;
			this.ApplyChildrenWeight.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ApplyChildrenWeight.Name = "ApplyChildrenWeight";
			this.ApplyChildrenWeight.Size = new System.Drawing.Size(94, 20);
			this.ApplyChildrenWeight.Text = "Apply Children";
			this.ApplyChildrenWeight.ToolTipText = "Applies weight to the selected nodes, and all their children";
			this.AutoSyncSelectionWeight.CheckOnClick = true;
			this.AutoSyncSelectionWeight.Image = GameTuner.Framework.Properties.Resources.Control_ImageList;
			this.AutoSyncSelectionWeight.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.AutoSyncSelectionWeight.Name = "AutoSyncSelectionWeight";
			this.AutoSyncSelectionWeight.Size = new System.Drawing.Size(120, 20);
			this.AutoSyncSelectionWeight.Text = "AutoSync Selection";
			this.AutoSyncSelectionWeight.ToolTipText = "Automatically synchronizes the scene selection to the bones selected in the weight list";
			this.AutoSyncSelectionWeight.CheckedChanged += new System.EventHandler(AutoSyncSelectionWeight_CheckedChanged);
			this.ImportDefinitionsDialog.Filter = "XML Files|*.xml|GameTuner XML Files|*.fxsxml";
			this.ImportDefinitionsDialog.Multiselect = true;
			this.ImportDefinitionsDialog.Title = "Select File(s) containing definitions to import";
			this.ExportDefinitionsDialog.Title = "Select File to export definitions to";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(710, 673);
			base.Controls.Add(this.splitContainer1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			base.HelpButton = true;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "GameTunerAnimationManager";
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "GameTuner Animation Manager";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.AnimationToolstrip.ResumeLayout(false);
			this.AnimationToolstrip.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.LayerToolstrip.ResumeLayout(false);
			this.LayerToolstrip.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.BoneWeightsToolstrip.ResumeLayout(false);
			this.BoneWeightsToolstrip.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
