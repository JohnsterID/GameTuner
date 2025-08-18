using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using GameTuner.Framework;

namespace GameTuner
{
	public class ValueControl : Panel, ICustomControl
	{
		private string m_sSetFunc = string.Empty;

		private string m_sGetFunc = string.Empty;

		protected Label m_lbl = new Label();

		protected TextBox m_txt = new TextBox();

		private Timer m_SetValueTimer = new Timer();

		private int m_iNetworkListener;

		private bool m_bUpdatingValue;

		private bool m_bTabSelected;

		private ContextMenuStrip m_mnuContext = new ContextMenuStrip();

		private ToolStripMenuItem m_btnEdit = new ToolStripMenuItem("Edit");

		private ToolStripMenuItem m_btnCopy = new ToolStripMenuItem("Copy");

		private ToolStripMenuItem m_btnMove = new ToolStripMenuItem("Move");

		private ToolStripMenuItem m_btnDelete = new ToolStripMenuItem("Delete");

		public string ControlName
		{
			get
			{
				return m_lbl.Text.TrimEnd(':');
			}
			set
			{
				m_lbl.Text = value + ':';
				Graphics graphics = m_lbl.CreateGraphics();
				SizeF sizeF = graphics.MeasureString(m_lbl.Text, m_lbl.Font);
				m_lbl.Width = (int)sizeF.Width + 4;
				OnLabelResized();
			}
		}

		public string SetFunction
		{
			get
			{
				return m_sSetFunc;
			}
			set
			{
				m_sSetFunc = value;
			}
		}

		public string GetFunction
		{
			get
			{
				return m_sGetFunc;
			}
			set
			{
				m_sGetFunc = value;
				CustomUI customUI = (CustomUI)base.Parent;
				if (customUI != null && customUI.LuaState != null && m_bTabSelected)
				{
					OpenQuery(customUI.LuaState);
				}
			}
		}

		protected virtual void OnLabelResized()
		{
			m_txt.Left = m_lbl.Width + 4;
			base.Width = m_lbl.Width + m_txt.Width + 12;
		}

		public ValueControl(MouseEventHandler onMouseClick, MouseEventHandler onMouseMove)
		{
			m_SetValueTimer.Interval = 250;
			m_SetValueTimer.Tick += m_SetValueTimer_Tick;
			base.BorderStyle = BorderStyle.FixedSingle;
			base.MouseMove += onMouseMove;
			base.MouseClick += onMouseClick;
			m_lbl.Location = new Point(4, 4);
			m_lbl.Height = m_lbl.Font.Height;
			m_lbl.MouseMove += onMouseMove;
			m_lbl.MouseClick += onMouseClick;
			base.Controls.Add(m_lbl);
			m_txt.Location = new Point(4, 4);
			m_txt.Height = m_txt.Font.Height;
			m_txt.BackColor = Color.Black;
			m_txt.ForeColor = Color.White;
			m_txt.MouseMove += onMouseMove;
			m_txt.TextChanged += OnTextChanged;
			base.Controls.Add(m_txt);
			m_btnEdit.Click += m_btnEdit_Click;
			m_btnCopy.Click += m_btnCopy_Click;
			m_btnMove.Click += m_btnMove_Click;
			m_btnDelete.Click += m_btnDelete_Click;
			m_mnuContext.Items.Add(m_btnEdit);
			m_mnuContext.Items.Add(m_btnCopy);
			m_mnuContext.Items.Add(m_btnMove);
			m_mnuContext.Items.Add(new ToolStripSeparator());
			m_mnuContext.Items.Add(m_btnDelete);
			ContextMenuStrip = m_mnuContext;
			m_iNetworkListener = Connection.Instance.AddRequestListener(CompletedAction);
			base.Height = m_txt.Height + 8;
			m_lbl.Top = m_txt.Top + (m_txt.Height - m_lbl.Height) / 2;
		}

		public void Release()
		{
			Connection.Instance.RemoveRequestListener(m_iNetworkListener);
		}

		public void LuaStateChanged(LuaState state, LuaState lastState)
		{
			if (lastState != null)
			{
				KillQuery(lastState);
			}
			if (state != null && m_bTabSelected)
			{
				OpenQuery(state);
			}
		}

		private void OpenQuery(LuaState state)
		{
			Connection.Instance.Request("QRY:" + state.ID + ":" + m_sGetFunc, m_iNetworkListener);
		}

		private void KillQuery(LuaState state)
		{
			Connection.Instance.Request("KILLQRY:" + state.ID + ":", m_iNetworkListener);
		}

		public void CompletedAction(List<string> luaMessages)
		{
			if (luaMessages.Count == 1)
			{
				ValueUpdateRecieved(luaMessages[0]);
			}
			else if (luaMessages.Count == 2 && luaMessages[0] == m_txt.Text)
			{
				ValueUpdateRecieved(luaMessages[1]);
			}
		}

		private void ValueUpdateRecieved(string sValue)
		{
			m_bUpdatingValue = true;
			if (sValue.StartsWith("ERR:"))
			{
				string text = sValue.Substring(4);
				if (text.StartsWith("SET:"))
				{
					MessageBox.Show(text.Substring(4), "Error Setting Value");
				}
				else
				{
					m_txt.Enabled = false;
					BackColor = Color.Red;
					m_txt.Text = text;
				}
			}
			else
			{
				m_txt.Enabled = true;
				BackColor = Color.Black;
				ValueUpdated(sValue);
			}
			m_bUpdatingValue = false;
		}

		protected virtual void ValueUpdated(string sNewVal)
		{
			m_txt.Text = sNewVal;
		}

		public void StartDrag()
		{
			base.BorderStyle = BorderStyle.Fixed3D;
			HideControls();
		}

		public void EndDrag()
		{
			base.BorderStyle = BorderStyle.FixedSingle;
			ShowControls();
		}

		public void TabEntered()
		{
			m_bTabSelected = true;
			CustomUI customUI = (CustomUI)base.Parent;
			if (customUI != null && customUI.LuaState != null)
			{
				OpenQuery(customUI.LuaState);
			}
		}

		public void TabLeft()
		{
			m_bTabSelected = false;
			CustomUI customUI = (CustomUI)base.Parent;
			if (customUI != null && customUI.LuaState != null)
			{
				KillQuery(customUI.LuaState);
			}
		}

		protected virtual void HideControls()
		{
			m_lbl.Hide();
			m_txt.Hide();
		}

		protected virtual void ShowControls()
		{
			m_lbl.Show();
			m_txt.Show();
		}

		protected virtual string ValidateInput(string s)
		{
			return s;
		}

		private void OnTextChanged(object o, EventArgs e)
		{
			if (!m_bUpdatingValue)
			{
				m_SetValueTimer.Stop();
				m_txt.Text = ValidateInput(m_txt.Text);
				m_SetValueTimer.Start();
			}
			ResizeToFitText();
			if (base.Parent is CustomUI)
			{
				CustomUI customUI = base.Parent as CustomUI;
				if (customUI.MovingControl == this)
				{
					Point controlMoveOffset = customUI.m_ControlMoveOffset;
					controlMoveOffset.X = -base.Width / 2;
					customUI.m_ControlMoveOffset = controlMoveOffset;
				}
			}
		}

		protected virtual void ResizeToFitText()
		{
			Graphics graphics = m_txt.CreateGraphics();
			SizeF sizeF = graphics.MeasureString(m_txt.Text, m_txt.Font);
			m_txt.Width = (int)sizeF.Width + 4;
			int selectionStart = m_txt.SelectionStart;
			m_txt.SelectionStart = 0;
			m_txt.SelectionStart = selectionStart;
			base.Width = m_lbl.Width + m_txt.Width + 12;
		}

		private void m_SetValueTimer_Tick(object sender, EventArgs e)
		{
			SetValue();
			m_SetValueTimer.Stop();
		}

		protected virtual string GetValueType()
		{
			return "string";
		}

		protected virtual string GetValue()
		{
			return m_txt.Text;
		}

		private void SetValue()
		{
			CustomUI customUI = (CustomUI)base.Parent;
			if (customUI != null && customUI.LuaState != null)
			{
				Connection.Instance.Request("VAL:" + customUI.LuaState.ID + ":" + GetValueType() + ":" + GetValue() + ":" + SetFunction, m_iNetworkListener);
			}
		}

		protected virtual void EditControl()
		{
		}

		private void m_btnEdit_Click(object sender, EventArgs e)
		{
			EditControl();
		}

		private void m_btnCopy_Click(object sender, EventArgs e)
		{
			StringWriter stringWriter = new StringWriter();
			try
			{
				CustomUI.PanelData.ValueControlData data = ((this is StringControl) ? new CustomUI.PanelData.StringControlData((StringControl)this) : ((this is IntegerControl) ? new CustomUI.PanelData.IntegerControlData((IntegerControl)this) : ((this is FloatControl) ? new CustomUI.PanelData.FloatControlData((FloatControl)this) : ((!(this is BooleanControl)) ? new CustomUI.PanelData.ValueControlData(this) : new CustomUI.PanelData.BooleanControlData((BooleanControl)this)))));
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(GenericObjectSerializer));
				xmlSerializer.Serialize(stringWriter, new GenericObjectSerializer(data));
				Clipboard.SetText(stringWriter.ToString());
			}
			catch (Exception ex)
			{
				ErrorHandling.Error(ex, "Failed to copy control:\n" + ex.Message, ErrorLevel.ShowMessage);
			}
			finally
			{
				stringWriter.Close();
			}
		}

		private void m_btnMove_Click(object sender, EventArgs e)
		{
			CustomUI customUI = (CustomUI)base.Parent;
			customUI.MovingControl = this;
		}

		private void m_btnDelete_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this control?", "Deleting Control", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
			if (dialogResult != DialogResult.Yes)
			{
				return;
			}
			CustomUI customUI = (CustomUI)base.Parent;
			if (customUI != null)
			{
				customUI.Controls.Remove(this);
				customUI.Dirty = true;
				if (customUI.LuaState != null)
				{
					KillQuery(customUI.LuaState);
				}
			}
		}
	}
}
