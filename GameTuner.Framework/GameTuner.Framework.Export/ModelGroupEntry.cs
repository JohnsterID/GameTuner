using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace GameTuner.Framework.Export
{
	public class ModelGroupEntry
	{
		public List<string> Meshes;

		[Category("Identification")]
		[Description("The name of the model group. This will be basis for the output filenames.")]
		public string ModelGroupName { get; set; }

		[Category("Identification")]
		[Description("The filename that will contain the model data for this group.")]
		public string ModelFilename
		{
			get
			{
				return Interface.GetModelName(this);
			}
		}

		[Description("The filename that will contain the metadata for this group.")]
		[Category("Identification")]
		public string DataFilename
		{
			get
			{
				return Interface.GetDataName(this);
			}
		}

		[Description("The settings used to export this model.")]
		[TypeConverter(typeof(ModelExporterSettingsConverter))]
		[Category("Export")]
		public string ExportSettings { get; set; }

		[Category("Export")]
		[Description("Whether this model group is animated.")]
		public bool UsesAnimations { get; set; }

		public IModelManagerToolInterface Interface { get; private set; }

		public ModelGroupEntry(IModelManagerToolInterface kIFace)
		{
			Meshes = new List<string>();
			Interface = kIFace;
		}

		public ModelGroupEntry(ModelGroupEntry kCopy)
		{
			ModelGroupName = kCopy.ModelGroupName;
			ExportSettings = kCopy.ExportSettings;
			Meshes = kCopy.Meshes.ToList();
			Interface = kCopy.Interface;
		}
	}
}
