using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
	public class ToolStripOpenButton : ToolStripSplitButton
	{
		private ToolStripMenuItem butVirtOpen;

		private ToolStripMenuItem butNew;

		[Category("Behavior")]
		[DefaultValue(true)]
		[Description("Set to True for the button support the New command")]
		public bool SupportsNew { get; set; }

		[DefaultValue(true)]
		[Category("Behavior")]
		[Description("Set to True for the button support the Virtual open command")]
		public bool SupportsVirtualOpen { get; set; }

		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Set to True for the Open By File dialog to support multiselect")]
		public bool SupportsMultiSelect { get; set; }

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

		[Category("Action")]
		public event EventHandler CommandNewPressed;

		[Category("Action")]
		public event EventHandler CommandOpenPressed;

		[Category("Action")]
		public event EventHandler CommandOpenByFile;

		public ToolStripOpenButton()
		{
			InitializeComponent();
			DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			Image = GameTuner.Framework.Properties.Resources.dir_closed;
			Text = "Open";
			SupportsNew = true;
			SupportsVirtualOpen = true;
		}

		private void InitializeComponent()
		{
			butNew = new ToolStripMenuItem();
			butVirtOpen = new ToolStripMenuItem();
			butNew.Image = GameTuner.Framework.Properties.Resources.file_document;
			butNew.Name = "butNew";
			butNew.Size = new Size(149, 22);
			butNew.Text = "New";
			butNew.Click += butNew_Click;
			butVirtOpen.Image = GameTuner.Framework.Properties.Resources.import_asset;
			butVirtOpen.Name = "butVirtOpen";
			butVirtOpen.Size = new Size(149, 22);
			butVirtOpen.Text = "Open by File...";
			butVirtOpen.Click += butVirtOpen_Click;
			base.DropDownItems.AddRange(new ToolStripItem[2] { butNew, butVirtOpen });
			base.ButtonClick += ToolStripOpenButton_ButtonClick;
			base.DropDownOpening += ToolStripOpenButton_DropDownOpening;
		}

		private void butVirtOpen_Click(object sender, EventArgs e)
		{
			if (!Context.InDesignMode)
			{
				EventHandler commandOpenByFile = this.CommandOpenByFile;
				if (commandOpenByFile != null)
				{
					commandOpenByFile(this, e);
				}
			}
		}

		private void butNew_Click(object sender, EventArgs e)
		{
			if (!Context.InDesignMode)
			{
				EventHandler commandNewPressed = this.CommandNewPressed;
				if (commandNewPressed != null)
				{
					commandNewPressed(this, e);
				}
			}
		}

		private void ToolStripOpenButton_DropDownOpening(object sender, EventArgs e)
		{
			butNew.Visible = SupportsNew;
			butVirtOpen.Visible = SupportsVirtualOpen;
		}

		private void ToolStripOpenButton_ButtonClick(object sender, EventArgs e)
		{
			EventHandler commandOpenPressed = this.CommandOpenPressed;
			if (commandOpenPressed != null)
			{
				commandOpenPressed(this, e);
			}
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
