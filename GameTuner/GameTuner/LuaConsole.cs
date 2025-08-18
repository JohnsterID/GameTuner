using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameTuner.Framework;

namespace GameTuner
{
	public class LuaConsole : UserControl
	{
		private struct FontEntry
		{
			public int iSelectionStart;

			public int iSelectionLength;

			public Font font;

			public Color color;
		}

		private int m_iOnHelpRecievedListener;

		private int m_iUpdateHelpEntriesListener;

		private int m_iConsoleCommandOutputRecievedListener;

		private string m_sCurrentStateName = string.Empty;

		private AutoCompletePopup m_CodeBuddyWnd = new AutoCompletePopup();

		private static char[] m_CodeBuddyTriggers = new char[4] { ':', '.', '[', ']' };

		private ArrayList m_ConsoleHistory = new ArrayList();

		private int m_iHistoryIndex = -1;

		private string m_sPrevInputText = string.Empty;

		private List<string> m_OutputMessages = new List<string>();

		private List<FontEntry> m_FontStyles = new List<FontEntry>();

		private IContainer components;

		private ToolStrip ConsoleTools;

		private ToolStripComboBox cmbConsoleLuaState;

		private TextBox txtConsoleInput;

		private RichTextBox txtConsoleOutput;

		private ToolStripButton btnClearConsole;

		private ToolStripCheckBox m_chkLockLuaState;

		public bool ConsoleInputFocused
		{
			get
			{
				return txtConsoleInput.Focused;
			}
		}

		public LuaState LuaState
		{
			get
			{
				return (LuaState)cmbConsoleLuaState.SelectedItem;
			}
		}

		public bool LockLuaState
		{
			get
			{
				return m_chkLockLuaState.Checked;
			}
			set
			{
				m_chkLockLuaState.Checked = value;
			}
		}

		public string DefaultLuaState { get; set; }

		public LuaConsole()
		{
			InitializeComponent();
			m_CodeBuddyWnd.Owner = frmMainForm.MainForm;
			m_CodeBuddyWnd.TopMost = true;
			m_CodeBuddyWnd.TextBox = txtConsoleInput;
			m_CodeBuddyWnd.SetCategoryBrush(3u, Brushes.Green);
			m_CodeBuddyWnd.SetCategoryBrush(4u, Brushes.Blue);
			m_CodeBuddyWnd.SetCategoryBrush(5u, Brushes.Maroon);
			m_CodeBuddyWnd.InvertColorsForHighlight = true;
			m_CodeBuddyWnd.OnSelection += OnCodeBuddySelection;
			m_CodeBuddyWnd.FindReplacementSymbol = FindLastSymbol;
		}

		public void Init()
		{
			if (Connection.Instance != null)
			{
				Connection.Instance.OnOutputMsgRecieved = PrintOutputMessage;
				m_iOnHelpRecievedListener = Connection.Instance.AddRequestListener(OnHelpRecieved);
				m_iUpdateHelpEntriesListener = Connection.Instance.AddRequestListener(UpdateHelpEntries);
				m_iConsoleCommandOutputRecievedListener = Connection.Instance.AddRequestListener(ConsoleCommandOutputRecieved);
			}
		}

		private void btnClearConsole_Click(object sender, EventArgs e)
		{
			txtConsoleOutput.Text = string.Empty;
		}

		public void UpdateLuaStates(IEnumerable<LuaState> luaStates)
		{
			LuaState luaState = LuaState;
			cmbConsoleLuaState.Items.Clear();
			if (luaStates != null)
			{
				foreach (LuaState luaState4 in luaStates)
				{
					cmbConsoleLuaState.Items.Add(luaState4);
				}
			}
			if (luaState != null)
			{
				foreach (LuaState item in cmbConsoleLuaState.Items)
				{
					if (item.Name == luaState.Name)
					{
						cmbConsoleLuaState.SelectedItem = item;
						break;
					}
				}
			}
			else
			{
				string text = ((!LockLuaState) ? "Main State" : DefaultLuaState);
				foreach (LuaState item2 in cmbConsoleLuaState.Items)
				{
					if (item2.Name == text)
					{
						cmbConsoleLuaState.SelectedItem = item2;
						break;
					}
				}
			}
			if (cmbConsoleLuaState.SelectedIndex == -1 && !LockLuaState)
			{
				if (cmbConsoleLuaState.Items.Count > 0)
				{
					cmbConsoleLuaState.SelectedIndex = 0;
				}
				else
				{
					cmbConsoleLuaState.Text = "";
				}
			}
		}

		private void cmbConsoleLuaState_SelectedIndexChanged(object sender, EventArgs e)
		{
			string name = LuaState.Name;
			if (name != m_sCurrentStateName)
			{
				PrintToConsoleOutput("[ Lua State = " + name + " ]\n", FontStyle.Bold, Color.LightBlue);
				m_sCurrentStateName = name;
			}
		}

		private void m_chkLockLuaState_Click(object sender, EventArgs e)
		{
			if (m_chkLockLuaState.Checked && cmbConsoleLuaState.SelectedItem != null)
			{
				DefaultLuaState = cmbConsoleLuaState.SelectedItem.ToString();
			}
			else
			{
				DefaultLuaState = string.Empty;
			}
		}

		public void ForceCloseCodeBuddy()
		{
			if (m_CodeBuddyWnd.PopupOpen)
			{
				m_CodeBuddyWnd.PopupOpen = false;
			}
		}

		private bool IsSymbolChar(char c)
		{
			if ((c < 'A' || c > 'Z') && (c < 'a' || c > 'z') && (c < '0' || c > '9'))
			{
				return c == '_';
			}
			return true;
		}

		private string FindLastSymbol(string sLuaCode)
		{
			string text = null;
			for (int num = sLuaCode.Length - 1; num >= 0; num--)
			{
				if (!IsSymbolChar(sLuaCode[num]))
				{
					int num2 = num + 1;
					text = ((num2 >= sLuaCode.Length) ? string.Empty : sLuaCode.Substring(num2));
					break;
				}
			}
			if (text == null)
			{
				text = sLuaCode;
			}
			return text;
		}

		private void OnHelpRecieved(List<string> helpStrings)
		{
			UpdateHelpEntries(helpStrings);
			m_sPrevInputText = txtConsoleInput.Text;
			m_CodeBuddyWnd.PopupOpen = true;
		}

		private void UpdateHelpEntries(List<string> helpStrings)
		{
			List<AutoCompletePopup.Entry> list = new List<AutoCompletePopup.Entry>();
			for (int i = 0; i + 1 < helpStrings.Count; i += 3)
			{
				uint result = 0u;
				uint.TryParse(helpStrings[i], out result);
				AutoCompletePopup.Entry item = new AutoCompletePopup.Entry(helpStrings[i + 1], helpStrings[i + 2], result);
				list.Add(item);
			}
			m_CodeBuddyWnd.Entries = list;
		}

		private void OnCodeBuddySelection(object sender, AutoCompletePopup.AutoCompleteEventArgs e)
		{
			string text = FindLastSymbol(txtConsoleInput.Text);
			if (string.IsNullOrEmpty(text))
			{
				txtConsoleInput.Text += e.Selected;
			}
			else
			{
				string text2 = txtConsoleInput.Text;
				text2 = text2.Substring(0, text2.Length - text.Length);
				text2 += e.Selected;
				txtConsoleInput.Text = text2;
			}
			txtConsoleInput.SelectionStart = txtConsoleInput.Text.Length;
			txtConsoleInput.SelectionLength = 0;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (m_CodeBuddyWnd.PopupOpen && (keyData == Keys.Up || keyData == Keys.Down))
			{
				KeyEventArgs e = new KeyEventArgs(keyData);
				m_CodeBuddyWnd.TextBox_KeyDown(this, e);
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void txtConsoleInput_KeyDown(object sender, KeyEventArgs e)
		{
			if (!m_CodeBuddyWnd.PopupOpen)
			{
				if (e.KeyCode.Equals(Keys.Return))
				{
					RunConsoleCommand();
				}
				else if (e.KeyCode.Equals(Keys.Tab) && Connection.Instance.Connected && LuaState != null)
				{
					Connection.Instance.Request("HELP:" + LuaState.ID + ":" + txtConsoleInput.Text, m_iOnHelpRecievedListener);
				}
			}
			else if (e.KeyCode.Equals(Keys.Tab) || e.KeyCode.Equals(Keys.Escape))
			{
				m_CodeBuddyWnd.PopupOpen = false;
			}
		}

		private void txtConsoleInput_KeyUp(object sender, KeyEventArgs e)
		{
			if (m_CodeBuddyWnd.PopupOpen)
			{
				return;
			}
			if (e.KeyCode.Equals(Keys.Up))
			{
				int count = m_ConsoleHistory.Count;
				if (count > 0)
				{
					m_iHistoryIndex--;
					if (m_iHistoryIndex < 0)
					{
						m_iHistoryIndex = count - 1;
					}
					txtConsoleInput.Text = m_ConsoleHistory[m_iHistoryIndex].ToString();
					txtConsoleInput.SelectionStart = txtConsoleInput.Text.Length;
				}
			}
			else
			{
				if (!e.KeyCode.Equals(Keys.Down))
				{
					return;
				}
				int count2 = m_ConsoleHistory.Count;
				if (count2 > 0)
				{
					m_iHistoryIndex++;
					if (m_iHistoryIndex >= count2)
					{
						m_iHistoryIndex = 0;
					}
					txtConsoleInput.Text = m_ConsoleHistory[m_iHistoryIndex].ToString();
					txtConsoleInput.SelectionStart = txtConsoleInput.Text.Length;
				}
			}
		}

		private void txtConsoleInput_TextChanged(object sender, EventArgs e)
		{
			if (!m_CodeBuddyWnd.PopupOpen)
			{
				return;
			}
			bool flag = false;
			string text = txtConsoleInput.Text;
			int length = text.Length;
			int length2 = m_sPrevInputText.Length;
			if (length > length2 && length2 > 0)
			{
				string source = text.Replace(m_sPrevInputText, "");
				char[] codeBuddyTriggers = m_CodeBuddyTriggers;
				foreach (char value in codeBuddyTriggers)
				{
					if (source.Contains(value))
					{
						flag = true;
						break;
					}
				}
			}
			else if (length2 > length && length > 0)
			{
				string source2 = m_sPrevInputText.Replace(text, "");
				char[] codeBuddyTriggers2 = m_CodeBuddyTriggers;
				foreach (char value2 in codeBuddyTriggers2)
				{
					if (source2.Contains(value2))
					{
						flag = true;
						break;
					}
				}
			}
			else if (length2 == length)
			{
				flag = true;
			}
			if (flag && LuaState != null)
			{
				Connection.Instance.Request("HELP:" + LuaState.ID + ":" + txtConsoleInput.Text, m_iUpdateHelpEntriesListener);
			}
		}

		private bool RunConsoleCommand()
		{
			if (!Connection.Instance.Connected)
			{
				MessageBox.Show("Can't run a command while disconnected");
				return false;
			}
			if (LuaState == null)
			{
				MessageBox.Show("No valid lua state");
				return false;
			}
			string text = txtConsoleInput.Text;
			m_ConsoleHistory.Add(text);
			m_iHistoryIndex = -1;
			txtConsoleInput.ReadOnly = true;
			PrintToConsoleOutput("> " + txtConsoleInput.Text + "\n");
			Connection.Instance.Request("CMD:" + LuaState.ID + ":" + text, m_iConsoleCommandOutputRecievedListener);
			return true;
		}

		private void ConsoleCommandOutputRecieved(List<string> results)
		{
			string text = results[0];
			if (!text.Equals(string.Empty))
			{
				if (!text.StartsWith("ERR"))
				{
					PrintToConsoleOutput(text + "\n");
				}
				else
				{
					PrintToConsoleOutput(text.Substring(4) + "\n", Color.Red);
				}
			}
			txtConsoleInput.Text = string.Empty;
			txtConsoleInput.ReadOnly = false;
		}

		private void txtConsoleOutput_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
			{
				string text = txtConsoleOutput.Text.Substring(txtConsoleOutput.SelectionStart, txtConsoleOutput.SelectionLength);
				Clipboard.SetText(text);
			}
		}

		public void PrintToConsoleOutput(string sText)
		{
			PrintToConsoleOutput(sText, txtConsoleOutput.Font, txtConsoleOutput.ForeColor);
		}

		public void PrintToConsoleOutput(string sText, FontStyle style)
		{
			Font font = new Font(txtConsoleOutput.SelectionFont, style);
			PrintToConsoleOutput(sText, font, txtConsoleOutput.ForeColor);
		}

		public void PrintToConsoleOutput(string sText, Font font)
		{
			PrintToConsoleOutput(sText, font, txtConsoleOutput.ForeColor);
		}

		public void PrintToConsoleOutput(string sText, Color color)
		{
			PrintToConsoleOutput(sText, txtConsoleOutput.Font, color);
		}

		public void PrintToConsoleOutput(string sText, FontStyle style, Color color)
		{
			Font font = new Font(txtConsoleOutput.SelectionFont, style);
			PrintToConsoleOutput(sText, font, color);
		}

		public void PrintToConsoleOutput(string sText, Font font, Color color)
		{
			if (txtConsoleOutput.IsDisposed)
			{
				return;
			}
			int textLength = txtConsoleOutput.TextLength;
			txtConsoleOutput.Text += sText;
			int textLength2 = txtConsoleOutput.TextLength;
			FontEntry item = default(FontEntry);
			item.iSelectionStart = textLength;
			item.iSelectionLength = textLength2 - textLength;
			item.color = color;
			item.font = font;
			m_FontStyles.Add(item);
			foreach (FontEntry fontStyle in m_FontStyles)
			{
				txtConsoleOutput.SelectionStart = fontStyle.iSelectionStart;
				txtConsoleOutput.SelectionLength = fontStyle.iSelectionLength;
				txtConsoleOutput.SelectionFont = fontStyle.font;
				txtConsoleOutput.SelectionColor = fontStyle.color;
			}
			AutoScrollConsoleOutput();
		}

		private void PrintOutputMessage(string sOutputMsg)
		{
			m_OutputMessages.Add(sOutputMsg);
		}

		public void FlushOutputMessages()
		{
			if (m_OutputMessages.Count <= 0)
			{
				return;
			}
			string text = string.Empty;
			foreach (string outputMessage in m_OutputMessages)
			{
				text = text + " " + outputMessage.TrimEnd('\n') + "\n";
			}
			PrintToConsoleOutput(text, FontStyle.Italic);
			m_OutputMessages.Clear();
		}

		private void AutoScrollConsoleOutput()
		{
			txtConsoleOutput.SelectionStart = txtConsoleOutput.TextLength;
			txtConsoleOutput.SelectionLength = 0;
			txtConsoleOutput.ScrollToCaret();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LuaConsole));
			this.ConsoleTools = new System.Windows.Forms.ToolStrip();
			this.cmbConsoleLuaState = new System.Windows.Forms.ToolStripComboBox();
			this.btnClearConsole = new System.Windows.Forms.ToolStripButton();
			this.m_chkLockLuaState = new ToolStripCheckBox();
			this.txtConsoleInput = new System.Windows.Forms.TextBox();
			this.txtConsoleOutput = new System.Windows.Forms.RichTextBox();
			this.ConsoleTools.SuspendLayout();
			base.SuspendLayout();
			this.ConsoleTools.BackColor = System.Drawing.Color.Black;
			this.ConsoleTools.BackgroundImage = (System.Drawing.Image)resources.GetObject("ConsoleTools.BackgroundImage");
			this.ConsoleTools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.ConsoleTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.cmbConsoleLuaState, this.btnClearConsole, this.m_chkLockLuaState });
			this.ConsoleTools.Location = new System.Drawing.Point(0, 0);
			this.ConsoleTools.Name = "ConsoleTools";
			this.ConsoleTools.Size = new System.Drawing.Size(859, 25);
			this.ConsoleTools.TabIndex = 5;
			this.ConsoleTools.Text = "Console Tools";
			this.cmbConsoleLuaState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbConsoleLuaState.Name = "cmbConsoleLuaState";
			this.cmbConsoleLuaState.Size = new System.Drawing.Size(121, 25);
			this.cmbConsoleLuaState.Sorted = true;
			this.cmbConsoleLuaState.SelectedIndexChanged += new System.EventHandler(cmbConsoleLuaState_SelectedIndexChanged);
			this.btnClearConsole.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.btnClearConsole.BackColor = System.Drawing.Color.Gainsboro;
			this.btnClearConsole.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.btnClearConsole.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnClearConsole.ForeColor = System.Drawing.Color.Black;
			this.btnClearConsole.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnClearConsole.Name = "btnClearConsole";
			this.btnClearConsole.Size = new System.Drawing.Size(79, 22);
			this.btnClearConsole.Text = "Clear Output";
			this.btnClearConsole.Click += new System.EventHandler(btnClearConsole_Click);
			this.m_chkLockLuaState.BackColor = System.Drawing.Color.Transparent;
			this.m_chkLockLuaState.Checked = false;
			this.m_chkLockLuaState.Name = "m_chkLockLuaState";
			this.m_chkLockLuaState.Size = new System.Drawing.Size(102, 22);
			this.m_chkLockLuaState.Text = "Lock Lua State";
			this.m_chkLockLuaState.Click += new System.EventHandler(m_chkLockLuaState_Click);
			this.txtConsoleInput.AcceptsTab = true;
			this.txtConsoleInput.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtConsoleInput.BackColor = System.Drawing.Color.Black;
			this.txtConsoleInput.Font = new System.Drawing.Font("Courier New", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtConsoleInput.ForeColor = System.Drawing.Color.White;
			this.txtConsoleInput.Location = new System.Drawing.Point(3, 449);
			this.txtConsoleInput.Name = "txtConsoleInput";
			this.txtConsoleInput.Size = new System.Drawing.Size(853, 20);
			this.txtConsoleInput.TabIndex = 4;
			this.txtConsoleInput.TextChanged += new System.EventHandler(txtConsoleInput_TextChanged);
			this.txtConsoleInput.KeyDown += new System.Windows.Forms.KeyEventHandler(txtConsoleInput_KeyDown);
			this.txtConsoleInput.KeyUp += new System.Windows.Forms.KeyEventHandler(txtConsoleInput_KeyUp);
			this.txtConsoleOutput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtConsoleOutput.BackColor = System.Drawing.Color.Black;
			this.txtConsoleOutput.DetectUrls = false;
			this.txtConsoleOutput.Font = new System.Drawing.Font("Courier New", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtConsoleOutput.ForeColor = System.Drawing.Color.White;
			this.txtConsoleOutput.Location = new System.Drawing.Point(0, 24);
			this.txtConsoleOutput.Name = "txtConsoleOutput";
			this.txtConsoleOutput.ReadOnly = true;
			this.txtConsoleOutput.ShortcutsEnabled = false;
			this.txtConsoleOutput.Size = new System.Drawing.Size(859, 419);
			this.txtConsoleOutput.TabIndex = 3;
			this.txtConsoleOutput.Text = "";
			this.txtConsoleOutput.WordWrap = false;
			this.txtConsoleOutput.KeyUp += new System.Windows.Forms.KeyEventHandler(txtConsoleOutput_KeyUp);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			base.Controls.Add(this.ConsoleTools);
			base.Controls.Add(this.txtConsoleInput);
			base.Controls.Add(this.txtConsoleOutput);
			this.ForeColor = System.Drawing.Color.White;
			base.Name = "LuaConsole";
			base.Size = new System.Drawing.Size(859, 472);
			this.ConsoleTools.ResumeLayout(false);
			this.ConsoleTools.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
