using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner.Framework.Controls
{
	public class FloatParameterEditor : UserControl, IParameterEditor
	{
		public const int MAX_ELEMENTS = 16;

		private Dictionary<Control, int> m_mapControlIndex = new Dictionary<Control, int>();

		private int m_iElementCount;

		private float[] m_afData;

		private IContainer components;

		private SplitContainer splitContainer1;

		private Label NameLabel;

		private TableLayoutPanel tableLayoutPanel1;

		private NumericUpDown[] Element = new NumericUpDown[16];

		public int ElementCount
		{
			get
			{
				return m_iElementCount;
			}
			set
			{
				m_iElementCount = Math.Min(16, value);
				for (int i = 0; i < 16; i++)
				{
					Element[i].Visible = i < m_iElementCount;
				}
				base.Size = new Size(base.Size.Width, 25 * Math.Max(1, m_iElementCount / 4));
			}
		}

		public float Increment
		{
			get
			{
				return Convert.ToSingle(Element[0].Increment);
			}
			set
			{
				NumericUpDown[] element = Element;
				foreach (NumericUpDown numericUpDown in element)
				{
					numericUpDown.Increment = Convert.ToDecimal(value);
				}
			}
		}

		string IParameterEditor.Name
		{
			get
			{
				return NameLabel.Text;
			}
			set
			{
				NameLabel.Text = value;
			}
		}

		object IParameterEditor.Value
		{
			get
			{
				return m_afData;
			}
			set
			{
				float[] array = value as float[];
				if (array != null)
				{
					array.CopyTo(m_afData, 0);
					for (int i = 0; i < Math.Min(array.Length, 16); i++)
					{
						Element[i].Value = Convert.ToDecimal(array[i]);
					}
				}
			}
		}

		public event EventHandler ValueChanged;

		public FloatParameterEditor()
		{
			InitializeComponent();
			Array.Resize(ref m_afData, 16);
			int num = 0;
			NumericUpDown[] element = Element;
			foreach (NumericUpDown numericUpDown in element)
			{
				numericUpDown.ValueChanged += HandleElementChange;
				m_mapControlIndex[numericUpDown] = num++;
			}
			Increment = 0.01f;
		}

		private void HandleElementChange(object sender, EventArgs e)
		{
			int value;
			if (m_mapControlIndex.TryGetValue(sender as Control, out value))
			{
				m_afData[value] = (float)Element[value].Value;
				if (this.ValueChanged != null)
				{
					this.ValueChanged(this, EventArgs.Empty);
				}
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.NameLabel = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddRows;
			this.Element[0] = new System.Windows.Forms.NumericUpDown();
			this.Element[1] = new System.Windows.Forms.NumericUpDown();
			this.Element[2] = new System.Windows.Forms.NumericUpDown();
			this.Element[3] = new System.Windows.Forms.NumericUpDown();
			this.Element[4] = new System.Windows.Forms.NumericUpDown();
			this.Element[5] = new System.Windows.Forms.NumericUpDown();
			this.Element[6] = new System.Windows.Forms.NumericUpDown();
			this.Element[7] = new System.Windows.Forms.NumericUpDown();
			this.Element[8] = new System.Windows.Forms.NumericUpDown();
			this.Element[9] = new System.Windows.Forms.NumericUpDown();
			this.Element[10] = new System.Windows.Forms.NumericUpDown();
			this.Element[11] = new System.Windows.Forms.NumericUpDown();
			this.Element[12] = new System.Windows.Forms.NumericUpDown();
			this.Element[13] = new System.Windows.Forms.NumericUpDown();
			this.Element[14] = new System.Windows.Forms.NumericUpDown();
			this.Element[15] = new System.Windows.Forms.NumericUpDown();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.Element[0]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[1]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[2]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[3]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[4]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[5]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[6]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[7]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[8]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[9]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[10]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[11]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[12]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[13]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[14]).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Element[15]).BeginInit();
			base.SuspendLayout();
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Panel1.Controls.Add(this.NameLabel);
			this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
			this.splitContainer1.Size = new System.Drawing.Size(386, 24);
			this.splitContainer1.SplitterDistance = 95;
			this.splitContainer1.TabIndex = 0;
			this.NameLabel.AutoSize = true;
			this.NameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.NameLabel.Location = new System.Drawing.Point(0, 0);
			this.NameLabel.Name = "NameLabel";
			this.NameLabel.Size = new System.Drawing.Size(86, 13);
			this.NameLabel.TabIndex = 0;
			this.NameLabel.Text = "Parameter Name";
			this.tableLayoutPanel1.Controls.Add(this.Element[0]);
			this.tableLayoutPanel1.Controls.Add(this.Element[1]);
			this.tableLayoutPanel1.Controls.Add(this.Element[2]);
			this.tableLayoutPanel1.Controls.Add(this.Element[3]);
			this.tableLayoutPanel1.Controls.Add(this.Element[4]);
			this.tableLayoutPanel1.Controls.Add(this.Element[5]);
			this.tableLayoutPanel1.Controls.Add(this.Element[6]);
			this.tableLayoutPanel1.Controls.Add(this.Element[7]);
			this.tableLayoutPanel1.Controls.Add(this.Element[8]);
			this.tableLayoutPanel1.Controls.Add(this.Element[9]);
			this.tableLayoutPanel1.Controls.Add(this.Element[10]);
			this.tableLayoutPanel1.Controls.Add(this.Element[11]);
			this.tableLayoutPanel1.Controls.Add(this.Element[12]);
			this.tableLayoutPanel1.Controls.Add(this.Element[13]);
			this.tableLayoutPanel1.Controls.Add(this.Element[14]);
			this.tableLayoutPanel1.Controls.Add(this.Element[15]);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Size = new System.Drawing.Size(287, 107);
			this.tableLayoutPanel1.TabIndex = 0;
			this.Element[0].DecimalPlaces = 3;
			this.Element[0].Location = new System.Drawing.Point(3, 3);
			this.Element[0].Name = "Element00";
			this.Element[0].Size = new System.Drawing.Size(65, 20);
			this.Element[0].TabIndex = 1;
			this.Element[1].DecimalPlaces = 3;
			this.Element[1].Location = new System.Drawing.Point(74, 3);
			this.Element[1].Name = "Element01";
			this.Element[1].Size = new System.Drawing.Size(65, 20);
			this.Element[1].TabIndex = 2;
			this.Element[2].DecimalPlaces = 3;
			this.Element[2].Location = new System.Drawing.Point(145, 3);
			this.Element[2].Name = "Element02";
			this.Element[2].Size = new System.Drawing.Size(65, 20);
			this.Element[2].TabIndex = 3;
			this.Element[3].DecimalPlaces = 3;
			this.Element[3].Location = new System.Drawing.Point(216, 3);
			this.Element[3].Name = "Element03";
			this.Element[3].Size = new System.Drawing.Size(65, 20);
			this.Element[3].TabIndex = 4;
			this.Element[4].DecimalPlaces = 3;
			this.Element[4].Location = new System.Drawing.Point(3, 29);
			this.Element[4].Name = "Element04";
			this.Element[4].Size = new System.Drawing.Size(65, 20);
			this.Element[4].TabIndex = 5;
			this.Element[5].DecimalPlaces = 3;
			this.Element[5].Location = new System.Drawing.Point(74, 29);
			this.Element[5].Name = "Element05";
			this.Element[5].Size = new System.Drawing.Size(65, 20);
			this.Element[5].TabIndex = 6;
			this.Element[6].DecimalPlaces = 3;
			this.Element[6].Location = new System.Drawing.Point(145, 29);
			this.Element[6].Name = "Element06";
			this.Element[6].Size = new System.Drawing.Size(65, 20);
			this.Element[6].TabIndex = 7;
			this.Element[7].DecimalPlaces = 3;
			this.Element[7].Location = new System.Drawing.Point(216, 29);
			this.Element[7].Name = "Element07";
			this.Element[7].Size = new System.Drawing.Size(65, 20);
			this.Element[7].TabIndex = 8;
			this.Element[8].DecimalPlaces = 3;
			this.Element[8].Location = new System.Drawing.Point(3, 55);
			this.Element[8].Name = "Element08";
			this.Element[8].Size = new System.Drawing.Size(65, 20);
			this.Element[8].TabIndex = 9;
			this.Element[9].DecimalPlaces = 3;
			this.Element[9].Location = new System.Drawing.Point(74, 55);
			this.Element[9].Name = "Element09";
			this.Element[9].Size = new System.Drawing.Size(65, 20);
			this.Element[9].TabIndex = 10;
			this.Element[10].DecimalPlaces = 3;
			this.Element[10].Location = new System.Drawing.Point(145, 55);
			this.Element[10].Name = "Element10";
			this.Element[10].Size = new System.Drawing.Size(65, 20);
			this.Element[10].TabIndex = 11;
			this.Element[11].DecimalPlaces = 3;
			this.Element[11].Location = new System.Drawing.Point(216, 55);
			this.Element[11].Name = "Element11";
			this.Element[11].Size = new System.Drawing.Size(65, 20);
			this.Element[11].TabIndex = 12;
			this.Element[12].DecimalPlaces = 3;
			this.Element[12].Location = new System.Drawing.Point(3, 81);
			this.Element[12].Name = "Element12";
			this.Element[12].Size = new System.Drawing.Size(65, 20);
			this.Element[12].TabIndex = 13;
			this.Element[13].DecimalPlaces = 3;
			this.Element[13].Location = new System.Drawing.Point(74, 81);
			this.Element[13].Name = "Element13";
			this.Element[13].Size = new System.Drawing.Size(65, 20);
			this.Element[13].TabIndex = 14;
			this.Element[14].DecimalPlaces = 3;
			this.Element[14].Location = new System.Drawing.Point(145, 81);
			this.Element[14].Name = "Element14";
			this.Element[14].Size = new System.Drawing.Size(65, 20);
			this.Element[14].TabIndex = 15;
			this.Element[15].DecimalPlaces = 3;
			this.Element[15].Location = new System.Drawing.Point(216, 81);
			this.Element[15].Name = "Element15";
			this.Element[15].Size = new System.Drawing.Size(65, 20);
			this.Element[15].TabIndex = 16;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.splitContainer1);
			base.Name = "FloatParameterEditor";
			base.Size = new System.Drawing.Size(386, 25);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.Element[0]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[1]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[2]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[3]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[4]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[5]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[6]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[7]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[8]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[9]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[10]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[11]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[12]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[13]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[14]).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Element[15]).EndInit();
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
