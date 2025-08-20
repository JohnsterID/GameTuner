using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GameTuner.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string connectionStatus = "Disconnected";

    [ObservableProperty]
    private bool isConnected = false;

    public LuaConsoleViewModel LuaConsoleViewModel { get; } = new();
    public ActivePlayerViewModel ActivePlayerViewModel { get; } = new();
    public PanelCreatorViewModel PanelCreatorViewModel { get; } = new();

    public MainWindowViewModel()
    {
        // Initialize view models
        // Connection status change handler
        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ConnectionStatus))
        {
            IsConnected = ConnectionStatus == "Connected";
        }
    }

    // Menu Commands
    [RelayCommand]
    private void NewPanel()
    {
        // TODO: Implement new panel creation
    }

    [RelayCommand]
    private void OpenPanel()
    {
        // TODO: Implement panel opening
    }

    [RelayCommand]
    private void SavePanel()
    {
        // TODO: Implement panel saving
    }

    [RelayCommand]
    private void SavePanelAs()
    {
        // TODO: Implement save panel as
    }

    [RelayCommand]
    private void Exit()
    {
        // TODO: Implement application exit
        System.Environment.Exit(0);
    }

    [RelayCommand]
    private void ChangeConnection()
    {
        // TODO: Implement connection change dialog
        // For now, toggle connection for testing
        ConnectionStatus = ConnectionStatus == "Connected" ? "Disconnected" : "Connected";
    }

    [RelayCommand]
    private void ForceDisconnect()
    {
        // TODO: Implement force disconnect
        ConnectionStatus = "Disconnected";
    }

    [RelayCommand]
    private void RefreshLuaStates()
    {
        // TODO: Implement Lua states refresh
    }

    [RelayCommand]
    private void EditProjectPanels()
    {
        // TODO: Implement project panels editor
    }

    [RelayCommand]
    private void About()
    {
        // TODO: Implement about dialog
    }
}

// Placeholder view models for tab content
public partial class ActivePlayerViewModel : ViewModelBase
{
    [ObservableProperty]
    private string goldAmount = "0";

    [ObservableProperty]
    private string currentResearchId = "";

    [ObservableProperty]
    private string researchProgress = "0";

    // Tech Commands
    [RelayCommand]
    private void GrantAllTechs()
    {
        // TODO: Implement grant all techs
    }

    [RelayCommand]
    private void GrantAllTechsAllPlayers()
    {
        // TODO: Implement grant all techs for all players
    }

    [RelayCommand]
    private void RemoveAllTechs()
    {
        // TODO: Implement remove all techs
    }

    [RelayCommand]
    private void RemoveAllTechsAllPlayers()
    {
        // TODO: Implement remove all techs for all players
    }

    // Era Tech Commands
    [RelayCommand]
    private void AncientEraTechs()
    {
        // TODO: Implement ancient era techs
    }

    [RelayCommand]
    private void ClassicalEraTechs()
    {
        // TODO: Implement classical era techs
    }

    [RelayCommand]
    private void ClassicalEraTechsAllPlayers()
    {
        // TODO: Implement classical era techs for all players
    }

    [RelayCommand]
    private void MedievalEraTechs()
    {
        // TODO: Implement medieval era techs
    }

    [RelayCommand]
    private void RenaissanceEraTechs()
    {
        // TODO: Implement renaissance era techs
    }

    [RelayCommand]
    private void RenaissanceEraTechsAllPlayers()
    {
        // TODO: Implement renaissance era techs for all players
    }

    [RelayCommand]
    private void IndustrialEraTechs()
    {
        // TODO: Implement industrial era techs
    }

    [RelayCommand]
    private void ModernEraTechs()
    {
        // TODO: Implement modern era techs
    }

    [RelayCommand]
    private void FutureEraTechs()
    {
        // TODO: Implement future era techs
    }

    [RelayCommand]
    private void FutureEraTechsAllPlayers()
    {
        // TODO: Implement future era techs for all players
    }

    // Resource Commands
    [RelayCommand]
    private void Add1000Gold()
    {
        // TODO: Implement add 1000 gold
    }

    [RelayCommand]
    private void Player1DoW()
    {
        // TODO: Implement player 1 declaration of war
    }

    [RelayCommand]
    private void Add1000Culture()
    {
        // TODO: Implement add 1000 culture
    }

    [RelayCommand]
    private void Add100Faith()
    {
        // TODO: Implement add 100 faith
    }

    // Building Commands
    [RelayCommand]
    private void RecallBuildingMaint()
    {
        // TODO: Implement recall building maintenance
    }

    // Action Commands
    [RelayCommand]
    private void AddOneTech()
    {
        // TODO: Implement add one tech
    }
}

public partial class LuaConsoleViewModel : ViewModelBase
{
    [ObservableProperty]
    private string consoleOutput = "GameTuner Lua Console\nReady for commands...\n";

    [ObservableProperty]
    private string commandInput = string.Empty;
}

public partial class PanelCreatorViewModel : ViewModelBase
{
    [ObservableProperty]
    private string panelName = "New Panel";
}
