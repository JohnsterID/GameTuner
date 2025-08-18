using System.Collections.Generic;

namespace GameTuner.Framework.Layout
{
	public interface ILayoutManager
	{
		Factory<ILayoutWindow> LayoutFactory { get; }

		LayoutWindowCollection Layouts { get; }

		IEnumerable<string> Perspectives { get; }

		void SavePerspectives(string name);

		void LoadPerspectives(string name);
	}
}
