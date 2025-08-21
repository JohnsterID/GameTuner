using Avalonia.Controls;
using GameTuner.Avalonia.ViewModels;

namespace GameTuner.Avalonia.Views;

public partial class ConnectionDialog : Window
{
    public ConnectionDialog()
    {
        InitializeComponent();
        DataContext = new ConnectionDialogViewModel();
    }

    public ConnectionDialog(ConnectionDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}