using System.Collections.Generic;

namespace GameTuner.Framework.Export
{
	public abstract class IModelManagerToolInterface
	{
		public abstract List<ModelGroupEntry> ModelGroupList { get; set; }

		public abstract string DefaultGroupName { get; }

		public abstract List<string> MeshList { get; }

		public abstract void StartSelect();

		public abstract void SelectGroup(ModelGroupEntry kEntry);

		public abstract void EndSelect();

		public abstract void AddDefaultGroup();

		public abstract void AddOneGroupPerMesh();

		public abstract string GetModelName(ModelGroupEntry kEntry);

		public abstract string GetDataName(ModelGroupEntry kEntry);
	}
}
