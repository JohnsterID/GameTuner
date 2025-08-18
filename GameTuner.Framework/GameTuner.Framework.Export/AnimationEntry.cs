using System.ComponentModel;

namespace GameTuner.Framework.Export
{
	public class AnimationEntry
	{
		[Description("The name of the animation.")]
		[Category("Identification")]
		public string AnimationName { get; set; }

		[Category("Identification")]
		[Description("The filename that will contain the animation data for this group.")]
		public string AnimationFilename
		{
			get
			{
				return Interface.GetAnimationFilename(this);
			}
		}

		[Description("An identifier code which is used by the engine to determine animation type.")]
		[Category("Identification")]
		public int EventCode { get; set; }

		[Category("Range")]
		[Description("The start frame of the animation.")]
		public int StartFrame { get; set; }

		[Category("Range")]
		[Description("The end frame of the animation.")]
		public int EndFrame { get; set; }

		[Description("The settings used to export this animation.")]
		[TypeConverter(typeof(AnimExporterSettingsConverter))]
		[Category("Export")]
		public string ExportSettings { get; set; }

		public IAnimationManagerToolInterface Interface { get; private set; }

		public AnimationEntry(IAnimationManagerToolInterface kIFace)
		{
			Interface = kIFace;
		}

		public AnimationEntry(AnimationEntry kCopy)
		{
			AnimationName = kCopy.AnimationName;
			EventCode = kCopy.EventCode;
			StartFrame = kCopy.StartFrame;
			EndFrame = kCopy.EndFrame;
			ExportSettings = kCopy.ExportSettings;
			Interface = kCopy.Interface;
		}
	}
}
