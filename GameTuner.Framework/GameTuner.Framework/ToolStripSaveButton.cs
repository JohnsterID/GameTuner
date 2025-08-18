using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
	public class ToolStripSaveButton : ToolStripSplitButton
	{
		private ToolStripMenuItem butSaveAs;

		private ToolStripMenuItem butSaveAll;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ImageTransparentColor
		{
			get
			{
				return base.ImageTransparentColor;
			}
			set
			{
				base.ImageTransparentColor = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ToolStripItemDisplayStyle DisplayStyle
		{
			get
			{
				return base.DisplayStyle;
			}
			set
			{
				base.DisplayStyle = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public new Image Image
		{
			get
			{
				return base.Image;
			}
			set
			{
				base.Image = value;
			}
		}

		[DefaultValue(true)]
		[Category("Behavior")]
		[Description("Set to True for the button support the Save As command")]
		public bool SupportsSaveAs { get; set; }

		[Description("Set to True for the button support the Save All command")]
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool SupportsSaveAll { get; set; }

		[Category("Action")]
		public event EventHandler CommandSavePressed;

		[Category("Action")]
		public event EventHandler CommandSaveAsPressed;

		[Category("Action")]
		public event EventHandler CommandSaveAllPressed;

		public ToolStripSaveButton()
		{
			InitializeComponent();
			DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			Image = GameTuner.Framework.Properties.Resources.save;
			Text = "Save";
			SupportsSaveAs = true;
			SupportsSaveAll = false;
		}

		private void InitializeComponent()
		{
			butSaveAs = new ToolStripMenuItem();
			butSaveAll = new ToolStripMenuItem();
			butSaveAs.Image = GameTuner.Framework.Properties.Resources.save;
			butSaveAs.Name = "butSaveAs";
			butSaveAs.Size = new Size(123, 22);
			butSaveAs.Text = "Save As...";
			butSaveAs.Click += butSaveAs_Click;
			butSaveAll.Image = GameTuner.Framework.Properties.Resources.save_all;
			butSaveAll.Name = "butSaveAll";
			butSaveAll.Size = new Size(123, 22);
			butSaveAll.Text = "Save All";
			butSaveAll.Click += butSaveAll_Click;
			base.DropDownItems.AddRange(new ToolStripItem[2] { butSaveAs, butSaveAll });
			base.Click += ToolStripSaveButton_Click;
			base.ButtonClick += ToolStripSaveButton_ButtonClick;
			base.DropDownOpening += ToolStripSaveButton_DropDownOpening;
		}

		private void ToolStripSaveButton_Click(object sender, EventArgs e)
		{
		}

		private void butSaveAs_Click(object sender, EventArgs e)
		{
			EventHandler commandSaveAsPressed = this.CommandSaveAsPressed;
			if (commandSaveAsPressed != null)
			{
				commandSaveAsPressed(this, e);
			}
		}

		private void butSaveAll_Click(object sender, EventArgs e)
		{
			EventHandler commandSaveAllPressed = this.CommandSaveAllPressed;
			if (commandSaveAllPressed != null)
			{
				commandSaveAllPressed(this, e);
			}
		}

		private void ToolStripSaveButton_ButtonClick(object sender, EventArgs e)
		{
			EventHandler commandSavePressed = this.CommandSavePressed;
			if (commandSavePressed != null)
			{
				commandSavePressed(this, e);
			}
		}

		private void ToolStripSaveButton_DropDownOpening(object sender, EventArgs e)
		{
			butSaveAs.Visible = SupportsSaveAs;
			butSaveAll.Visible = SupportsSaveAll;
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				ShowDropDown();
			}
			base.OnMouseUp(e);
		}
	}
}
