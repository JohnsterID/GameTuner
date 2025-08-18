using System.Collections.Generic;

namespace GameTuner.Framework.Layout
{
	public class LayoutManager : ILayoutManager
	{
		private List<string> perspectives;

		public Factory<ILayoutWindow> LayoutFactory { get; private set; }

		public LayoutWindowCollection Layouts { get; private set; }

		public IEnumerable<string> Perspectives
		{
			get
			{
				foreach (string perspective in perspectives)
				{
					yield return perspective;
				}
			}
		}

		public LayoutManager()
		{
			LayoutFactory = new Factory<ILayoutWindow>();
			Layouts = new LayoutWindowCollection();
			perspectives = new List<string>();
		}

		public void SavePerspectives(string name)
		{
		}

		public void LoadPerspectives(string name)
		{
		}
	}
}
