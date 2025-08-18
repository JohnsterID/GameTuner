using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner.Framework.Controls
{
	public class ColorParameterEditor : UserControl, IParameterEditor
	{
		private string m_szName;

		private IContainer components;

		private TableLayoutPanel tableLayoutPanel1;

		private ColorButton ColorButton;

		private Label label1;

		string IParameterEditor.Name
		{
			get
			{
				return m_szName;
			}
			set
			{
				m_szName = value;
				label1.Text = m_szName;
			}
		}

		public string LabelText
		{
			get
			{
				return label1.Text;
			}
			set
			{
				label1.Text = value;
			}
		}

		public Color ColorValue
		{
			get
			{
				return ColorButton.Color;
			}
			set
			{
				ColorButton.Color = value;
				if (this.ValueChanged != null)
				{
					this.ValueChanged(this, EventArgs.Empty);
				}
			}
		}

		object IParameterEditor.Value
		{
			get
			{
				return ColorValue;
			}
			set
			{
				Color? color = null;
				if (value is Color)
				{
					color = (Color)value;
				}
				else if (value is float[])
				{
					float[] array = value as float[];
					color = Color.FromArgb(Convert.ToInt32(array[0] * 255f), Convert.ToInt32(array[1] * 255f), Convert.ToInt32(array[2] * 255f), Convert.ToInt32(array[3] * 255f));
				}
				if (color.HasValue)
				{
					ColorButton.Color = color.Value;
				}
			}
		}

		public event EventHandler ValueChanged;

		public ColorParameterEditor()
		{
			InitializeComponent();
			ColorValue = Color.Black;
		}

		private void ColorButton_ValueChanged(object sender, EventArgs e)
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged(this, EventArgs.Empty);
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.ColorButton = new GameTuner.Framework.Controls.ColorButton();
			this.label1 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 69f));
			this.tableLayoutPanel1.Controls.Add(this.ColorButton, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50f));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(232, 30);
			this.tableLayoutPanel1.TabIndex = 0;
			this.ColorButton.Color = System.Drawing.Color.Empty;
			this.ColorButton.Location = new System.Drawing.Point(168, 5);
			this.ColorButton.Margin = new System.Windows.Forms.Padding(5);
			this.ColorButton.Name = "ColorButton";
			this.ColorButton.Padding = new System.Windows.Forms.Padding(5);
			this.ColorButton.Size = new System.Drawing.Size(51, 20);
			this.ColorButton.TabIndex = 1;
			this.ColorButton.ValueChanged += new System.EventHandler(ColorButton_ValueChanged);
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "label1";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.tableLayoutPanel1);
			base.Name = "ColorParameterEditor";
			base.Size = new System.Drawing.Size(232, 30);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			base.ResumeLayout(false);
		}

		object IParameterEditor.Tag
		{
			get
			{
				return base.Tag;
			}
			set
			{
				base.Tag = value;
			}
		}
	}
}
