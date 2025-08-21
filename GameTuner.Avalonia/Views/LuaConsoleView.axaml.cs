using Avalonia.Controls;
using Avalonia.Input;
using GameTuner.Avalonia.ViewModels;

namespace GameTuner.Avalonia.Views;

public partial class LuaConsoleView : UserControl
{
    public LuaConsoleView()
    {
        InitializeComponent();
    }

    private void CommandInput_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is LuaConsoleViewModel viewModel)
        {
            viewModel.ExecuteCommandCommand.Execute(null);
        }
    }
}