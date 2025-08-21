using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTuner.Core;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace GameTuner.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string connectionStatus = "Disconnected";

    [ObservableProperty]
    private bool isConnected = false;

    public ObservableCollection<TabItemViewModel> Tabs { get; } = new();

    [ObservableProperty]
    private TabItemViewModel? selectedTab;

    public LuaConsoleViewModel LuaConsoleViewModel { get; } = new();
    public ActivePlayerViewModel ActivePlayerViewModel { get; } = new();
    public PanelCreatorViewModel PanelCreatorViewModel { get; } = new();

    public MainWindowViewModel()
    {
        // Initialize view models
        // Connection status change handler
        PropertyChanged += OnPropertyChanged;
        InitializeConnection();
        InitializeTabs();
    }

    private void InitializeTabs()
    {
        // Add permanent tabs
        Tabs.Add(new TabItemViewModel("Lua Console", new Views.LuaConsoleView { DataContext = LuaConsoleViewModel }));
        Tabs.Add(new TabItemViewModel("* New Panel *", new Views.PanelCreatorView { DataContext = PanelCreatorViewModel }));
    }

    private void InitializeConnection()
    {
        Connection.Init();
        if (Connection.Instance != null)
        {
            Connection.Instance.ConnectionLost += OnConnectionLost;
            UpdateConnectionStatus();
        }
    }

    private void OnConnectionLost(object? sender, Exception e)
    {
        IsConnected = false;
        ConnectionStatus = "Disconnected";
        // TODO: Show error message
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsConnected))
        {
            ConnectionStatus = IsConnected ? "Connected" : "Disconnected";
        }
    }

    private void UpdateConnectionStatus()
    {
        IsConnected = Connection.Instance?.Connected ?? false;
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
    private async Task ChangeConnection()
    {
        var dialogViewModel = new ConnectionDialogViewModel();
        var dialog = new Views.ConnectionDialog(dialogViewModel);
        
        // Show dialog (in a real implementation, this would be handled differently)
        // For now, just use the current settings
        var config = GameTunerConfig.Instance;
        var ip = config.GetValue("LastConnectionIP", "127.0.0.1");
        var port = config.GetValue("LastConnectionPort", 4318);
        
        if (Connection.Instance != null)
        {
            var success = await Connection.Instance.ConnectAsync(ip, port);
            if (success)
            {
                IsConnected = true;
                ConnectionStatus = "Connected";
            }
            else
            {
                ConnectionStatus = "Connection failed";
            }
        }
    }

    [RelayCommand]
    private void ForceDisconnect()
    {
        Connection.Instance?.Disconnect();
        IsConnected = false;
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
    private string consoleOutput = string.Empty;

    [ObservableProperty]
    private string commandInput = string.Empty;

    [ObservableProperty]
    private ObservableCollection<LuaState> luaStates = new();

    [ObservableProperty]
    private LuaState? selectedLuaState;

    public LuaConsoleViewModel()
    {
        // Initialize with dynamic greeting
        ConsoleOutput = GetTimeBasedGreeting() + "\n";
        
        // Subscribe to connection events
        if (Connection.Instance != null)
        {
            Connection.Instance.OnOutputMsgReceived += OnOutputReceived;
        }

        // Add some sample Lua states for testing
        LuaStates.Add(new LuaState("Game", 1));
        LuaStates.Add(new LuaState("UI", 2));
        SelectedLuaState = LuaStates.FirstOrDefault();
    }

    private string GetTimeBasedGreeting()
    {
        DateTime now = DateTime.Now;
        bool inHouse = false; // TODO: Implement UserInfo.InHouse logic if needed
        
        if (!inHouse)
        {
            return now.Hour switch
            {
                < 5 => "Hang in there!",
                < 11 => "Good Morning",
                < 13 => "Hello Master",
                < 15 => "Good Afternoon",
                < 18 => "Hi!  Nice to see you again.",
                >= 20 => "Working late?",
                _ => "Good Evening"
            };
        }
        else
        {
            // InHouse version with different messages
            string firstName = "User"; // TODO: Get actual user first name
            return now.Hour switch
            {
                < 5 => "Hang in there!",
                < 11 => "Good Morning",
                < 13 => "Hello Master",
                < 15 => "Let's play Global Thermonuclear War",
                < 18 => $"I'm sorry, {firstName}. I'm afraid I can't do that.",
                < 20 => $"{firstName}, my mind is going. I can feel it. I can feel it. My mind is going.",
                >= 21 => "Working late?",
                _ => "Daisy, Daisy, give me your answer do."
            };
        }
    }

    private void OnOutputReceived(string output)
    {
        ConsoleOutput += output + "\n";
    }

    [RelayCommand]
    private async Task ExecuteCommand()
    {
        if (string.IsNullOrWhiteSpace(CommandInput) || Connection.Instance == null)
            return;

        var command = CommandInput.Trim();
        ConsoleOutput += $"> {command}\n";

        try
        {
            var success = await Connection.Instance.SendCommandAsync(command);
            if (!success)
            {
                ConsoleOutput += "Failed to send command - not connected\n";
            }
        }
        catch (Exception ex)
        {
            ConsoleOutput += $"Error: {ex.Message}\n";
        }

        CommandInput = string.Empty;
    }

    [RelayCommand]
    private void ClearConsole()
    {
        ConsoleOutput = GetTimeBasedGreeting() + "\n";
    }


}

public partial class PanelCreatorViewModel : ViewModelBase
{
    [ObservableProperty]
    private string panelName = "New Panel";
}
