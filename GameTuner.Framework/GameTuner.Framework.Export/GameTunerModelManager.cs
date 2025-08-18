using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework.Export
{
	public class GameTunerModelManager : Form
	{
		private IModelManagerToolInterface m_kToolInterface;

		private static string DefaultGroupName = "Group";

		private IContainer components;

		private SplitContainer splitContainer1;

		private GroupBox groupBox1;

		private GroupBox groupBox2;

		private SplitContainer splitContainer2;

		private ToolStrip ModelGroupToolstrip;

		private ToolStripButton GroupAllOne;

		private ToolStripButton GroupEachOne;

		private ToolStripButton GroupAdd;

		private ToolStripButton GroupDelete;

		private ToolStripButton AutoSyncGroup;

		private ListView GroupList;

		private ListView MeshList;

		private ToolStrip MeshListToolstrip;

		private ColumnHeader columnHeader1;

		private ColumnHeader columnHeader2;

		private ToolStripButton MeshAddToAll;

		private ToolStripButton MeshAddToSelected;

		private ToolStripButton MeshRemoveFromAll;

		private ToolStripButton MeshRemoveFromSelected;

		private ColumnHeader columnHeader3;

		private ColumnHeader columnHeader4;

		private ColumnHeader columnHeader5;

		private PropertyGridEx GroupProperty;

		public IModelManagerToolInterface ToolInterface
		{
			get
			{
				if (m_kToolInterface == null)
				{
					m_kToolInterface = new NullModelToolInterface();
				}
				return m_kToolInterface;
			}
			set
			{
				m_kToolInterface = value;
				if (m_kToolInterface == null)
				{
					m_kToolInterface = new NullModelToolInterface();
				}
				PopulateMeshAndGroupLists(false, false);
			}
		}

		public ProjectConfig ProjectCfg
		{
			get
			{
				return Context.Get<ProjectConfig>();
			}
		}

		private ListViewItem FindItemByTag(object TagValue, ListView kList)
		{
			foreach (ListViewItem item in kList.Items)
			{
				if (item.Tag == TagValue)
				{
					return item;
				}
			}
			return null;
		}

		private void PopulateMeshAndGroupLists(bool bReselectMeshes, bool bReselectGroups)
		{
			List<ModelGroupEntry> modelGroupList = ToolInterface.ModelGroupList;
			Dictionary<string, List<int>> dictionary = new Dictionary<string, List<int>>();
			if (!bReselectGroups)
			{
				GroupList.Items.Clear();
				GroupProperty.SelectedObjects = null;
			}
			if (!bReselectMeshes)
			{
				MeshList.Items.Clear();
			}
			foreach (ModelGroupEntry item in modelGroupList)
			{
				ListViewItem listViewItem = FindItemByTag(item, GroupList);
				if (listViewItem == null)
				{
					listViewItem = GroupList.Items.Add(GroupList.Items.Count.ToString());
					listViewItem.Tag = item;
				}
				while (listViewItem.SubItems.Count < 4)
				{
					listViewItem.SubItems.Add("temp");
				}
				listViewItem.SubItems[1].Text = item.ModelGroupName;
				listViewItem.SubItems[2].Text = item.Meshes.Count.ToString();
				listViewItem.SubItems[3].Text = item.UsesAnimations.ToString();
				foreach (string mesh in item.Meshes)
				{
					if (!dictionary.ContainsKey(mesh))
					{
						dictionary[mesh] = new List<int>();
					}
					dictionary[mesh].Add(Convert.ToInt32(listViewItem.Text));
				}
			}
			foreach (string mesh2 in ToolInterface.MeshList)
			{
				ListViewItem listViewItem2 = MeshList.FindItemWithText(mesh2);
				if (listViewItem2 == null)
				{
					listViewItem2 = MeshList.Items.Add(mesh2);
					listViewItem2.SubItems.Add("0");
				}
				string text = "";
				bool flag = true;
				if (dictionary.ContainsKey(mesh2))
				{
					foreach (int item2 in dictionary[mesh2])
					{
						text += string.Format("{0}{1}", flag ? "" : ", ", item2.ToString());
						flag = false;
					}
				}
				else
				{
					text = "None";
				}
				listViewItem2.SubItems[1].Text = text;
			}
			UpdateEnabledUI();
		}

		public GameTunerModelManager()
		{
			InitializeComponent();
			if (ProjectCfg != null)
			{
				AutoSyncGroup.Checked = Convert.ToBoolean(ProjectCfg.GetOption("ModelManager", "AutoSyncModel", true));
			}
		}

		private string GetUnusedGroupName()
		{
			int num = 0;
			string szUnusedName;
			do
			{
				szUnusedName = string.Format("{0}{1:D2}", DefaultGroupName, num++);
			}
			while (ToolInterface.ModelGroupList.Find((ModelGroupEntry kE) => kE.ModelGroupName == szUnusedName) != null);
			return szUnusedName;
		}

		private void GroupList_SelectedIndexChanged(object sender, EventArgs e)
		{
			GroupProperty.BeginSelected();
			foreach (ListViewItem selectedItem in GroupList.SelectedItems)
			{
				GroupProperty.PushSelected(selectedItem.Tag);
			}
			GroupProperty.EndSelected();
			if (AutoSyncGroup.Checked)
			{
				ToolInterface.StartSelect();
				foreach (ListViewItem selectedItem2 in GroupList.SelectedItems)
				{
					ToolInterface.SelectGroup(selectedItem2.Tag as ModelGroupEntry);
				}
				ToolInterface.EndSelect();
			}
			UpdateEnabledUI();
		}

		private void MeshAddToAll_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem selectedItem in MeshList.SelectedItems)
			{
				foreach (ListViewItem item in GroupList.Items)
				{
					ModelGroupEntry modelGroupEntry = item.Tag as ModelGroupEntry;
					if (!modelGroupEntry.Meshes.Contains(selectedItem.Text))
					{
						modelGroupEntry.Meshes.Add(selectedItem.Text);
					}
				}
			}
			PopulateMeshAndGroupLists(true, true);
		}

		private void MeshAddToSelected_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem selectedItem in MeshList.SelectedItems)
			{
				foreach (ListViewItem selectedItem2 in GroupList.SelectedItems)
				{
					ModelGroupEntry modelGroupEntry = selectedItem2.Tag as ModelGroupEntry;
					if (!modelGroupEntry.Meshes.Contains(selectedItem.Text))
					{
						modelGroupEntry.Meshes.Add(selectedItem.Text);
					}
				}
			}
			PopulateMeshAndGroupLists(true, true);
		}

		private void MeshRemoveFromAll_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem selectedItem in MeshList.SelectedItems)
			{
				foreach (ListViewItem item in GroupList.Items)
				{
					ModelGroupEntry modelGroupEntry = item.Tag as ModelGroupEntry;
					modelGroupEntry.Meshes.Remove(selectedItem.Text);
				}
			}
			PopulateMeshAndGroupLists(true, true);
		}

		private void MeshRemoveFromSelected_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem selectedItem in MeshList.SelectedItems)
			{
				foreach (ListViewItem selectedItem2 in GroupList.SelectedItems)
				{
					ModelGroupEntry modelGroupEntry = selectedItem2.Tag as ModelGroupEntry;
					modelGroupEntry.Meshes.Remove(selectedItem.Text);
				}
			}
			PopulateMeshAndGroupLists(true, true);
		}

		private void GroupAllOne_Click(object sender, EventArgs e)
		{
			GroupList.Items.Clear();
			if (MessageBox.Show("This will delete all currently defined groups. Are you sure you wish to continue?", "Delete All Groups?", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				ToolInterface.AddDefaultGroup();
			}
			PopulateMeshAndGroupLists(true, false);
		}

		private void GroupEachOne_Click(object sender, EventArgs e)
		{
			GroupList.Items.Clear();
			if (MessageBox.Show("This will delete all currently defined groups. Are you sure you wish to continue?", "Delete All Groups?", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				ToolInterface.AddOneGroupPerMesh();
			}
			PopulateMeshAndGroupLists(true, false);
		}

		private void GroupAdd_Click(object sender, EventArgs e)
		{
			ModelGroupEntry modelGroupEntry = new ModelGroupEntry(ToolInterface);
			modelGroupEntry.ModelGroupName = GetUnusedGroupName();
			modelGroupEntry.UsesAnimations = true;
			modelGroupEntry.ExportSettings = MESGlobalVars.ModelExportSettings[0];
			ToolInterface.ModelGroupList.Add(modelGroupEntry);
			PopulateMeshAndGroupLists(true, true);
		}

		private void GroupDelete_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem selectedItem in GroupList.SelectedItems)
			{
				ModelGroupEntry item = selectedItem.Tag as ModelGroupEntry;
				ToolInterface.ModelGroupList.Remove(item);
			}
			PopulateMeshAndGroupLists(true, false);
		}

		private void GroupProperty_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			PropertyGrid propertyGrid = s as PropertyGrid;
			propertyGrid.Refresh();
			PopulateMeshAndGroupLists(true, true);
		}

		private void UpdateEnabledUI()
		{
			bool flag = MeshList.SelectedItems.Count > 0;
			bool flag2 = GroupList.SelectedItems.Count > 0;
			MeshAddToAll.Enabled = flag;
			MeshRemoveFromAll.Enabled = flag;
			MeshAddToSelected.Enabled = flag && flag2;
			MeshRemoveFromSelected.Enabled = flag && flag2;
			GroupDelete.Enabled = flag2;
			GroupProperty.Enabled = flag2;
		}

		private void MeshList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			UpdateEnabledUI();
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
			this.MeshList = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.MeshListToolstrip = new System.Windows.Forms.ToolStrip();
			this.MeshAddToAll = new System.Windows.Forms.ToolStripButton();
			this.MeshAddToSelected = new System.Windows.Forms.ToolStripButton();
			this.MeshRemoveFromAll = new System.Windows.Forms.ToolStripButton();
			this.MeshRemoveFromSelected = new System.Windows.Forms.ToolStripButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.ModelGroupToolstrip = new System.Windows.Forms.ToolStrip();
			this.GroupAllOne = new System.Windows.Forms.ToolStripButton();
			this.GroupEachOne = new System.Windows.Forms.ToolStripButton();
			this.GroupAdd = new System.Windows.Forms.ToolStripButton();
			this.GroupDelete = new System.Windows.Forms.ToolStripButton();
			this.AutoSyncGroup = new System.Windows.Forms.ToolStripButton();
			this.GroupList = new System.Windows.Forms.ListView();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.GroupProperty = new GameTuner.Framework.PropertyGridEx();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.MeshListToolstrip.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.ModelGroupToolstrip.SuspendLayout();
			base.SuspendLayout();
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
			this.splitContainer1.Size = new System.Drawing.Size(658, 624);
			this.splitContainer1.SplitterDistance = 301;
			this.splitContainer1.TabIndex = 0;
			this.groupBox1.Controls.Add(this.MeshList);
			this.groupBox1.Controls.Add(this.MeshListToolstrip);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(658, 301);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Scene Mesh List";
			this.MeshList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.columnHeader1, this.columnHeader2 });
			this.MeshList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MeshList.FullRowSelect = true;
			this.MeshList.GridLines = true;
			this.MeshList.HideSelection = false;
			this.MeshList.Location = new System.Drawing.Point(3, 16);
			this.MeshList.Name = "MeshList";
			this.MeshList.Size = new System.Drawing.Size(652, 236);
			this.MeshList.TabIndex = 1;
			this.MeshList.UseCompatibleStateImageBehavior = false;
			this.MeshList.View = System.Windows.Forms.View.Details;
			this.MeshList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(MeshList_ItemSelectionChanged);
			this.columnHeader1.Text = "Mesh Name";
			this.columnHeader1.Width = 173;
			this.columnHeader2.Text = "Member of Groups";
			this.columnHeader2.Width = 441;
			this.MeshListToolstrip.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.MeshListToolstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.MeshListToolstrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.MeshListToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.MeshAddToAll, this.MeshAddToSelected, this.MeshRemoveFromAll, this.MeshRemoveFromSelected });
			this.MeshListToolstrip.Location = new System.Drawing.Point(3, 252);
			this.MeshListToolstrip.Name = "MeshListToolstrip";
			this.MeshListToolstrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.MeshListToolstrip.Size = new System.Drawing.Size(652, 46);
			this.MeshListToolstrip.TabIndex = 0;
			this.MeshListToolstrip.Text = "toolStrip1";
			this.MeshAddToAll.Image = GameTuner.Framework.Properties.Resources.imageset_add_24;
			this.MeshAddToAll.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.MeshAddToAll.Name = "MeshAddToAll";
			this.MeshAddToAll.Size = new System.Drawing.Size(67, 43);
			this.MeshAddToAll.Text = "Add To All";
			this.MeshAddToAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.MeshAddToAll.Click += new System.EventHandler(MeshAddToAll_Click);
			this.MeshAddToSelected.Image = GameTuner.Framework.Properties.Resources.image_add_24;
			this.MeshAddToSelected.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.MeshAddToSelected.Name = "MeshAddToSelected";
			this.MeshAddToSelected.Size = new System.Drawing.Size(97, 43);
			this.MeshAddToSelected.Text = "Add To Selected";
			this.MeshAddToSelected.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.MeshAddToSelected.Click += new System.EventHandler(MeshAddToSelected_Click);
			this.MeshRemoveFromAll.Image = GameTuner.Framework.Properties.Resources.imageset_remove_24;
			this.MeshRemoveFromAll.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.MeshRemoveFromAll.Name = "MeshRemoveFromAll";
			this.MeshRemoveFromAll.Size = new System.Drawing.Size(102, 43);
			this.MeshRemoveFromAll.Text = "Remove From All";
			this.MeshRemoveFromAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.MeshRemoveFromAll.Click += new System.EventHandler(MeshRemoveFromAll_Click);
			this.MeshRemoveFromSelected.Image = GameTuner.Framework.Properties.Resources.image_remove_24;
			this.MeshRemoveFromSelected.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.MeshRemoveFromSelected.Name = "MeshRemoveFromSelected";
			this.MeshRemoveFromSelected.Size = new System.Drawing.Size(132, 43);
			this.MeshRemoveFromSelected.Text = "Remove From Selected";
			this.MeshRemoveFromSelected.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.MeshRemoveFromSelected.Click += new System.EventHandler(MeshRemoveFromSelected_Click);
			this.groupBox2.Controls.Add(this.splitContainer2);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(0, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(658, 319);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Model Groups";
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(3, 16);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Panel1.Controls.Add(this.GroupList);
			this.splitContainer2.Panel1.Controls.Add(this.ModelGroupToolstrip);
			this.splitContainer2.Panel2.Controls.Add(this.GroupProperty);
			this.splitContainer2.Size = new System.Drawing.Size(652, 300);
			this.splitContainer2.SplitterDistance = 349;
			this.splitContainer2.TabIndex = 0;
			this.ModelGroupToolstrip.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ModelGroupToolstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.ModelGroupToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.GroupAllOne, this.GroupEachOne, this.GroupAdd, this.GroupDelete, this.AutoSyncGroup });
			this.ModelGroupToolstrip.Location = new System.Drawing.Point(0, 262);
			this.ModelGroupToolstrip.Name = "ModelGroupToolstrip";
			this.ModelGroupToolstrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.ModelGroupToolstrip.Size = new System.Drawing.Size(349, 38);
			this.ModelGroupToolstrip.TabIndex = 1;
			this.ModelGroupToolstrip.Text = "ModelGroupToolstrip";
			this.GroupAllOne.Image = GameTuner.Framework.Properties.Resources.member_16;
			this.GroupAllOne.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.GroupAllOne.Name = "GroupAllOne";
			this.GroupAllOne.Size = new System.Drawing.Size(67, 35);
			this.GroupAllOne.Text = "All-In-One";
			this.GroupAllOne.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.GroupAllOne.ToolTipText = "Removes all currently selected groups, and makes a single group with all scene meshes";
			this.GroupAllOne.Click += new System.EventHandler(GroupAllOne_Click);
			this.GroupEachOne.Image = GameTuner.Framework.Properties.Resources.member_profile_16;
			this.GroupEachOne.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.GroupEachOne.Name = "GroupEachOne";
			this.GroupEachOne.Size = new System.Drawing.Size(78, 35);
			this.GroupEachOne.Text = "Each-In-One";
			this.GroupEachOne.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.GroupEachOne.ToolTipText = "Deletes all currently defined groups and creates a new group for each mesh in the scene";
			this.GroupEachOne.Click += new System.EventHandler(GroupEachOne_Click);
			this.GroupAdd.Image = GameTuner.Framework.Properties.Resources.file_document;
			this.GroupAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.GroupAdd.Name = "GroupAdd";
			this.GroupAdd.Size = new System.Drawing.Size(33, 35);
			this.GroupAdd.Text = "Add";
			this.GroupAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.GroupAdd.ToolTipText = "Adds a new empty model group";
			this.GroupAdd.Click += new System.EventHandler(GroupAdd_Click);
			this.GroupDelete.Image = GameTuner.Framework.Properties.Resources.tool_delete;
			this.GroupDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.GroupDelete.Name = "GroupDelete";
			this.GroupDelete.Size = new System.Drawing.Size(44, 35);
			this.GroupDelete.Text = "Delete";
			this.GroupDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.GroupDelete.ToolTipText = "Deletes the currently selected groups";
			this.GroupDelete.Click += new System.EventHandler(GroupDelete_Click);
			this.AutoSyncGroup.CheckOnClick = true;
			this.AutoSyncGroup.Image = GameTuner.Framework.Properties.Resources.graph_zoom2;
			this.AutoSyncGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.AutoSyncGroup.Name = "AutoSyncGroup";
			this.AutoSyncGroup.Size = new System.Drawing.Size(113, 35);
			this.AutoSyncGroup.Text = "AutoSync Selection";
			this.AutoSyncGroup.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.AutoSyncGroup.ToolTipText = "Automatically synchronizes the scene selection to the current group";
			this.GroupList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3] { this.columnHeader3, this.columnHeader4, this.columnHeader5 });
			this.GroupList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.GroupList.FullRowSelect = true;
			this.GroupList.GridLines = true;
			this.GroupList.HideSelection = false;
			this.GroupList.Location = new System.Drawing.Point(0, 0);
			this.GroupList.Name = "GroupList";
			this.GroupList.Size = new System.Drawing.Size(349, 262);
			this.GroupList.TabIndex = 0;
			this.GroupList.UseCompatibleStateImageBehavior = false;
			this.GroupList.View = System.Windows.Forms.View.Details;
			this.GroupList.SelectedIndexChanged += new System.EventHandler(GroupList_SelectedIndexChanged);
			this.columnHeader3.Text = "#";
			this.columnHeader3.Width = 42;
			this.columnHeader4.Text = "Group Name";
			this.columnHeader4.Width = 156;
			this.columnHeader5.Text = "Mesh Count";
			this.columnHeader5.Width = 130;
			this.GroupProperty.Dock = System.Windows.Forms.DockStyle.Fill;
			this.GroupProperty.Filter = "";
			this.GroupProperty.Location = new System.Drawing.Point(0, 0);
			this.GroupProperty.Name = "GroupProperty";
			this.GroupProperty.ReadOnly = false;
			this.GroupProperty.Size = new System.Drawing.Size(299, 300);
			this.GroupProperty.TabIndex = 0;
			this.GroupProperty.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(GroupProperty_PropertyValueChanged);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(658, 624);
			base.Controls.Add(this.splitContainer1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			base.HelpButton = true;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "GameTunerModelManager";
			this.Text = "GameTuner Model Manager";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.MeshListToolstrip.ResumeLayout(false);
			this.MeshListToolstrip.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.ModelGroupToolstrip.ResumeLayout(false);
			this.ModelGroupToolstrip.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
