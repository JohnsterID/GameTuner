using System.ComponentModel;

namespace GameTuner.Framework.Export
{
	public class AnimExporterSettingsConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(AESGlobalVars.AnimExportSettings);
		}
	}
}
