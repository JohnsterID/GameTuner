using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using GameTuner.Framework;

namespace GameTuner
{
	public class CustomUI : UserControl
	{
		private delegate void UpdateLuaStatesDelegate(IEnumerable<LuaState> luaStates);

		public class PanelData
		{
			public class ActionData
			{
				public string Name = string.Empty;

				public string Action = string.Empty;

				public Point Location = default(Point);

				public ActionData()
				{
				}

				public ActionData(ActionButton button)
				{
					Name = button.Text;
					Action = button.Tag.ToString();
					Location = button.Location;
				}
			}

			public class ValueControlData
			{
				public string Name = string.Empty;

				public string GetFunction = string.Empty;

				public string SetFunction = string.Empty;

				public Point Location = default(Point);

				public ValueControlData()
				{
				}

				public ValueControlData(ValueControl ctrl)
				{
					Name = ctrl.ControlName;
					GetFunction = ctrl.GetFunction;
					SetFunction = ctrl.SetFunction;
					Location = ctrl.Location;
				}
			}

			public class StringControlData : ValueControlData
			{
				public StringControlData()
				{
				}

				public StringControlData(StringControl ctrl)
					: base(ctrl)
				{
				}
			}

			public class IntegerControlData : ValueControlData
			{
				public int DefaultValue;

				public IntegerControlData()
				{
				}

				public IntegerControlData(IntegerControl ctrl)
					: base(ctrl)
				{
					DefaultValue = ctrl.DefaultValue;
				}
			}

			public class FloatControlData : ValueControlData
			{
				public float DefaultValue;

				public FloatControlData()
				{
				}

				public FloatControlData(FloatControl ctrl)
					: base(ctrl)
				{
					DefaultValue = ctrl.DefaultValue;
				}
			}

			public class BooleanControlData : ValueControlData
			{
				public BooleanControlData()
				{
				}

				public BooleanControlData(BooleanControl ctrl)
					: base(ctrl)
				{
				}
			}

			public class TableViewData
			{
				public string Table = string.Empty;

				public bool Fixed;

				public string OnRefresh = string.Empty;

				public Point Location = default(Point);

				public Size Size = default(Size);

				public TableViewData()
				{
				}

				public TableViewData(TableView ctrl)
				{
					Table = ctrl.Table;
					Fixed = ctrl.Fixed;
					OnRefresh = ctrl.OnRefresh;
					Location = ctrl.Location;
					Size = ctrl.Size;
				}
			}

			public class DataViewData
			{
				public Point Location = default(Point);

				public Size Size = default(Size);

				public DataViewData()
				{
				}

				public DataViewData(DataView ctrl)
				{
					Location = ctrl.Location;
					Size = ctrl.Size;
				}
			}

			public class StatTrackerData
			{
				public Point Location = default(Point);

				public Size Size = default(Size);

				public StatTrackerData()
				{
				}

				public StatTrackerData(StatTracker ctrl)
				{
					Location = ctrl.Location;
					Size = ctrl.Size;
				}
			}

			public class SelectionListData
			{
				public Point Location = default(Point);

				public Size Size = default(Size);

				public string Name = string.Empty;

				public string PopulateList = string.Empty;

				public string OnSelection = string.Empty;

				public bool Sorted;

				public SelectionListData()
				{
				}

				public SelectionListData(SelectionList ctrl)
				{
					Location = ctrl.Location;
					Size = ctrl.Size;
					Name = ctrl.ControlName;
					PopulateList = ctrl.PopulateList;
					OnSelection = ctrl.OnSelection;
					Sorted = ctrl.Sorted;
				}
			}

			public class MultiselectListData
			{
				public Point Location = default(Point);

				public Size Size = default(Size);

				public string Name = string.Empty;

				public string PopulateList = string.Empty;

				public string OnSelected = string.Empty;

				public string OnDeselected = string.Empty;

				public MultiselectListData()
				{
				}

				public MultiselectListData(MultiselectList ctrl)
				{
					Location = ctrl.Location;
					Size = ctrl.Size;
					Name = ctrl.ControlName;
					PopulateList = ctrl.PopulateList;
					OnSelected = ctrl.OnSelected;
					OnDeselected = ctrl.OnDeslected;
				}
			}

			public string Name = string.Empty;

			public string App = string.Empty;

			public string EnterAction = string.Empty;

			public string ExitAction = string.Empty;

			public List<string> CompatibleStates = new List<string>();

			public List<ActionData> Actions = new List<ActionData>();

			public List<StringControlData> StringControls = new List<StringControlData>();

			public List<IntegerControlData> IntegerControls = new List<IntegerControlData>();

			public List<FloatControlData> FloatControls = new List<FloatControlData>();

			public List<BooleanControlData> BooleanControls = new List<BooleanControlData>();

			public List<TableViewData> TableViews = new List<TableViewData>();

			public List<DataViewData> DataViews = new List<DataViewData>();

			public List<StatTrackerData> StatTrackers = new List<StatTrackerData>();

			public List<SelectionListData> SelectionLists = new List<SelectionListData>();

			public List<MultiselectListData> MultiselectLists = new List<MultiselectListData>();

			public PanelData()
			{
			}

			public PanelData(CustomUI ui)
			{
				Name = ui.PanelName;
				App = ui.App;
				CompatibleStates = ui.CompatibleStates;
				EnterAction = ui.EnterAction;
				ExitAction = ui.ExitAction;
				foreach (Control control in ui.Controls)
				{
					if (control is ActionButton)
					{
						Actions.Add(new ActionData((ActionButton)control));
					}
					else if (control is StringControl)
					{
						StringControls.Add(new StringControlData((StringControl)control));
					}
					else if (control is IntegerControl)
					{
						IntegerControls.Add(new IntegerControlData((IntegerControl)control));
					}
					else if (control is FloatControl)
					{
						FloatControls.Add(new FloatControlData((FloatControl)control));
					}
					else if (control is BooleanControl)
					{
						BooleanControls.Add(new BooleanControlData((BooleanControl)control));
					}
					else if (control is TableView)
					{
						TableViews.Add(new TableViewData((TableView)control));
					}
					else if (control is DataView)
					{
						DataViews.Add(new DataViewData((DataView)control));
					}
					else if (control is StatTracker)
					{
						StatTrackers.Add(new StatTrackerData((StatTracker)control));
					}
					else if (control is SelectionList)
					{
						SelectionLists.Add(new SelectionListData((SelectionList)control));
					}
					else if (control is MultiselectList)
					{
						MultiselectLists.Add(new MultiselectListData((MultiselectList)control));
					}
				}
			}
		}

		private string m_sPanelName = string.Empty;

		private string m_sApp;

		private string m_sFile = string.Empty;

		private bool m_bDirty = true;

		private bool m_bDefaultPanel;

		private int m_iEnterActionListener;

		private int m_iExitActionListener;

		private string m_sEnterAction = string.Empty;

		private string m_sExitAction = string.Empty;

		private bool m_bTabSelected;

		private List<string> m_CompatibleStates = new List<string>();

		private bool m_bPendingLuaStateChange;

		private LuaState m_LastSelectedState;

		private ICustomControl m_MovingControl;

		private ICustomControl m_ResizingControl;

		public Point m_ControlMoveOffset = default(Point);

		private IContainer components;

		private ToolStrip ToolStrip;

		private ToolStripDropDownButton toolStripDropDownButton1;

		private ToolStripMenuItem addActionControlToolStripMenuItem;

		private ToolStripComboBox cmbLuaState;

		private ToolStripButton btnSave;

		private ToolStripButton btnClose;

		private ToolStripMenuItem addIntegerControlToolStripMenuItem;

		private ToolStripMenuItem addTableViewToolStripMenuItem;

		private ContextMenuStrip mnuContext;

		private ToolStripMenuItem btnEditPanel;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripMenuItem btnNewActionButton;

		private ToolStripMenuItem btnNewIntegerControl;

		private ToolStripMenuItem btnNewTableView;

		private ToolStripMenuItem addFloatControlToolStripMenuItem;

		private ToolStripMenuItem btnNewFloatControl;

		private ToolStripMenuItem addStringControlToolStripMenuItem;

		private ToolStripMenuItem btnNewStringControl;

		private ToolStripMenuItem addBooleanControlToolStripMenuItem;

		private ToolStripMenuItem btnNewBooleanControl;

		private ToolStripMenuItem btnPasteControl;

		private ToolStripMenuItem addDataViewToolStripMenuItem;

		private ToolStripMenuItem btnNewDataView;

		private ToolStripMenuItem addStatTrackerToolStripMenuItem;

		private ToolStripMenuItem btnNewStatTracker;

		private ToolStripMenuItem btnNewSelectionList;

		private ToolStripMenuItem addSelectionListToolStripMenuItem;

		private ToolStripMenuItem addMultiselectListToolStripMenuItem;

		private ToolStripMenuItem newMultiselectListToolStripMenuItem;

		public string PanelName
		{
			get
			{
				return m_sPanelName;
			}
			set
			{
				m_sPanelName = value;
			}
		}

		public string App
		{
			get
			{
				return m_sApp;
			}
		}

		public string File
		{
			get
			{
				return m_sFile;
			}
		}

		public bool Dirty
		{
			get
			{
				return m_bDirty;
			}
			set
			{
				m_bDirty = value;
			}
		}

		public bool DefaultPanel
		{
			get
			{
				return m_bDefaultPanel;
			}
			set
			{
				m_bDefaultPanel = true;
			}
		}

		public string EnterAction
		{
			get
			{
				return m_sEnterAction;
			}
			set
			{
				m_sEnterAction = value;
			}
		}

		public string ExitAction
		{
			get
			{
				return m_sExitAction;
			}
			set
			{
				m_sExitAction = value;
			}
		}

		public List<string> CompatibleStates
		{
			get
			{
				return m_CompatibleStates;
			}
			set
			{
				m_CompatibleStates = value;
				LuaStatesUpdated(LuaStateManager.Instance.LuaStates);
			}
		}

		public LuaState LuaState
		{
			get
			{
				return (LuaState)cmbLuaState.SelectedItem;
			}
		}

		public ICustomControl MovingControl
		{
			get
			{
				return m_MovingControl;
			}
			set
			{
				m_bDirty = true;
				if (ResizingControl != null)
				{
					ResizingControl = null;
				}
				if (m_MovingControl != null)
				{
					m_MovingControl.EndDrag();
					SnapControlToGrid((Control)m_MovingControl);
				}
				m_MovingControl = value;
				if (value != null && value is Control)
				{
					m_MovingControl.StartDrag();
					Cursor = Cursors.Hand;
					Control control = (Control)m_MovingControl;
					m_ControlMoveOffset = new Point(-control.Width / 2, -control.Height / 2);
					AutoScroll = false;
				}
				else
				{
					Cursor = Cursors.Arrow;
					AutoScroll = true;
				}
			}
		}

		public ICustomControl ResizingControl
		{
			get
			{
				return m_ResizingControl;
			}
			set
			{
				m_bDirty = true;
				if (MovingControl != null)
				{
					MovingControl = null;
				}
				if (m_ResizingControl != null)
				{
					m_ResizingControl.EndDrag();
				}
				m_ResizingControl = value;
				if (value != null && value is Control)
				{
					m_ResizingControl.StartDrag();
					Cursor = Cursors.Hand;
					Control control = (Control)m_MovingControl;
				}
				else
				{
					Cursor = Cursors.Arrow;
				}
			}
		}

		public CustomUI(string sApp)
		{
			InitializeComponent();
			Dock = DockStyle.Fill;
			m_sApp = sApp;
			LuaStateManager.Instance.OnLuaStatesUpdated += CustomUI_OnLuaStatesUpdated;
		}

		public CustomUI(FileStream stream)
		{
			InitializeComponent();
			Dock = DockStyle.Fill;
			m_sApp = GameTuner.ConnectedApp;
			m_sFile = stream.Name;
			LoadPanel(stream);
			LuaStateManager.Instance.OnLuaStatesUpdated += CustomUI_OnLuaStatesUpdated;
			m_iEnterActionListener = Connection.Instance.AddRequestListener(OnEnterActionComplete);
			m_iExitActionListener = Connection.Instance.AddRequestListener(OnExitActionComplete);
		}

		public void Release()
		{
			LuaStateManager.Instance.OnLuaStatesUpdated -= CustomUI_OnLuaStatesUpdated;
			foreach (Control control in base.Controls)
			{
				ICustomControl customControl = control as ICustomControl;
				if (customControl != null)
				{
					customControl.Release();
				}
			}
			base.Controls.Clear();
		}

		protected override bool ProcessTabKey(bool forward)
		{
			return frmMainForm.MainForm.TabKeyHandler(forward);
		}

		public void TabEntered()
		{
			m_bTabSelected = true;
			if (m_bPendingLuaStateChange)
			{
				LuaStatesUpdated(LuaStateManager.Instance.LuaStates);
			}
			FireEnterAction(LuaState);
			foreach (Control control in base.Controls)
			{
				if (control is ICustomControl)
				{
					((ICustomControl)control).TabEntered();
				}
			}
		}

		private void FireEnterAction(LuaState L)
		{
			if (L != null && !string.IsNullOrEmpty(EnterAction))
			{
				Connection.Instance.Request("CMD:" + L.ID + ":" + EnterAction, m_iEnterActionListener);
			}
		}

		public void TabLeft()
		{
			m_bTabSelected = false;
			FireExitAction(LuaState);
			foreach (Control control in base.Controls)
			{
				if (control is ICustomControl)
				{
					((ICustomControl)control).TabLeft();
				}
			}
		}

		private void FireExitAction(LuaState L)
		{
			if (L != null && !string.IsNullOrEmpty(ExitAction))
			{
				Connection.Instance.Request("CMD:" + L.ID + ":" + ExitAction, m_iExitActionListener);
			}
		}

		public void OnEnterActionComplete(List<string> messages)
		{
		}

		public void OnExitActionComplete(List<string> messages)
		{
		}

		private void CustomUI_OnLuaStatesUpdated(object sender, LuaStateManager.UpdatedEventArgs e)
		{
			Invoke(new UpdateLuaStatesDelegate(LuaStatesUpdated), e.LuaStates);
		}

		private void LuaStatesUpdated(IEnumerable<LuaState> luaStates)
		{
			if (!m_bTabSelected)
			{
				m_bPendingLuaStateChange = true;
				return;
			}
			m_bPendingLuaStateChange = false;
			LuaState luaState = LuaState;
			cmbLuaState.Items.Clear();
			if (luaStates != null)
			{
				foreach (LuaState luaState3 in luaStates)
				{
					if (m_CompatibleStates.Contains(luaState3.Name))
					{
						cmbLuaState.Items.Add(luaState3);
					}
				}
			}
			if (m_LastSelectedState != null)
			{
				for (int i = 0; i < cmbLuaState.Items.Count; i++)
				{
					if (cmbLuaState.Items[i] == m_LastSelectedState)
					{
						cmbLuaState.SelectedIndex = i;
						break;
					}
				}
			}
			if (cmbLuaState.SelectedIndex == -1)
			{
				if (cmbLuaState.Items.Count > 0)
				{
					cmbLuaState.SelectedIndex = 0;
				}
				else
				{
					cmbLuaState.Text = "";
				}
			}
			if (luaState == null)
			{
				return;
			}
			foreach (LuaState item in cmbLuaState.Items)
			{
				if (item.Name == luaState.Name)
				{
					cmbLuaState.SelectedItem = item;
					break;
				}
			}
		}

		private void cmbLuaState_SelectedIndexChanged(object sender, EventArgs e)
		{
			LuaState luaState = LuaState;
			if (luaState == m_LastSelectedState)
			{
				return;
			}
			if (m_LastSelectedState != null)
			{
				m_LastSelectedState = LuaStateManager.Instance.GetLuaStateByName(m_LastSelectedState.Name);
			}
			if (luaState == m_LastSelectedState)
			{
				return;
			}
			foreach (Control control in base.Controls)
			{
				if (control is ICustomControl)
				{
					((ICustomControl)control).LuaStateChanged(luaState, m_LastSelectedState);
				}
			}
			if (m_LastSelectedState != null)
			{
				FireExitAction(m_LastSelectedState);
			}
			if (luaState != null && m_bTabSelected)
			{
				FireEnterAction(luaState);
			}
			m_LastSelectedState = luaState;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			Save();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			if (frmMainForm.ClosePanelHandler != null)
			{
				frmMainForm.ClosePanelHandler(this, true);
			}
		}

		public void Save()
		{
			if (m_sFile == string.Empty)
			{
				SaveAs();
				return;
			}
			try
			{
				if (UserInfo.InHouse && System.IO.File.Exists(m_sFile) && new FileInfo(m_sFile).IsReadOnly)
				{
					Process process = new Process();
					process.StartInfo.FileName = "p4";
					process.StartInfo.Arguments = "edit \"" + m_sFile + "\"";
					process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
					process.Start();
					process.WaitForExit();
				}
				Stream stream = new FileStream(m_sFile, FileMode.Create, FileAccess.Write);
				SavePanel(stream);
				stream.Close();
			}
			catch (Exception ex)
			{
				DialogResult dialogResult = MessageBox.Show("Save failed: " + ex.Message + "\nWould you like to try saving in a different location?", "Save Failed", MessageBoxButtons.YesNo);
				if (dialogResult == DialogResult.Yes)
				{
					SaveAs();
				}
			}
		}

		public void SaveAs()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Liver Tuner Panel Files (*.ltp)|*.ltp";
			if (m_sFile != string.Empty)
			{
				saveFileDialog.InitialDirectory = Path.GetDirectoryName(m_sFile);
				saveFileDialog.FileName = Path.GetFileName(m_sFile);
			}
			else
			{
				saveFileDialog.InitialDirectory = GameTuner.DefaultPanelDir;
				saveFileDialog.FileName = PanelName;
			}
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				m_sFile = saveFileDialog.FileName;
				Save();
			}
		}

		private void SavePanel(Stream stream)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(PanelData));
			xmlSerializer.Serialize(stream, new PanelData(this));
			m_bDirty = false;
		}

		private ActionButton AddActionControlFromData(PanelData.ActionData ctrlData)
		{
			ActionButton actionButton = new ActionButton();
			actionButton.MouseUp += Control_Click;
			actionButton.MouseMove += Control_MouseMove;
			actionButton.Text = ctrlData.Name;
			actionButton.Tag = ctrlData.Action;
			actionButton.Location = ctrlData.Location;
			SizeActionControl(actionButton);
			base.Controls.Add(actionButton);
			SnapControlToGrid(actionButton);
			return actionButton;
		}

		private StringControl AddStringControlFromData(PanelData.StringControlData ctrlData)
		{
			StringControl stringControl = new StringControl(Control_Click, Control_MouseMove);
			stringControl.ControlName = ctrlData.Name;
			stringControl.Location = ctrlData.Location;
			base.Controls.Add(stringControl);
			SnapControlToGrid(stringControl);
			stringControl.SetFunction = ctrlData.SetFunction;
			stringControl.GetFunction = ctrlData.GetFunction;
			return stringControl;
		}

		private IntegerControl AddIntegerControlFromData(PanelData.IntegerControlData ctrlData)
		{
			IntegerControl integerControl = new IntegerControl(Control_Click, Control_MouseMove);
			integerControl.ControlName = ctrlData.Name;
			integerControl.DefaultValue = ctrlData.DefaultValue;
			integerControl.Location = ctrlData.Location;
			base.Controls.Add(integerControl);
			SnapControlToGrid(integerControl);
			integerControl.SetFunction = ctrlData.SetFunction;
			integerControl.GetFunction = ctrlData.GetFunction;
			return integerControl;
		}

		private FloatControl AddFloatControlFromData(PanelData.FloatControlData ctrlData)
		{
			FloatControl floatControl = new FloatControl(Control_Click, Control_MouseMove);
			floatControl.ControlName = ctrlData.Name;
			floatControl.DefaultValue = ctrlData.DefaultValue;
			floatControl.Location = ctrlData.Location;
			base.Controls.Add(floatControl);
			SnapControlToGrid(floatControl);
			floatControl.SetFunction = ctrlData.SetFunction;
			floatControl.GetFunction = ctrlData.GetFunction;
			floatControl.GetFunction = ctrlData.GetFunction;
			return floatControl;
		}

		private BooleanControl AddBooleanControlFromData(PanelData.BooleanControlData ctrlData)
		{
			BooleanControl booleanControl = new BooleanControl(Control_Click, Control_MouseMove);
			booleanControl.ControlName = ctrlData.Name;
			booleanControl.Location = ctrlData.Location;
			base.Controls.Add(booleanControl);
			SnapControlToGrid(booleanControl);
			booleanControl.SetFunction = ctrlData.SetFunction;
			booleanControl.GetFunction = ctrlData.GetFunction;
			return booleanControl;
		}

		private TableView AddTableViewFromData(PanelData.TableViewData ctrlData)
		{
			TableView tableView = new TableView(Control_Click, Control_MouseMove);
			tableView.Table = ctrlData.Table;
			tableView.Fixed = ctrlData.Fixed;
			tableView.OnRefresh = ctrlData.OnRefresh;
			tableView.Location = ctrlData.Location;
			tableView.Size = ctrlData.Size;
			base.Controls.Add(tableView);
			SnapControlToGrid(tableView);
			return tableView;
		}

		private DataView AddDataViewFromData(PanelData.DataViewData ctrlData)
		{
			DataView dataView = new DataView(Control_Click, Control_MouseMove);
			dataView.Location = ctrlData.Location;
			dataView.Size = ctrlData.Size;
			base.Controls.Add(dataView);
			return dataView;
		}

		private StatTracker AddStatTrackerFromData(PanelData.StatTrackerData ctrlData)
		{
			StatTracker statTracker = new StatTracker(Control_Click, Control_MouseMove);
			statTracker.Location = ctrlData.Location;
			statTracker.Size = ctrlData.Size;
			base.Controls.Add(statTracker);
			return statTracker;
		}

		private SelectionList AddSelectionListFromData(PanelData.SelectionListData ctrlData)
		{
			SelectionList selectionList = new SelectionList(Control_Click, Control_MouseMove);
			selectionList.Location = ctrlData.Location;
			selectionList.Size = ctrlData.Size;
			selectionList.ControlName = ctrlData.Name;
			selectionList.PopulateList = ctrlData.PopulateList;
			selectionList.OnSelection = ctrlData.OnSelection;
			selectionList.Sorted = ctrlData.Sorted;
			base.Controls.Add(selectionList);
			return selectionList;
		}

		private MultiselectList AddMultiselectListFromData(PanelData.MultiselectListData ctrlData)
		{
			MultiselectList multiselectList = new MultiselectList(Control_Click, Control_MouseMove);
			multiselectList.Location = ctrlData.Location;
			multiselectList.Size = ctrlData.Size;
			multiselectList.ControlName = ctrlData.Name;
			multiselectList.PopulateList = ctrlData.PopulateList;
			multiselectList.OnSelected = ctrlData.OnSelected;
			multiselectList.OnDeslected = ctrlData.OnDeselected;
			base.Controls.Add(multiselectList);
			return multiselectList;
		}

		private void LoadPanel(Stream stream)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(PanelData));
			PanelData panelData = (PanelData)xmlSerializer.Deserialize(stream);
			if (panelData != null)
			{
				PanelName = panelData.Name;
				m_sApp = panelData.App;
				CompatibleStates = panelData.CompatibleStates;
				EnterAction = panelData.EnterAction;
				ExitAction = panelData.ExitAction;
				foreach (PanelData.ActionData action in panelData.Actions)
				{
					AddActionControlFromData(action);
				}
				foreach (PanelData.StringControlData stringControl in panelData.StringControls)
				{
					AddStringControlFromData(stringControl);
				}
				foreach (PanelData.IntegerControlData integerControl in panelData.IntegerControls)
				{
					AddIntegerControlFromData(integerControl);
				}
				foreach (PanelData.FloatControlData floatControl in panelData.FloatControls)
				{
					AddFloatControlFromData(floatControl);
				}
				foreach (PanelData.BooleanControlData booleanControl in panelData.BooleanControls)
				{
					AddBooleanControlFromData(booleanControl);
				}
				foreach (PanelData.TableViewData tableView in panelData.TableViews)
				{
					AddTableViewFromData(tableView);
				}
				foreach (PanelData.DataViewData dataView in panelData.DataViews)
				{
					AddDataViewFromData(dataView);
				}
				foreach (PanelData.StatTrackerData statTracker in panelData.StatTrackers)
				{
					AddStatTrackerFromData(statTracker);
				}
				foreach (PanelData.SelectionListData selectionList in panelData.SelectionLists)
				{
					AddSelectionListFromData(selectionList);
				}
				foreach (PanelData.MultiselectListData multiselectList in panelData.MultiselectLists)
				{
					AddMultiselectListFromData(multiselectList);
				}
			}
			else
			{
				MessageBox.Show("Failed to load " + stream.ToString());
			}
			m_bDirty = false;
		}

		private void AddActionControl_Click(object sender, EventArgs e)
		{
			ActionBuilder actionBuilder = new ActionBuilder();
			if (LuaState != null)
			{
				actionBuilder.LuaStateID = LuaState.ID.ToString();
			}
			if (actionBuilder.ShowDialog() == DialogResult.OK)
			{
				ActionButton actionButton = new ActionButton();
				actionButton.MouseUp += Control_Click;
				actionButton.MouseMove += Control_MouseMove;
				ConfigureActionControl(actionButton, actionBuilder);
				base.Controls.Add(actionButton);
				MovingControl = actionButton;
			}
		}

		private void AddStringControl_Click(object sender, EventArgs e)
		{
			ValueControlBuilder valueControlBuilder = new ValueControlBuilder();
			if (valueControlBuilder.ShowDialog() == DialogResult.OK)
			{
				StringControl stringControl = new StringControl(Control_Click, Control_MouseMove);
				stringControl.ControlName = valueControlBuilder.ControlName;
				base.Controls.Add(stringControl);
				stringControl.GetFunction = valueControlBuilder.GetFunction;
				stringControl.SetFunction = valueControlBuilder.SetFunction;
				MovingControl = stringControl;
			}
		}

		private void AddIntegerControl_Click(object sender, EventArgs e)
		{
			ValueControlBuilder valueControlBuilder = new ValueControlBuilder();
			if (valueControlBuilder.ShowDialog() == DialogResult.OK)
			{
				IntegerControl integerControl = new IntegerControl(Control_Click, Control_MouseMove);
				integerControl.ControlName = valueControlBuilder.ControlName;
				int result = 0;
				int.TryParse(valueControlBuilder.DefaultValue, out result);
				integerControl.DefaultValue = result;
				base.Controls.Add(integerControl);
				integerControl.GetFunction = valueControlBuilder.GetFunction;
				integerControl.SetFunction = valueControlBuilder.SetFunction;
				MovingControl = integerControl;
			}
		}

		private void AddFloatControl_Click(object sender, EventArgs e)
		{
			ValueControlBuilder valueControlBuilder = new ValueControlBuilder();
			if (valueControlBuilder.ShowDialog() == DialogResult.OK)
			{
				FloatControl floatControl = new FloatControl(Control_Click, Control_MouseMove);
				floatControl.ControlName = valueControlBuilder.ControlName;
				float result = 0f;
				float.TryParse(valueControlBuilder.DefaultValue, out result);
				floatControl.DefaultValue = result;
				base.Controls.Add(floatControl);
				floatControl.GetFunction = valueControlBuilder.GetFunction;
				floatControl.SetFunction = valueControlBuilder.SetFunction;
				MovingControl = floatControl;
			}
		}

		private void AddBooleanControl_Click(object sender, EventArgs e)
		{
			ValueControlBuilder valueControlBuilder = new ValueControlBuilder();
			if (valueControlBuilder.ShowDialog() == DialogResult.OK)
			{
				BooleanControl booleanControl = new BooleanControl(Control_Click, Control_MouseMove);
				booleanControl.ControlName = valueControlBuilder.ControlName;
				base.Controls.Add(booleanControl);
				booleanControl.GetFunction = valueControlBuilder.GetFunction;
				booleanControl.SetFunction = valueControlBuilder.SetFunction;
				MovingControl = booleanControl;
			}
		}

		private void AddTableView_Click(object sender, EventArgs e)
		{
			TableView tableView = new TableView(Control_Click, Control_MouseMove);
			base.Controls.Add(tableView);
			MovingControl = tableView;
		}

		private void AddDataView_Click(object sender, EventArgs e)
		{
			DataView dataView = new DataView(Control_Click, Control_MouseMove);
			base.Controls.Add(dataView);
			MovingControl = dataView;
		}

		private void AddStatTracker_Click(object sender, EventArgs e)
		{
			StatTracker statTracker = new StatTracker(Control_Click, Control_MouseMove);
			base.Controls.Add(statTracker);
			MovingControl = statTracker;
		}

		private void AddSelectionList_Click(object sender, EventArgs e)
		{
			SelectionListBuilder selectionListBuilder = new SelectionListBuilder();
			if (selectionListBuilder.ShowDialog() == DialogResult.OK)
			{
				SelectionList selectionList = new SelectionList(Control_Click, Control_MouseMove);
				base.Controls.Add(selectionList);
				selectionList.ControlName = selectionListBuilder.ControlName;
				selectionList.PopulateList = selectionListBuilder.PopulateList;
				selectionList.OnSelection = selectionListBuilder.OnSelection;
				selectionList.Sorted = selectionListBuilder.Sorted;
				selectionList.TabEntered();
				MovingControl = selectionList;
			}
		}

		private void AddMultiselectList_Click(object sender, EventArgs e)
		{
			MultiselectListBuilder multiselectListBuilder = new MultiselectListBuilder();
			if (multiselectListBuilder.ShowDialog() == DialogResult.OK)
			{
				MultiselectList multiselectList = new MultiselectList(Control_Click, Control_MouseMove);
				multiselectList.ControlName = multiselectListBuilder.ControlName;
				multiselectList.PopulateList = multiselectListBuilder.PopulateList;
				multiselectList.OnSelected = multiselectListBuilder.OnSelected;
				multiselectList.OnDeslected = multiselectListBuilder.OnDeslected;
				base.Controls.Add(multiselectList);
				multiselectList.TabEntered();
				MovingControl = multiselectList;
			}
		}

		private void SnapControlToGrid(Control ctrl)
		{
			int num = ctrl.Location.X;
			int num2 = ctrl.Location.Y;
			int num3 = num % 10;
			num = ((num3 >= 5) ? (num + (10 - num3)) : (num - num3));
			int num4 = num % 10;
			num2 = ((num4 >= 5) ? (num2 + (10 - num4)) : (num2 - num4));
			ctrl.Location = new Point(num, num2);
		}

		private void btnEditPanel_Click(object sender, EventArgs e)
		{
			PanelBuilder panelBuilder = new PanelBuilder();
			panelBuilder.PanelName = PanelName;
			panelBuilder.AvailableLuaStates = LuaStateManager.Instance.LuaStateNames;
			panelBuilder.CompatibleStates = CompatibleStates;
			panelBuilder.EnterAction = EnterAction;
			panelBuilder.ExitAction = ExitAction;
			if (panelBuilder.ShowDialog() == DialogResult.OK)
			{
				PanelName = panelBuilder.PanelName;
				CompatibleStates = panelBuilder.CompatibleStates;
				EnterAction = panelBuilder.EnterAction;
				ExitAction = panelBuilder.ExitAction;
				if (base.Parent != null)
				{
					base.Parent.Text = PanelName;
				}
				m_bDirty = true;
			}
		}

		private void btnPasteControl_Click(object sender, EventArgs e)
		{
			try
			{
				StringReader textReader = new StringReader(Clipboard.GetText());
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(GenericObjectSerializer));
				GenericObjectSerializer genericObjectSerializer = xmlSerializer.Deserialize(textReader) as GenericObjectSerializer;
				ICustomControl customControl = null;
				if (genericObjectSerializer.Data is PanelData.ActionData)
				{
					customControl = AddActionControlFromData((PanelData.ActionData)genericObjectSerializer.Data);
				}
				else if (genericObjectSerializer.Data is PanelData.StringControlData)
				{
					customControl = AddStringControlFromData((PanelData.StringControlData)genericObjectSerializer.Data);
				}
				else if (genericObjectSerializer.Data is PanelData.IntegerControlData)
				{
					customControl = AddIntegerControlFromData((PanelData.IntegerControlData)genericObjectSerializer.Data);
				}
				else if (genericObjectSerializer.Data is PanelData.FloatControlData)
				{
					customControl = AddFloatControlFromData((PanelData.FloatControlData)genericObjectSerializer.Data);
				}
				else if (genericObjectSerializer.Data is PanelData.BooleanControlData)
				{
					customControl = AddBooleanControlFromData((PanelData.BooleanControlData)genericObjectSerializer.Data);
				}
				else if (genericObjectSerializer.Data is PanelData.SelectionListData)
				{
					customControl = AddSelectionListFromData((PanelData.SelectionListData)genericObjectSerializer.Data);
					((SelectionList)customControl).TabEntered();
				}
				else if (genericObjectSerializer.Data is PanelData.MultiselectListData)
				{
					customControl = AddMultiselectListFromData((PanelData.MultiselectListData)genericObjectSerializer.Data);
					((MultiselectList)customControl).TabEntered();
				}
				MovingControl = customControl;
			}
			catch
			{
				MessageBox.Show("Clipboard does not contain control data", "Paste Failed");
			}
		}

		private void CustomUI_MouseDown(object sender, MouseEventArgs e)
		{
			if (MovingControl != null)
			{
				MovingControl = null;
			}
			else if (ResizingControl != null)
			{
				ResizingControl = null;
			}
		}

		private void CustomUI_MouseMove(object sender, MouseEventArgs e)
		{
			if (MovingControl != null)
			{
				Control control = (Control)MovingControl;
				control.Location = new Point(e.X + m_ControlMoveOffset.X, e.Y + m_ControlMoveOffset.Y);
			}
			else if (ResizingControl != null)
			{
				Control control2 = (Control)ResizingControl;
				control2.Width = e.X - control2.Left;
				control2.Height = e.Y - control2.Top;
			}
		}

		private void Control_Click(object sender, MouseEventArgs e)
		{
			ICustomControl customControl = null;
			if (sender is ICustomControl)
			{
				customControl = (ICustomControl)sender;
			}
			else if (sender is Control)
			{
				Control control = ((Control)sender).Parent;
				if (control is ICustomControl)
				{
					customControl = (ICustomControl)control;
				}
			}
			if (MovingControl != null)
			{
				MovingControl = null;
			}
			else if (ResizingControl != null)
			{
				ResizingControl = null;
			}
			else if (e.Button == MouseButtons.Left && LuaState != null && customControl is ActionButton)
			{
				ActionButton actionButton = (ActionButton)sender;
				Connection.Instance.Request("CMD:" + LuaState.ID + ":" + actionButton.Tag.ToString(), actionButton.CompletedAction);
			}
		}

		private void Control_MouseMove(object sender, MouseEventArgs e)
		{
			Control control = null;
			if ((control = (Control)sender) != null)
			{
				Point location = control.Location;
				if (control.Parent != this && control.Parent != null)
				{
					location.X += control.Parent.Location.X;
					location.Y += control.Parent.Location.Y;
				}
				if (MovingControl != null)
				{
					Control control2 = (Control)MovingControl;
					control2.Location = new Point(location.X + e.X + m_ControlMoveOffset.X, location.Y + e.Y + m_ControlMoveOffset.Y);
				}
				else if (ResizingControl != null)
				{
					Control control3 = (Control)ResizingControl;
					control3.Width = location.X + e.X - control3.Left;
					control3.Height = location.Y + e.Y - control3.Top;
				}
			}
		}

		public void EditAction(ActionButton control)
		{
			if (control != null)
			{
				ActionBuilder actionBuilder = new ActionBuilder();
				if (LuaState != null)
				{
					actionBuilder.LuaStateID = LuaState.ID.ToString();
				}
				actionBuilder.ActionName = control.Text;
				actionBuilder.ActionCode = control.Tag.ToString();
				if (actionBuilder.ShowDialog() == DialogResult.OK)
				{
					ConfigureActionControl(control, actionBuilder);
					m_bDirty = true;
				}
			}
		}

		public void EditStringControl(StringControl control)
		{
			ValueControlBuilder valueControlBuilder = new ValueControlBuilder();
			valueControlBuilder.ControlName = control.ControlName;
			valueControlBuilder.DefaultValue = "NA";
			valueControlBuilder.GetFunction = control.GetFunction;
			valueControlBuilder.SetFunction = control.SetFunction;
			if (valueControlBuilder.ShowDialog() == DialogResult.OK)
			{
				control.ControlName = valueControlBuilder.ControlName;
				control.GetFunction = valueControlBuilder.GetFunction;
				control.SetFunction = valueControlBuilder.SetFunction;
				m_bDirty = true;
			}
		}

		public void EditIntegerControl(IntegerControl control)
		{
			ValueControlBuilder valueControlBuilder = new ValueControlBuilder();
			valueControlBuilder.ControlName = control.ControlName;
			valueControlBuilder.DefaultValue = control.DefaultValue.ToString();
			valueControlBuilder.GetFunction = control.GetFunction;
			valueControlBuilder.SetFunction = control.SetFunction;
			if (valueControlBuilder.ShowDialog() == DialogResult.OK)
			{
				control.ControlName = valueControlBuilder.ControlName;
				int result = 0;
				int.TryParse(valueControlBuilder.DefaultValue, out result);
				control.DefaultValue = result;
				control.GetFunction = valueControlBuilder.GetFunction;
				control.SetFunction = valueControlBuilder.SetFunction;
				m_bDirty = true;
			}
		}

		public void EditFloatControl(FloatControl control)
		{
			ValueControlBuilder valueControlBuilder = new ValueControlBuilder();
			valueControlBuilder.ControlName = control.ControlName;
			valueControlBuilder.DefaultValue = control.DefaultValue.ToString();
			valueControlBuilder.GetFunction = control.GetFunction;
			valueControlBuilder.SetFunction = control.SetFunction;
			if (valueControlBuilder.ShowDialog() == DialogResult.OK)
			{
				control.ControlName = valueControlBuilder.ControlName;
				float result = 0f;
				float.TryParse(valueControlBuilder.DefaultValue, out result);
				control.DefaultValue = result;
				control.GetFunction = valueControlBuilder.GetFunction;
				control.SetFunction = valueControlBuilder.SetFunction;
				m_bDirty = true;
			}
		}

		public void EditBooleanControl(BooleanControl control)
		{
			ValueControlBuilder valueControlBuilder = new ValueControlBuilder();
			valueControlBuilder.ControlName = control.ControlName;
			valueControlBuilder.DefaultValue = "NA";
			valueControlBuilder.GetFunction = control.GetFunction;
			valueControlBuilder.SetFunction = control.SetFunction;
			if (valueControlBuilder.ShowDialog() == DialogResult.OK)
			{
				control.ControlName = valueControlBuilder.ControlName;
				control.GetFunction = valueControlBuilder.GetFunction;
				control.SetFunction = valueControlBuilder.SetFunction;
				m_bDirty = true;
			}
		}

		public void EditSelectionListControl(SelectionList control)
		{
			SelectionListBuilder selectionListBuilder = new SelectionListBuilder();
			selectionListBuilder.ControlName = control.ControlName;
			selectionListBuilder.PopulateList = control.PopulateList;
			selectionListBuilder.OnSelection = control.OnSelection;
			selectionListBuilder.Sorted = control.Sorted;
			if (selectionListBuilder.ShowDialog() == DialogResult.OK)
			{
				control.ControlName = selectionListBuilder.ControlName;
				control.PopulateList = selectionListBuilder.PopulateList;
				control.OnSelection = selectionListBuilder.OnSelection;
				control.Sorted = selectionListBuilder.Sorted;
				control.TabEntered();
				m_bDirty = true;
			}
		}

		public void EditMultiselectListControl(MultiselectList control)
		{
			MultiselectListBuilder multiselectListBuilder = new MultiselectListBuilder();
			multiselectListBuilder.ControlName = control.ControlName;
			multiselectListBuilder.PopulateList = control.PopulateList;
			multiselectListBuilder.OnSelected = control.OnSelected;
			multiselectListBuilder.OnDeslected = control.OnDeslected;
			if (multiselectListBuilder.ShowDialog() == DialogResult.OK)
			{
				control.ControlName = multiselectListBuilder.ControlName;
				control.PopulateList = multiselectListBuilder.PopulateList;
				control.OnSelected = multiselectListBuilder.OnSelected;
				control.OnDeslected = multiselectListBuilder.OnDeslected;
				control.TabEntered();
				m_bDirty = true;
			}
		}

		public void EditTableView(TableView control)
		{
			TableControlBuilder tableControlBuilder = new TableControlBuilder();
			tableControlBuilder.Table = control.Table;
			tableControlBuilder.Fixed = control.Fixed;
			tableControlBuilder.OnRefresh = control.OnRefresh;
			if (tableControlBuilder.ShowDialog() == DialogResult.OK)
			{
				control.Table = tableControlBuilder.Table;
				control.Fixed = tableControlBuilder.Fixed;
				control.OnRefresh = tableControlBuilder.OnRefresh;
				m_bDirty = true;
			}
		}

		private void ConfigureActionControl(Control control, ActionBuilder builder)
		{
			control.Text = builder.ActionName;
			control.Tag = builder.ActionCode;
			if (builder.BadCode)
			{
				control.BackColor = Color.Red;
			}
			else
			{
				control.BackColor = SystemColors.Control;
			}
			SizeActionControl(control);
		}

		private void SizeActionControl(Control control)
		{
			Graphics graphics = control.CreateGraphics();
			control.Width = Math.Max((int)graphics.MeasureString(control.Text, control.Font).Width + 10, 32);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomUI));
			this.ToolStrip = new System.Windows.Forms.ToolStrip();
			this.cmbLuaState = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			this.addActionControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addStringControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addIntegerControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addFloatControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addBooleanControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addTableViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addDataViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addStatTrackerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addSelectionListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addMultiselectListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.btnClose = new System.Windows.Forms.ToolStripButton();
			this.btnSave = new System.Windows.Forms.ToolStripButton();
			this.mnuContext = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.btnEditPanel = new System.Windows.Forms.ToolStripMenuItem();
			this.btnPasteControl = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.btnNewActionButton = new System.Windows.Forms.ToolStripMenuItem();
			this.btnNewStringControl = new System.Windows.Forms.ToolStripMenuItem();
			this.btnNewIntegerControl = new System.Windows.Forms.ToolStripMenuItem();
			this.btnNewFloatControl = new System.Windows.Forms.ToolStripMenuItem();
			this.btnNewBooleanControl = new System.Windows.Forms.ToolStripMenuItem();
			this.btnNewTableView = new System.Windows.Forms.ToolStripMenuItem();
			this.btnNewDataView = new System.Windows.Forms.ToolStripMenuItem();
			this.btnNewStatTracker = new System.Windows.Forms.ToolStripMenuItem();
			this.btnNewSelectionList = new System.Windows.Forms.ToolStripMenuItem();
			this.newMultiselectListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStrip.SuspendLayout();
			this.mnuContext.SuspendLayout();
			base.SuspendLayout();
			this.ToolStrip.BackgroundImage = (System.Drawing.Image)resources.GetObject("ToolStrip.BackgroundImage");
			this.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.cmbLuaState, this.toolStripDropDownButton1, this.btnClose, this.btnSave });
			this.ToolStrip.Location = new System.Drawing.Point(0, 0);
			this.ToolStrip.Name = "ToolStrip";
			this.ToolStrip.Size = new System.Drawing.Size(593, 25);
			this.ToolStrip.TabIndex = 0;
			this.ToolStrip.Text = "Tools";
			this.cmbLuaState.Name = "cmbLuaState";
			this.cmbLuaState.Size = new System.Drawing.Size(121, 25);
			this.cmbLuaState.Sorted = true;
			this.cmbLuaState.SelectedIndexChanged += new System.EventHandler(cmbLuaState_SelectedIndexChanged);
			this.toolStripDropDownButton1.BackColor = System.Drawing.Color.White;
			this.toolStripDropDownButton1.BackgroundImage = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton1.BackgroundImage");
			this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[10] { this.addActionControlToolStripMenuItem, this.addStringControlToolStripMenuItem, this.addIntegerControlToolStripMenuItem, this.addFloatControlToolStripMenuItem, this.addBooleanControlToolStripMenuItem, this.addTableViewToolStripMenuItem, this.addDataViewToolStripMenuItem, this.addStatTrackerToolStripMenuItem, this.addSelectionListToolStripMenuItem, this.addMultiselectListToolStripMenuItem });
			this.toolStripDropDownButton1.ForeColor = System.Drawing.Color.Black;
			this.toolStripDropDownButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton1.Image");
			this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
			this.toolStripDropDownButton1.Size = new System.Drawing.Size(103, 22);
			this.toolStripDropDownButton1.Text = "New Control";
			this.addActionControlToolStripMenuItem.Name = "addActionControlToolStripMenuItem";
			this.addActionControlToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.addActionControlToolStripMenuItem.Text = "Action Control";
			this.addActionControlToolStripMenuItem.Click += new System.EventHandler(AddActionControl_Click);
			this.addStringControlToolStripMenuItem.Name = "addStringControlToolStripMenuItem";
			this.addStringControlToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.addStringControlToolStripMenuItem.Text = "String Control";
			this.addStringControlToolStripMenuItem.Click += new System.EventHandler(AddStringControl_Click);
			this.addIntegerControlToolStripMenuItem.Name = "addIntegerControlToolStripMenuItem";
			this.addIntegerControlToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.addIntegerControlToolStripMenuItem.Text = "Integer Control";
			this.addIntegerControlToolStripMenuItem.Click += new System.EventHandler(AddIntegerControl_Click);
			this.addFloatControlToolStripMenuItem.Name = "addFloatControlToolStripMenuItem";
			this.addFloatControlToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.addFloatControlToolStripMenuItem.Text = "Float Control";
			this.addFloatControlToolStripMenuItem.Click += new System.EventHandler(AddFloatControl_Click);
			this.addBooleanControlToolStripMenuItem.Name = "addBooleanControlToolStripMenuItem";
			this.addBooleanControlToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.addBooleanControlToolStripMenuItem.Text = "Boolean Control";
			this.addBooleanControlToolStripMenuItem.Click += new System.EventHandler(AddBooleanControl_Click);
			this.addTableViewToolStripMenuItem.Name = "addTableViewToolStripMenuItem";
			this.addTableViewToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.addTableViewToolStripMenuItem.Text = "Table View";
			this.addTableViewToolStripMenuItem.Click += new System.EventHandler(AddTableView_Click);
			this.addDataViewToolStripMenuItem.Name = "addDataViewToolStripMenuItem";
			this.addDataViewToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.addDataViewToolStripMenuItem.Text = "Data View";
			this.addDataViewToolStripMenuItem.Click += new System.EventHandler(AddDataView_Click);
			this.addStatTrackerToolStripMenuItem.Name = "addStatTrackerToolStripMenuItem";
			this.addStatTrackerToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.addStatTrackerToolStripMenuItem.Text = "Stat Tracker";
			this.addStatTrackerToolStripMenuItem.Click += new System.EventHandler(AddStatTracker_Click);
			this.addSelectionListToolStripMenuItem.Name = "addSelectionListToolStripMenuItem";
			this.addSelectionListToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.addSelectionListToolStripMenuItem.Text = "Selection List";
			this.addSelectionListToolStripMenuItem.Click += new System.EventHandler(AddSelectionList_Click);
			this.addMultiselectListToolStripMenuItem.Name = "addMultiselectListToolStripMenuItem";
			this.addMultiselectListToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.addMultiselectListToolStripMenuItem.Text = "Multiselect List";
			this.addMultiselectListToolStripMenuItem.Click += new System.EventHandler(AddMultiselectList_Click);
			this.btnClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.btnClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnClose.Image = (System.Drawing.Image)resources.GetObject("btnClose.Image");
			this.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(23, 22);
			this.btnClose.ToolTipText = "Close Panel";
			this.btnClose.Click += new System.EventHandler(btnClose_Click);
			this.btnSave.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnSave.Image = (System.Drawing.Image)resources.GetObject("btnSave.Image");
			this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(23, 22);
			this.btnSave.ToolTipText = "Save Panel";
			this.btnSave.Click += new System.EventHandler(btnSave_Click);
			this.mnuContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[13]
			{
				this.btnEditPanel, this.btnPasteControl, this.toolStripSeparator1, this.btnNewActionButton, this.btnNewStringControl, this.btnNewIntegerControl, this.btnNewFloatControl, this.btnNewBooleanControl, this.btnNewTableView, this.btnNewDataView,
				this.btnNewStatTracker, this.btnNewSelectionList, this.newMultiselectListToolStripMenuItem
			});
			this.mnuContext.Name = "mnuContext";
			this.mnuContext.Size = new System.Drawing.Size(188, 274);
			this.btnEditPanel.Name = "btnEditPanel";
			this.btnEditPanel.Size = new System.Drawing.Size(187, 22);
			this.btnEditPanel.Text = "Edit Panel";
			this.btnEditPanel.Click += new System.EventHandler(btnEditPanel_Click);
			this.btnPasteControl.Name = "btnPasteControl";
			this.btnPasteControl.Size = new System.Drawing.Size(187, 22);
			this.btnPasteControl.Text = "Paste Control";
			this.btnPasteControl.Click += new System.EventHandler(btnPasteControl_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(184, 6);
			this.btnNewActionButton.Name = "btnNewActionButton";
			this.btnNewActionButton.Size = new System.Drawing.Size(187, 22);
			this.btnNewActionButton.Text = "New Action Control";
			this.btnNewActionButton.Click += new System.EventHandler(AddActionControl_Click);
			this.btnNewStringControl.Name = "btnNewStringControl";
			this.btnNewStringControl.Size = new System.Drawing.Size(187, 22);
			this.btnNewStringControl.Text = "New String Control";
			this.btnNewStringControl.Click += new System.EventHandler(AddStringControl_Click);
			this.btnNewIntegerControl.Name = "btnNewIntegerControl";
			this.btnNewIntegerControl.Size = new System.Drawing.Size(187, 22);
			this.btnNewIntegerControl.Text = "New Integer Control";
			this.btnNewIntegerControl.Click += new System.EventHandler(AddIntegerControl_Click);
			this.btnNewFloatControl.Name = "btnNewFloatControl";
			this.btnNewFloatControl.Size = new System.Drawing.Size(187, 22);
			this.btnNewFloatControl.Text = "New Float Control";
			this.btnNewFloatControl.Click += new System.EventHandler(AddFloatControl_Click);
			this.btnNewBooleanControl.Name = "btnNewBooleanControl";
			this.btnNewBooleanControl.Size = new System.Drawing.Size(187, 22);
			this.btnNewBooleanControl.Text = "New Boolean Control";
			this.btnNewBooleanControl.Click += new System.EventHandler(AddBooleanControl_Click);
			this.btnNewTableView.Name = "btnNewTableView";
			this.btnNewTableView.Size = new System.Drawing.Size(187, 22);
			this.btnNewTableView.Text = "New Table View";
			this.btnNewTableView.Click += new System.EventHandler(AddTableView_Click);
			this.btnNewDataView.Name = "btnNewDataView";
			this.btnNewDataView.Size = new System.Drawing.Size(187, 22);
			this.btnNewDataView.Text = "New Data View";
			this.btnNewDataView.Click += new System.EventHandler(AddDataView_Click);
			this.btnNewStatTracker.Name = "btnNewStatTracker";
			this.btnNewStatTracker.Size = new System.Drawing.Size(187, 22);
			this.btnNewStatTracker.Text = "New Stat Tracker";
			this.btnNewStatTracker.Click += new System.EventHandler(AddStatTracker_Click);
			this.btnNewSelectionList.Name = "btnNewSelectionList";
			this.btnNewSelectionList.Size = new System.Drawing.Size(187, 22);
			this.btnNewSelectionList.Text = "New Selection List";
			this.btnNewSelectionList.Click += new System.EventHandler(AddSelectionList_Click);
			this.newMultiselectListToolStripMenuItem.Name = "newMultiselectListToolStripMenuItem";
			this.newMultiselectListToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
			this.newMultiselectListToolStripMenuItem.Text = "New Multiselect List";
			this.newMultiselectListToolStripMenuItem.Click += new System.EventHandler(AddMultiselectList_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.BackColor = System.Drawing.Color.Black;
			this.ContextMenuStrip = this.mnuContext;
			base.Controls.Add(this.ToolStrip);
			this.ForeColor = System.Drawing.Color.White;
			base.Name = "CustomUI";
			base.Size = new System.Drawing.Size(593, 459);
			base.MouseMove += new System.Windows.Forms.MouseEventHandler(CustomUI_MouseMove);
			base.MouseDown += new System.Windows.Forms.MouseEventHandler(CustomUI_MouseDown);
			this.ToolStrip.ResumeLayout(false);
			this.ToolStrip.PerformLayout();
			this.mnuContext.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
