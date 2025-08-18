using System.Collections.Generic;

namespace GameTuner.Framework.Export
{
	public class NullAnimationToolInterface : IAnimationManagerToolInterface
	{
		public override List<AnimationEntry> AnimationList
		{
			get
			{
				return new List<AnimationEntry>();
			}
			set
			{
			}
		}

		public override int StartFrame
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public override int EndFrame
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public override int CurrentFrame
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public override string DefaultExportSettings
		{
			get
			{
				return "Defalut";
			}
		}

		public override List<string> BoneList
		{
			get
			{
				return new List<string>();
			}
		}

		public override List<LayerEntry> LayerList
		{
			get
			{
				return new List<LayerEntry>();
			}
			set
			{
			}
		}

		public override float GetBoneWeight(string szLayerName, string szBoneName)
		{
			return 0f;
		}

		public override void SetBoneWeight(string szLayerName, string szBoneName, float fWeight)
		{
		}

		public override string GetAnimationFilename(AnimationEntry kEntry)
		{
			return kEntry.AnimationName + ".NULL";
		}
	}
}
