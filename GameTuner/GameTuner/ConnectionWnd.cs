using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace GameTuner
{
	public class ConnectionWnd : Form
	{
		private IPAddress m_LocalHost = IPAddress.Parse("127.0.0.1");

		private IContainer components;

		private GroupBox grpAddress;

		private TextBox txtIPAddr;

		private Label lblIP;

		private Label lblPort;

		private TextBox txtPort;

		private Button btnConnect;

		private Button btnCancel;

		public IPAddress IP
		{
			get
			{
				IPAddress address = m_LocalHost;
				IPAddress.TryParse(txtIPAddr.Text, out address);
				return address;
			}
			set
			{
				txtIPAddr.Text = value.ToString();
			}
		}

		public int Port
		{
			get
			{
				return int.Parse(txtPort.Text);
			}
			set
			{
				txtPort.Text = value.ToString();
			}
		}

		public ConnectionWnd(string sIP, int iPort)
		{
			InitializeComponent();
			IPAddress address = m_LocalHost;
			IPAddress.TryParse(sIP, out address);
			IP = address;
			Port = iPort;
		}

		public ConnectionWnd(IPAddress ip, int iPort)
		{
			InitializeComponent();
			IP = ip;
			Port = iPort;
		}

		private void txtIPAddr_TextChanged(object sender, EventArgs e)
		{
			IPAddress address = m_LocalHost;
			if (IPAddress.TryParse(txtIPAddr.Text, out address))
			{
				txtIPAddr.BackColor = Color.White;
			}
			else
			{
				txtIPAddr.BackColor = Color.Red;
			}
		}

		private void txtPort_TextChanged(object sender, EventArgs e)
		{
			ushort result;
			ushort.TryParse(txtPort.Text, out result);
			txtPort.Text = result.ToString();
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
			this.grpAddress = new System.Windows.Forms.GroupBox();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.lblPort = new System.Windows.Forms.Label();
			this.lblIP = new System.Windows.Forms.Label();
			this.txtIPAddr = new System.Windows.Forms.TextBox();
			this.btnConnect = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.grpAddress.SuspendLayout();
			base.SuspendLayout();
			this.grpAddress.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.grpAddress.Controls.Add(this.txtPort);
			this.grpAddress.Controls.Add(this.lblPort);
			this.grpAddress.Controls.Add(this.lblIP);
			this.grpAddress.Controls.Add(this.txtIPAddr);
			this.grpAddress.Location = new System.Drawing.Point(0, 5);
			this.grpAddress.Name = "grpAddress";
			this.grpAddress.Size = new System.Drawing.Size(235, 51);
			this.grpAddress.TabIndex = 0;
			this.grpAddress.TabStop = false;
			this.grpAddress.Text = "Address";
			this.txtPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtPort.Location = new System.Drawing.Point(175, 16);
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(53, 22);
			this.txtPort.TabIndex = 3;
			this.txtPort.TextChanged += new System.EventHandler(txtPort_TextChanged);
			this.lblPort.AutoSize = true;
			this.lblPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.lblPort.Location = new System.Drawing.Point(138, 19);
			this.lblPort.Name = "lblPort";
			this.lblPort.Size = new System.Drawing.Size(35, 16);
			this.lblPort.TabIndex = 2;
			this.lblPort.Text = "Port:";
			this.lblIP.AutoSize = true;
			this.lblIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.lblIP.Location = new System.Drawing.Point(6, 19);
			this.lblIP.Name = "lblIP";
			this.lblIP.Size = new System.Drawing.Size(23, 16);
			this.lblIP.TabIndex = 1;
			this.lblIP.Text = "IP:";
			this.txtIPAddr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtIPAddr.Location = new System.Drawing.Point(35, 16);
			this.txtIPAddr.Name = "txtIPAddr";
			this.txtIPAddr.Size = new System.Drawing.Size(97, 22);
			this.txtIPAddr.TabIndex = 0;
			this.txtIPAddr.TextChanged += new System.EventHandler(txtIPAddr_TextChanged);
			this.btnConnect.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnConnect.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnConnect.Location = new System.Drawing.Point(99, 62);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(64, 33);
			this.btnConnect.TabIndex = 4;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(169, 62);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 33);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			base.AcceptButton = this.btnConnect;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new System.Drawing.Size(237, 98);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnConnect);
			base.Controls.Add(this.grpAddress);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ConnectionWnd";
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Tuner Connect";
			this.grpAddress.ResumeLayout(false);
			this.grpAddress.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
