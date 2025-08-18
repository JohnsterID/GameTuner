using System.Windows.Forms;

namespace GameTuner.Framework
{
	public interface IPropertyGridDescriptor
	{
		void OnEnterDescriptor(PropertyGrid propertyGrid);

		void OnPropertyChanged(PropertyGrid propertyGrid, PropertyValueChangedEventArgs e);
	}
}
