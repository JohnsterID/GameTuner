using System.Collections.Generic;

namespace GameTuner.Framework.Export
{
	public class NullModelToolInterface : IModelManagerToolInterface
	{
		public override List<ModelGroupEntry> ModelGroupList
		{
			get
			{
				return new List<ModelGroupEntry>();
			}
			set
			{
			}
		}

		public override string DefaultGroupName
		{
			get
			{
				return "Default";
			}
		}

		public override List<string> MeshList
		{
			get
			{
				return new List<string>();
			}
		}

		public override void StartSelect()
		{
		}

		public override void SelectGroup(ModelGroupEntry kEntry)
		{
		}

		public override void EndSelect()
		{
		}

		public override void AddDefaultGroup()
		{
		}

		public override void AddOneGroupPerMesh()
		{
		}

		public override string GetModelName(ModelGroupEntry kEntry)
		{
			return kEntry.ModelGroupName + ".NULL";
		}

		public override string GetDataName(ModelGroupEntry kEntry)
		{
			return kEntry.ModelGroupName + ".data.NULL";
		}
	}
}
