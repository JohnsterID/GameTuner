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
	public class frmMainForm : Form
	{
		private delegate void UpdateLuaStatesDelegate(IEnumerable<LuaState> luaStates);

		public delegate void ClosePanelDelegate(CustomUI panel, bool bCancelButton);

		private delegate void UpdateConnectedAppInfoHandler();

		public class PanelInfo
		{
			public bool LockLuaState;

			public string ConsoleLuaState = string.Empty;

			public int SelectedPanel;

			public List<string> OpenPanels = new List<string>();
		}

		public class DictionarySerializer<TKey, TValue>
		{
			public class Entry
			{
				public TKey Key;

				public TValue Value;

				public Entry()
				{
					Key = default(TKey);
					Value = default(TValue);
				}

				public Entry(TKey key, TValue value)
				{
					Key = key;
					Value = value;
				}
			}

			public List<Entry> SerializationMember = new List<Entry>();

			[XmlIgnore]
			public Dictionary<TKey, TValue> Dictionary
			{
				get
				{
					Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
					foreach (Entry item in SerializationMember)
					{
						dictionary[item.Key] = item.Value;
					}
					return dictionary;
				}
				set
				{
					foreach (TKey key in value.Keys)
					{
						SerializationMember.Add(new Entry(key, value[key]));
					}
				}
			}
		}

		public static frmMainForm MainForm;

		private WindowMessager FormMessager;

		private List<Control> m_ControlsAcceptingTab = new List<Control>();

		private static ClosePanelDelegate m_ClosePanelHandler;

		private TabPage m_CurrentTab;

		private bool m_bWasConnected;

		private Dictionary<string, PanelInfo> PanelConfig = new Dictionary<string, PanelInfo>();

		private IContainer components;

		private StatusStrip ctrlStatusStrip;

		private ToolStripStatusLabel lblConnectionStatus;

		private ToolStripMenuItem FileMenu;

		private ToolStripMenuItem btnNewPanel;

		private ToolStripMenuItem ConnectionMenu;

		private MenuStrip MainMenu;

		private TabPage tabLuaConsole;

		private TabControl ctrlMainFormTabs;

		private TabPage tabCreator;

		private ToolStripMenuItem btnOpenPanel;

		private ToolStripMenuItem btnSavePanel;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripMenuItem btnSavePanelAs;

		private ToolStripMenuItem btnRefreshLuaStates;

		private ToolStripMenuItem btnForceDisconnect;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripMenuItem btnExit;

		private ToolStripMenuItem btnChangeConnection;

		private ToolStripMenuItem adminToolStripMenuItem;

		private ToolStripMenuItem btnEditProjectPanels;

		private ToolStripMenuItem helpToolStripMenuItem;

		private ToolStripMenuItem aboutGameTunerToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator3;

		private LuaConsole LuaConsole;

		private Timer tmrUpdate;

		private Timer tmrProcessMessages;

		public static ClosePanelDelegate ClosePanelHandler
		{
			get
			{
				return m_ClosePanelHandler;
			}
		}

		public frmMainForm()
		{
			InitializeComponent();
			MainForm = this;
		}

		private void frmMainForm_Load(object sender, EventArgs e)
		{
			FormMessager = new WindowMessager(base.Handle);
			FormMessager.MessageRecieved += OnWindowMessage;
			IntPtr hWindow = NativeMethods.FindWindow(IntPtr.Zero, "GameTuner - Hidden Window");
			FormMessager.SendMessage(hWindow, "GameTuner");
		}

		private void OnWindowMessage(object sender, WindowMessageRecievedArgs args)
		{
			if (ApplicationHelper.ProductVersion != args.Message)
			{
				MessageBox.Show("You are not running the latest stable version of the Tuner.", "Warning");
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (FormMessager == null || !FormMessager.HandleWindowMessage(ref m))
			{
				base.WndProc(ref m);
			}
		}

		private void frmMainForm_Shown(object sender, EventArgs e)
		{
			Connection.Init(this);
			LuaStateManager.Init();
			LuaConsole.Init();
			LuaStateManager.Instance.OnLuaStatesUpdated += frmMainForm_OnLuaStatesUpdated;
			m_ClosePanelHandler = ClosePanel;
			GreetUser();
			LoadPanelConfig();
			Connection.Instance.OpenConnection();
			tmrProcessMessages.Enabled = true;
			tmrUpdate.Enabled = true;
		}

		private void frmMainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			LuaStateManager.Instance.OnLuaStatesUpdated -= frmMainForm_OnLuaStatesUpdated;
			foreach (TabPage tabPage in ctrlMainFormTabs.TabPages)
			{
				if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is CustomUI)
				{
					CustomUI customUI = (CustomUI)tabPage.Controls[0];
					if (customUI.Dirty)
					{
						SavePrompt(customUI, false);
					}
					customUI.TabLeft();
				}
			}
			SavePanelConfig();
			Connection.Instance.InvokeTarget = null;
		}

		private void GreetUser()
		{
			DateTime now = DateTime.Now;
			string text;
			if (!UserInfo.InHouse)
			{
				text = ((now.Hour < 5) ? "Hang in there!" : ((now.Hour < 11) ? "Good Morning" : ((now.Hour < 13) ? "Hello Master" : ((now.Hour < 15) ? "Good Afternoon" : ((now.Hour < 18) ? "Hi!  Nice to see you again." : ((now.Hour >= 20) ? "Working late?" : "Good Evening"))))));
			}
			else
			{
				string firstName = UserInfo.GetCurrent().FirstName;
				text = ((now.Hour < 5) ? "Hang in there!" : ((now.Hour < 11) ? "Good Morning" : ((now.Hour < 13) ? "Hello Master" : ((now.Hour < 15) ? "Let's play Global Thermonuclear War" : ((now.Hour < 18) ? ("I'm sorry, " + firstName + ". I'm afraid I can't do that.") : ((now.Hour < 20) ? (firstName + ", my mind is going. I can feel it. I can feel it. My mind is going.") : ((now.Hour >= 21) ? "Working late?" : "Daisy, Daisy, give me your answer do.")))))));
			}
			LuaConsole.PrintToConsoleOutput(text + "\n", FontStyle.Bold);
		}

		private void aboutGameTunerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutBox aboutBox = new AboutBox();
			aboutBox.ShowDialog(this);
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void frmMainForm_Move(object sender, EventArgs e)
		{
			LuaConsole.ForceCloseCodeBuddy();
		}

		private void frmMainForm_Resize(object sender, EventArgs e)
		{
			LuaConsole.ForceCloseCodeBuddy();
		}

		protected override bool ProcessTabKey(bool forward)
		{
			return TabKeyHandler(forward);
		}

		public void ControlAcceptsTab(Control ctrl)
		{
			m_ControlsAcceptingTab.Add(ctrl);
		}

		public bool TabKeyHandler(bool bForward)
		{
			if (LuaConsole.ConsoleInputFocused)
			{
				return false;
			}
			foreach (Control item in m_ControlsAcceptingTab)
			{
				if (item.Focused)
				{
					return false;
				}
			}
			return base.ProcessTabKey(bForward);
		}

		private void btnRefreshLuaStates_Click(object sender, EventArgs e)
		{
			LuaStateManager.Instance.QueryLuaStates();
		}

		private void frmMainForm_OnLuaStatesUpdated(object sender, LuaStateManager.UpdatedEventArgs e)
		{
			Invoke(new UpdateLuaStatesDelegate(LuaConsole.UpdateLuaStates), e.LuaStates);
		}

		private void btnNewPanel_Click(object sender, EventArgs e)
		{
			CreateNewPanel();
		}

		private void btnOpenPanel_Click(object sender, EventArgs e)
		{
			OpenPanel();
		}

		private void btnSavePanel_Click(object sender, EventArgs e)
		{
			TabPage selectedTab = ctrlMainFormTabs.SelectedTab;
			if (selectedTab != null && selectedTab.Controls.Count > 0 && selectedTab.Controls[0] is CustomUI)
			{
				CustomUI customUI = (CustomUI)selectedTab.Controls[0];
				customUI.Save();
			}
		}

		private void btnSavePanelAs_Click(object sender, EventArgs e)
		{
			TabPage selectedTab = ctrlMainFormTabs.SelectedTab;
			if (selectedTab != null && selectedTab.Controls.Count > 0 && selectedTab.Controls[0] is CustomUI)
			{
				CustomUI customUI = (CustomUI)selectedTab.Controls[0];
				customUI.SaveAs();
			}
		}

		private void ctrlMainFormTabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ctrlMainFormTabs.SelectedTab == tabCreator)
			{
				ctrlMainFormTabs.SelectedTab = tabLuaConsole;
				CreateNewPanel();
			}
		}

		private void btnEditProjectPanels_Click(object sender, EventArgs e)
		{
			if (!GameTuner.CheckForAdminPrivileges())
			{
				return;
			}
			List<string> list = new List<string>();
			DirectoryInfo directoryInfo = new DirectoryInfo(GameTuner.DefaultPanelDir);
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				if (fileInfo.Extension == ".ltp")
				{
					list.Add(fileInfo.Name);
				}
			}
			ProjectPanelsManager projectPanelsManager = new ProjectPanelsManager();
			projectPanelsManager.AvailablePanels = list;
			projectPanelsManager.ProjectPanels = GetDefaultPanels();
			if (projectPanelsManager.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			List<string> projectPanels = projectPanelsManager.ProjectPanels;
			SaveDefaultPanels(projectPanelsManager.ProjectPanels);
			List<string> list2 = new List<string>();
			foreach (TabPage tabPage in ctrlMainFormTabs.TabPages)
			{
				if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is CustomUI)
				{
					CustomUI customUI = (CustomUI)tabPage.Controls[0];
					string fileName = Path.GetFileName(customUI.File);
					list2.Add(fileName);
					if (projectPanels.Contains(fileName))
					{
						customUI.DefaultPanel = true;
					}
					else
					{
						customUI.DefaultPanel = false;
					}
				}
			}
			foreach (string item in projectPanels)
			{
				if (!list2.Contains(item))
				{
					CustomUI customUI2 = OpenPanel(GameTuner.DefaultPanelDir + "\\" + item);
					if (customUI2 != null)
					{
						customUI2.DefaultPanel = true;
					}
				}
			}
		}

		private void CreateNewPanel()
		{
			PanelBuilder panelBuilder = new PanelBuilder();
			panelBuilder.AvailableLuaStates = LuaStateManager.Instance.LuaStateNames;
			if (panelBuilder.ShowDialog() == DialogResult.OK)
			{
				CustomUI customUI = new CustomUI(GameTuner.ConnectedApp);
				customUI.PanelName = panelBuilder.PanelName;
				customUI.CompatibleStates = panelBuilder.CompatibleStates;
				customUI.EnterAction = panelBuilder.EnterAction;
				customUI.ExitAction = panelBuilder.ExitAction;
				TabPage tabPage = new TabPage();
				tabPage.Text = customUI.PanelName;
				tabPage.Controls.Add(customUI);
				ctrlMainFormTabs.TabPages.Insert(ctrlMainFormTabs.TabPages.Count - 1, tabPage);
				ctrlMainFormTabs.SelectedTab = tabPage;
			}
		}

		private void OpenPanel()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = true;
			openFileDialog.InitialDirectory = GameTuner.DefaultPanelDir;
			openFileDialog.Filter = "Liver Tuner Panel Files (*.ltp)|*.ltp";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string[] fileNames = openFileDialog.FileNames;
				foreach (string sFilename in fileNames)
				{
					OpenPanel(sFilename);
				}
			}
		}

		private CustomUI OpenPanel(string sFilename)
		{
			try
			{
				FileStream fileStream = new FileStream(sFilename, FileMode.Open, FileAccess.Read);
				CustomUI customUI = new CustomUI(fileStream);
				TabPage tabPage = new TabPage();
				tabPage.Text = customUI.PanelName;
				tabPage.Controls.Add(customUI);
				ctrlMainFormTabs.TabPages.Insert(ctrlMainFormTabs.TabPages.Count - 1, tabPage);
				ctrlMainFormTabs.SelectedTab = tabPage;
				fileStream.Close();
				return customUI;
			}
			catch (Exception ex)
			{
				try
				{
					ErrorHandling.Error(ex, "Failed to load panel \"" + sFilename + "\":\n" + ex.Message, ErrorLevel.ShowMessage);
				}
				catch
				{
				}
				return null;
			}
		}

		private void ClosePanel(CustomUI panel, bool bCancelButton)
		{
			foreach (TabPage tabPage in ctrlMainFormTabs.TabPages)
			{
				if (tabPage.Controls.Count > 0 && tabPage.Controls[0] == panel)
				{
					bool flag = false;
					if (panel.Dirty)
					{
						flag = !SavePrompt(panel, bCancelButton);
					}
					if (!flag)
					{
						panel.Release();
						ctrlMainFormTabs.TabPages.Remove(tabPage);
					}
				}
			}
		}

		private bool SavePrompt(CustomUI panel, bool bCancelButton)
		{
			string text = "Would you like to save your changes to \"" + panel.PanelName + "\"?";
			DialogResult dialogResult = ((!bCancelButton) ? MessageBox.Show(text, "Closing Panel", MessageBoxButtons.YesNo, MessageBoxIcon.None) : MessageBox.Show(text, "Closing Panel", MessageBoxButtons.YesNoCancel, MessageBoxIcon.None));
			if (dialogResult == DialogResult.Yes)
			{
				panel.Save();
			}
			return dialogResult != DialogResult.Cancel;
		}

		private void ctrlMainFormTabs_TabIndexChanged(object sender, EventArgs e)
		{
		}

		private void ctrlMainFormTabs_Selected(object sender, TabControlEventArgs e)
		{
			if (m_CurrentTab != ctrlMainFormTabs.SelectedTab)
			{
				if (m_CurrentTab != null && m_CurrentTab.Controls.Count > 0 && m_CurrentTab.Controls[0] is CustomUI)
				{
					((CustomUI)m_CurrentTab.Controls[0]).TabLeft();
				}
				m_CurrentTab = ctrlMainFormTabs.SelectedTab;
				if (m_CurrentTab != null && m_CurrentTab.Controls.Count > 0 && m_CurrentTab.Controls[0] is CustomUI)
				{
					((CustomUI)m_CurrentTab.Controls[0]).TabEntered();
				}
			}
		}

		private void CloseAllCustomUIs()
		{
			List<TabPage> list = new List<TabPage>();
			foreach (TabPage tabPage in ctrlMainFormTabs.TabPages)
			{
				if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is CustomUI)
				{
					CustomUI customUI = (CustomUI)tabPage.Controls[0];
					if (customUI.Dirty)
					{
						SavePrompt(customUI, false);
					}
					customUI.Release();
					list.Add(tabPage);
				}
			}
			foreach (TabPage item in list)
			{
				ctrlMainFormTabs.TabPages.Remove(item);
			}
			GC.Collect();
		}

		private void OnConnectionEstablished()
		{
			UpdateConnectedAppInfo();
			LuaStateManager.Instance.QueryLuaStates();
		}

		private void tmrUpdate_Tick(object sender, EventArgs e)
		{
			UpdateConnectionStatus();
		}

		private void tmrProcessMessages_Tick(object sender, EventArgs e)
		{
			Connection.Instance.RouteMessages();
			LuaConsole.FlushOutputMessages();
		}

		private void UpdateConnectionStatus()
		{
			tmrUpdate.Enabled = false;
			if (Connection.Instance.Connected)
			{
				lblConnectionStatus.Text = "Connected";
				if (!m_bWasConnected)
				{
					m_bWasConnected = true;
					OnConnectionEstablished();
				}
			}
			else
			{
				lblConnectionStatus.Text = "Disconnected";
				if (m_bWasConnected)
				{
					m_bWasConnected = false;
					ClearAppInfo();
					LuaStateManager.Instance.OnDisconnected();
				}
				if (!Connection.Instance.Connecting)
				{
					Connection.Instance.OpenConnection();
				}
			}
			tmrUpdate.Enabled = true;
		}

		private void btnChangeConnection_Click(object sender, EventArgs e)
		{
			Connection.Instance.ConnectionPrompt();
		}

		private void btnForceDisconnect_Click(object sender, EventArgs e)
		{
			Connection.Instance.CloseConnection();
		}

		private void UpdateConnectedAppInfo()
		{
			Connection.Instance.Request("APP:", OnAppInfoUpdate);
		}

		private void OnAppInfoUpdate(List<string> appInfoStrings)
		{
			if (GameTuner.ConnectedApp != string.Empty)
			{
				SavePanelConfig();
			}
			if (appInfoStrings.Count < 3)
			{
				return;
			}
			GameTuner.ConnectedApp = appInfoStrings[0];
			GameTuner.DefaultPanelDir = appInfoStrings[2];
			Text = "GameTuner: " + appInfoStrings[1];
			CloseAllCustomUIs();
			List<string> defaultPanels = GetDefaultPanels();
			if (defaultPanels != null)
			{
				foreach (string item in defaultPanels)
				{
					CustomUI customUI = OpenPanel(GameTuner.DefaultPanelDir + "\\" + item);
					if (customUI != null)
					{
						customUI.DefaultPanel = true;
					}
				}
			}
			PanelInfo value = null;
			if (PanelConfig.TryGetValue(GameTuner.ConnectedApp, out value))
			{
				LuaConsole.LockLuaState = value.LockLuaState;
				LuaConsole.DefaultLuaState = value.ConsoleLuaState;
				foreach (string openPanel in value.OpenPanels)
				{
					OpenPanel(openPanel);
				}
				try
				{
					int selectedPanel = value.SelectedPanel;
					if (selectedPanel < ctrlMainFormTabs.TabPages.Count && ctrlMainFormTabs.TabPages[selectedPanel] != tabCreator)
					{
						ctrlMainFormTabs.SelectedIndex = selectedPanel;
					}
				}
				catch
				{
				}
			}
			GameTuner.Admins.Clear();
			for (int i = 3; i < appInfoStrings.Count; i++)
			{
				GameTuner.Admins.Add(appInfoStrings[i]);
			}
		}

		private void ClearAppInfo()
		{
			if (GameTuner.ConnectedApp != string.Empty)
			{
				SavePanelConfig();
			}
			GameTuner.ConnectedApp = string.Empty;
			GameTuner.DefaultPanelDir = Directory.GetCurrentDirectory();
			Text = "GameTuner";
			CloseAllCustomUIs();
			GameTuner.Admins.Clear();
		}

		private void SavePanelConfig()
		{
			try
			{
				PanelInfo panelInfo = new PanelInfo();
				panelInfo.LockLuaState = LuaConsole.LockLuaState;
				panelInfo.ConsoleLuaState = LuaConsole.DefaultLuaState;
				panelInfo.SelectedPanel = ctrlMainFormTabs.SelectedIndex;
				foreach (TabPage tabPage in ctrlMainFormTabs.TabPages)
				{
					if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is CustomUI)
					{
						CustomUI customUI = (CustomUI)tabPage.Controls[0];
						if (!customUI.DefaultPanel && customUI.File != string.Empty)
						{
							panelInfo.OpenPanels.Add(customUI.File);
						}
					}
				}
				PanelConfig[GameTuner.ConnectedApp] = panelInfo;
				DictionarySerializer<string, PanelInfo> dictionarySerializer = new DictionarySerializer<string, PanelInfo>();
				dictionarySerializer.Dictionary = PanelConfig;
				Stream stream = new FileStream(GameTuner.LocalSettingsFolder + "\\PanelConfig.xml", FileMode.Create, FileAccess.Write);
				XmlSerializer xmlSerializer = new XmlSerializer(dictionarySerializer.GetType());
				xmlSerializer.Serialize(stream, dictionarySerializer);
				stream.Close();
			}
			catch (Exception ex)
			{
				ErrorHandling.Error(ex, "Error saving panel config:\n" + ex.Message, ErrorLevel.ShowMessage);
			}
		}

		private void LoadPanelConfig()
		{
			try
			{
				string path = GameTuner.LocalSettingsFolder + "\\PanelConfig.xml";
				if (File.Exists(path))
				{
					Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(DictionarySerializer<string, PanelInfo>));
					DictionarySerializer<string, PanelInfo> dictionarySerializer = (DictionarySerializer<string, PanelInfo>)xmlSerializer.Deserialize(stream);
					stream.Close();
					PanelConfig = dictionarySerializer.Dictionary;
				}
			}
			catch (Exception ex)
			{
				ErrorHandling.Error(ex, "Error loading panel config:\n" + ex.Message, ErrorLevel.ShowMessage);
			}
		}

		private List<string> GetDefaultPanels()
		{
			try
			{
				string path = GameTuner.DefaultPanelDir + "\\DefaultPanels.xml";
				if (File.Exists(path))
				{
					Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<string>));
					List<string> result = (List<string>)xmlSerializer.Deserialize(stream);
					stream.Close();
					return result;
				}
			}
			catch (Exception ex)
			{
				ErrorHandling.Error(ex, "Error loading default panel config:\n" + ex.Message, ErrorLevel.ShowMessage);
			}
			return null;
		}

		private void SaveDefaultPanels(List<string> panels)
		{
			try
			{
				string text = GameTuner.DefaultPanelDir + "\\DefaultPanels.xml";
				if (File.Exists(text) && new FileInfo(text).IsReadOnly)
				{
					Process process = new Process();
					process.StartInfo.FileName = "p4";
					process.StartInfo.Arguments = "edit \"" + text + "\"";
					process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
					process.Start();
					process.WaitForExit();
				}
				Stream stream = new FileStream(text, FileMode.Create, FileAccess.Write);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<string>));
				xmlSerializer.Serialize(stream, panels);
				stream.Close();
			}
			catch (Exception ex)
			{
				ErrorHandling.Error(ex, "Error saving default panel config:\n" + ex.Message, ErrorLevel.ShowMessage);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMainForm));
			this.ctrlStatusStrip = new System.Windows.Forms.StatusStrip();
			this.lblConnectionStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.btnNewPanel = new System.Windows.Forms.ToolStripMenuItem();
			this.btnOpenPanel = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.btnSavePanel = new System.Windows.Forms.ToolStripMenuItem();
			this.btnSavePanelAs = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.btnExit = new System.Windows.Forms.ToolStripMenuItem();
			this.ConnectionMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.btnChangeConnection = new System.Windows.Forms.ToolStripMenuItem();
			this.btnForceDisconnect = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.btnRefreshLuaStates = new System.Windows.Forms.ToolStripMenuItem();
			this.MainMenu = new System.Windows.Forms.MenuStrip();
			this.adminToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.btnEditProjectPanels = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutGameTunerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tabLuaConsole = new System.Windows.Forms.TabPage();
			this.LuaConsole = new LuaConsole();
			this.ctrlMainFormTabs = new System.Windows.Forms.TabControl();
			this.tabCreator = new System.Windows.Forms.TabPage();
			this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
			this.tmrProcessMessages = new System.Windows.Forms.Timer(this.components);
			this.ctrlStatusStrip.SuspendLayout();
			this.MainMenu.SuspendLayout();
			this.tabLuaConsole.SuspendLayout();
			this.ctrlMainFormTabs.SuspendLayout();
			base.SuspendLayout();
			this.ctrlStatusStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.ctrlStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.lblConnectionStatus });
			this.ctrlStatusStrip.Location = new System.Drawing.Point(0, 542);
			this.ctrlStatusStrip.Name = "ctrlStatusStrip";
			this.ctrlStatusStrip.Size = new System.Drawing.Size(944, 22);
			this.ctrlStatusStrip.TabIndex = 1;
			this.lblConnectionStatus.Name = "lblConnectionStatus";
			this.lblConnectionStatus.Size = new System.Drawing.Size(79, 17);
			this.lblConnectionStatus.Text = "Disconnected";
			this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[7] { this.btnNewPanel, this.btnOpenPanel, this.toolStripSeparator1, this.btnSavePanel, this.btnSavePanelAs, this.toolStripSeparator2, this.btnExit });
			this.FileMenu.Name = "FileMenu";
			this.FileMenu.ShortcutKeys = System.Windows.Forms.Keys.F | System.Windows.Forms.Keys.Alt;
			this.FileMenu.Size = new System.Drawing.Size(37, 20);
			this.FileMenu.Text = "File";
			this.btnNewPanel.Image = (System.Drawing.Image)resources.GetObject("btnNewPanel.Image");
			this.btnNewPanel.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.btnNewPanel.Name = "btnNewPanel";
			this.btnNewPanel.ShortcutKeys = System.Windows.Forms.Keys.N | System.Windows.Forms.Keys.Alt;
			this.btnNewPanel.Size = new System.Drawing.Size(174, 22);
			this.btnNewPanel.Text = "New Panel";
			this.btnNewPanel.Click += new System.EventHandler(btnNewPanel_Click);
			this.btnOpenPanel.Image = (System.Drawing.Image)resources.GetObject("btnOpenPanel.Image");
			this.btnOpenPanel.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.btnOpenPanel.Name = "btnOpenPanel";
			this.btnOpenPanel.ShortcutKeys = System.Windows.Forms.Keys.O | System.Windows.Forms.Keys.Alt;
			this.btnOpenPanel.Size = new System.Drawing.Size(174, 22);
			this.btnOpenPanel.Text = "Open Panel";
			this.btnOpenPanel.Click += new System.EventHandler(btnOpenPanel_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(171, 6);
			this.btnSavePanel.Image = (System.Drawing.Image)resources.GetObject("btnSavePanel.Image");
			this.btnSavePanel.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.btnSavePanel.Name = "btnSavePanel";
			this.btnSavePanel.ShortcutKeys = System.Windows.Forms.Keys.S | System.Windows.Forms.Keys.Alt;
			this.btnSavePanel.Size = new System.Drawing.Size(174, 22);
			this.btnSavePanel.Text = "Save Panel";
			this.btnSavePanel.Click += new System.EventHandler(btnSavePanel_Click);
			this.btnSavePanelAs.Name = "btnSavePanelAs";
			this.btnSavePanelAs.Size = new System.Drawing.Size(174, 22);
			this.btnSavePanelAs.Text = "Save Panel As...";
			this.btnSavePanelAs.Click += new System.EventHandler(btnSavePanelAs_Click);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(171, 6);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(174, 22);
			this.btnExit.Text = "Exit";
			this.btnExit.Click += new System.EventHandler(btnExit_Click);
			this.ConnectionMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.btnChangeConnection, this.btnForceDisconnect, this.toolStripSeparator3, this.btnRefreshLuaStates });
			this.ConnectionMenu.Name = "ConnectionMenu";
			this.ConnectionMenu.Size = new System.Drawing.Size(81, 20);
			this.ConnectionMenu.Text = "Connection";
			this.btnChangeConnection.Image = (System.Drawing.Image)resources.GetObject("btnChangeConnection.Image");
			this.btnChangeConnection.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.btnChangeConnection.Name = "btnChangeConnection";
			this.btnChangeConnection.Size = new System.Drawing.Size(180, 22);
			this.btnChangeConnection.Text = "Change Connection";
			this.btnChangeConnection.Click += new System.EventHandler(btnChangeConnection_Click);
			this.btnForceDisconnect.Image = (System.Drawing.Image)resources.GetObject("btnForceDisconnect.Image");
			this.btnForceDisconnect.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.btnForceDisconnect.Name = "btnForceDisconnect";
			this.btnForceDisconnect.Size = new System.Drawing.Size(180, 22);
			this.btnForceDisconnect.Text = "Force Disconnect";
			this.btnForceDisconnect.Click += new System.EventHandler(btnForceDisconnect_Click);
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
			this.btnRefreshLuaStates.Image = (System.Drawing.Image)resources.GetObject("btnRefreshLuaStates.Image");
			this.btnRefreshLuaStates.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.btnRefreshLuaStates.Name = "btnRefreshLuaStates";
			this.btnRefreshLuaStates.Size = new System.Drawing.Size(180, 22);
			this.btnRefreshLuaStates.Text = "Refresh Lua States";
			this.btnRefreshLuaStates.Click += new System.EventHandler(btnRefreshLuaStates_Click);
			this.MainMenu.BackColor = System.Drawing.SystemColors.Control;
			this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.FileMenu, this.ConnectionMenu, this.adminToolStripMenuItem, this.helpToolStripMenuItem });
			this.MainMenu.Location = new System.Drawing.Point(0, 0);
			this.MainMenu.Name = "MainMenu";
			this.MainMenu.Size = new System.Drawing.Size(944, 24);
			this.MainMenu.TabIndex = 2;
			this.adminToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.btnEditProjectPanels });
			this.adminToolStripMenuItem.Name = "adminToolStripMenuItem";
			this.adminToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
			this.adminToolStripMenuItem.Text = "Admin";
			this.btnEditProjectPanels.Image = (System.Drawing.Image)resources.GetObject("btnEditProjectPanels.Image");
			this.btnEditProjectPanels.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.btnEditProjectPanels.Name = "btnEditProjectPanels";
			this.btnEditProjectPanels.Size = new System.Drawing.Size(171, 22);
			this.btnEditProjectPanels.Text = "Edit Project Panels";
			this.btnEditProjectPanels.Click += new System.EventHandler(btnEditProjectPanels_Click);
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.aboutGameTunerToolStripMenuItem });
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			this.aboutGameTunerToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("aboutGameTunerToolStripMenuItem.Image");
			this.aboutGameTunerToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.aboutGameTunerToolStripMenuItem.Name = "aboutGameTunerToolStripMenuItem";
			this.aboutGameTunerToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.aboutGameTunerToolStripMenuItem.Text = "About GameTuner...";
			this.aboutGameTunerToolStripMenuItem.Click += new System.EventHandler(aboutGameTunerToolStripMenuItem_Click);
			this.tabLuaConsole.BackColor = System.Drawing.Color.Black;
			this.tabLuaConsole.Controls.Add(this.LuaConsole);
			this.tabLuaConsole.Location = new System.Drawing.Point(4, 22);
			this.tabLuaConsole.Name = "tabLuaConsole";
			this.tabLuaConsole.Size = new System.Drawing.Size(935, 486);
			this.tabLuaConsole.TabIndex = 0;
			this.tabLuaConsole.Text = "Lua Console";
			this.LuaConsole.BackColor = System.Drawing.Color.Black;
			this.LuaConsole.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LuaConsole.ForeColor = System.Drawing.Color.White;
			this.LuaConsole.Location = new System.Drawing.Point(0, 0);
			this.LuaConsole.Name = "LuaConsole";
			this.LuaConsole.Size = new System.Drawing.Size(935, 486);
			this.LuaConsole.TabIndex = 0;
			this.ctrlMainFormTabs.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.ctrlMainFormTabs.Controls.Add(this.tabLuaConsole);
			this.ctrlMainFormTabs.Controls.Add(this.tabCreator);
			this.ctrlMainFormTabs.Location = new System.Drawing.Point(0, 27);
			this.ctrlMainFormTabs.Name = "ctrlMainFormTabs";
			this.ctrlMainFormTabs.SelectedIndex = 0;
			this.ctrlMainFormTabs.Size = new System.Drawing.Size(943, 512);
			this.ctrlMainFormTabs.TabIndex = 0;
			this.ctrlMainFormTabs.Selected += new System.Windows.Forms.TabControlEventHandler(ctrlMainFormTabs_Selected);
			this.ctrlMainFormTabs.TabIndexChanged += new System.EventHandler(ctrlMainFormTabs_TabIndexChanged);
			this.ctrlMainFormTabs.SelectedIndexChanged += new System.EventHandler(ctrlMainFormTabs_SelectedIndexChanged);
			this.tabCreator.BackColor = System.Drawing.Color.Black;
			this.tabCreator.Location = new System.Drawing.Point(4, 22);
			this.tabCreator.Name = "tabCreator";
			this.tabCreator.Padding = new System.Windows.Forms.Padding(3);
			this.tabCreator.Size = new System.Drawing.Size(935, 486);
			this.tabCreator.TabIndex = 1;
			this.tabCreator.Text = "* New Panel *";
			this.tmrUpdate.Interval = 500;
			this.tmrUpdate.Tick += new System.EventHandler(tmrUpdate_Tick);
			this.tmrProcessMessages.Interval = 1;
			this.tmrProcessMessages.Tick += new System.EventHandler(tmrProcessMessages_Tick);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			base.ClientSize = new System.Drawing.Size(944, 564);
			base.Controls.Add(this.ctrlStatusStrip);
			base.Controls.Add(this.MainMenu);
			base.Controls.Add(this.ctrlMainFormTabs);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.MainMenuStrip = this.MainMenu;
			base.Name = "frmMainForm";
			this.Text = "GameTuner";
			base.Load += new System.EventHandler(frmMainForm_Load);
			base.Shown += new System.EventHandler(frmMainForm_Shown);
			base.Move += new System.EventHandler(frmMainForm_Move);
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmMainForm_FormClosing);
			base.Resize += new System.EventHandler(frmMainForm_Resize);
			this.ctrlStatusStrip.ResumeLayout(false);
			this.ctrlStatusStrip.PerformLayout();
			this.MainMenu.ResumeLayout(false);
			this.MainMenu.PerformLayout();
			this.tabLuaConsole.ResumeLayout(false);
			this.ctrlMainFormTabs.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
