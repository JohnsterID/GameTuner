using CommunityToolkit.Mvvm.ComponentModel;
using GameTuner.Core;

namespace GameTuner.Avalonia.ViewModels;

public partial class DynamicPanelViewModel : ViewModelBase
{
    [ObservableProperty]
    private CustomUI _panel;

    public DynamicPanelViewModel(CustomUI panel)
    {
        _panel = panel;
    }
}