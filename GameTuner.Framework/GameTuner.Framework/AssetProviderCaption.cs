using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.IO;

namespace GameTuner.Framework
{
	public class AssetProviderCaption : CaptionControl
	{
		private IAssetTarget assetProvider;

		private LinkedAssetsButton btnLinkedAssets;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public IAssetTarget AssetProvider
		{
			get
			{
				return assetProvider;
			}
			set
			{
				assetProvider = value;
				btnLinkedAssets.AssetProvider = assetProvider;
			}
		}

		public AssetProviderCaption()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.btnLinkedAssets = new GameTuner.Framework.IO.LinkedAssetsButton();
			base.SuspendLayout();
			this.btnLinkedAssets.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnLinkedAssets.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnLinkedAssets.ForeColor = System.Drawing.SystemColors.Highlight;
			this.btnLinkedAssets.Location = new System.Drawing.Point(340, 0);
			this.btnLinkedAssets.Name = "btnLinkedAssets";
			this.btnLinkedAssets.Size = new System.Drawing.Size(98, 27);
			this.btnLinkedAssets.TabIndex = 0;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			base.Controls.Add(this.btnLinkedAssets);
			this.DoubleBuffered = false;
			base.Name = "AssetProviderCaption";
			base.Size = new System.Drawing.Size(438, 27);
			base.ResumeLayout(false);
		}
	}
}
