using CommunityToolkit.Mvvm.ComponentModel;

namespace GameTuner.Avalonia.ViewModels;

public partial class TabItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string header = string.Empty;

    [ObservableProperty]
    private object? content;

    public TabItemViewModel()
    {
    }

    public TabItemViewModel(string header, object? content = null)
    {
        Header = header;
        Content = content;
    }
}