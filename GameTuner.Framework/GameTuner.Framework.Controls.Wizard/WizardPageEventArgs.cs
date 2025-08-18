using System;

namespace GameTuner.Framework.Controls.Wizard
{
	public class WizardPageEventArgs : EventArgs
	{
		public WizardPage Page { get; private set; }

		public WizardPageEventArgs(WizardPage page)
		{
			Page = page;
		}
	}
}
