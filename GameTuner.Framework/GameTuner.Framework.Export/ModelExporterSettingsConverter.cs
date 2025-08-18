using System.ComponentModel;

namespace GameTuner.Framework.Export
{
	public class ModelExporterSettingsConverter : StringConverter
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
			return new StandardValuesCollection(MESGlobalVars.ModelExportSettings);
		}
	}
}
