using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using GameTuner.Framework;

namespace GameTuner
{
	public class ActionButton : Button, ICustomControl
	{
		private ContextMenuStrip m_mnuContext = new ContextMenuStrip();

		private ToolStripMenuItem m_btnEdit = new ToolStripMenuItem("Edit");

		private ToolStripMenuItem m_btnCopy = new ToolStripMenuItem("Copy");

		private ToolStripMenuItem m_btnMove = new ToolStripMenuItem("Move");

		private ToolStripMenuItem m_btnDelete = new ToolStripMenuItem("Delete");

		public ActionButton()
		{
			BackColor = SystemColors.Control;
			ForeColor = SystemColors.ControlText;
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
		}

		public void Release()
		{
			Connection.Instance.RemoveRequestListener(CompletedAction);
		}

		public void LuaStateChanged(LuaState state, LuaState lastState)
		{
		}

		public void CompletedAction(List<string> luaMessages)
		{
			if (luaMessages.Count > 0 && luaMessages[0].StartsWith("ERR:"))
			{
				BackColor = Color.Red;
			}
			else
			{
				BackColor = SystemColors.Control;
			}
		}

		public void StartDrag()
		{
		}

		public void EndDrag()
		{
		}

		public void TabEntered()
		{
		}

		public void TabLeft()
		{
		}

		private void m_btnEdit_Click(object sender, EventArgs e)
		{
			CustomUI customUI = (CustomUI)base.Parent;
			customUI.EditAction(this);
		}

		private void m_btnCopy_Click(object sender, EventArgs e)
		{
			StringWriter stringWriter = new StringWriter();
			try
			{
				CustomUI.PanelData.ActionData data = new CustomUI.PanelData.ActionData(this);
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
			if (dialogResult == DialogResult.Yes)
			{
				CustomUI customUI = (CustomUI)base.Parent;
				customUI.Controls.Remove(this);
				customUI.Dirty = true;
			}
		}
	}
}
