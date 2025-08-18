using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public class ShowOnlyOnePanel : Panel
	{
		private Control active;

		private IContainer components;

		[Browsable(false)]
		public Control ActiveControl
		{
			get
			{
				return active;
			}
			set
			{
				foreach (Control control in base.Controls)
				{
					control.Dock = DockStyle.Fill;
					control.Visible = control == value;
				}
				active = value;
				if (active == null && base.Controls.Count > 0)
				{
					ActiveControl = base.Controls[0];
					return;
				}
				EventHandler activeControlChanged = this.ActiveControlChanged;
				if (activeControlChanged != null)
				{
					activeControlChanged(this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler ActiveControlChanged;

		public ShowOnlyOnePanel()
		{
			InitializeComponent();
		}

		public void HideControls()
		{
			foreach (Control control in base.Controls)
			{
				control.Dock = DockStyle.Fill;
				control.Visible = false;
			}
			active = null;
		}

		public bool HasControl<T>()
		{
			foreach (Control control in base.Controls)
			{
				if (typeof(T).IsInstanceOfType(control))
				{
					return true;
				}
			}
			return false;
		}

		public void ActivateControl<T>()
		{
			foreach (Control control in base.Controls)
			{
				if (typeof(T).IsInstanceOfType(control))
				{
					ActiveControl = control;
					break;
				}
			}
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			Control control = e.Control;
			control.Dock = DockStyle.Fill;
			if (!Context.InDesignMode)
			{
				control.Hide();
			}
			base.OnControlAdded(e);
		}

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			if (!Context.InDesignMode && active == e.Control)
			{
				ActiveControl = null;
			}
			base.OnControlRemoved(e);
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
		}
	}
}
