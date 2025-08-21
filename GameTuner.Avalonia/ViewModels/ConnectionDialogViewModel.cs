using System;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTuner.Core;

namespace GameTuner.Avalonia.ViewModels;

public partial class ConnectionDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _ipAddress = "127.0.0.1";

    [ObservableProperty]
    private int _port = 4318;

    [ObservableProperty]
    private bool _autoConnect = false;

    [ObservableProperty]
    private string _connectionStatus = "Not connected";

    [ObservableProperty]
    private IBrush _statusColor = Brushes.Gray;

    public bool DialogResult { get; private set; }

    public ConnectionDialogViewModel()
    {
        // Load settings from configuration
        var config = GameTunerConfig.Instance;
        IpAddress = config.GetValue("LastConnectionIP", "127.0.0.1");
        Port = config.GetValue("LastConnectionPort", 4318);
        AutoConnect = config.GetValue("AutoConnect", false);
        
        UpdateConnectionStatus();
    }

    [RelayCommand]
    private async Task TestConnection()
    {
        ConnectionStatus = "Testing...";
        StatusColor = Brushes.Orange;

        try
        {
            // Test connection without affecting the main connection
            var testConnection = Connection.Instance;
            var success = await testConnection.ConnectAsync(IpAddress, Port);
            
            if (success)
            {
                ConnectionStatus = "Connection successful";
                StatusColor = Brushes.Green;
                testConnection.Disconnect();
            }
            else
            {
                ConnectionStatus = "Connection failed";
                StatusColor = Brushes.Red;
            }
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Error: {ex.Message}";
            StatusColor = Brushes.Red;
        }
    }

    [RelayCommand]
    private void Ok()
    {
        // Save settings
        var config = GameTunerConfig.Instance;
        config.SetValue("LastConnectionIP", IpAddress);
        config.SetValue("LastConnectionPort", Port);
        config.SetValue("AutoConnect", AutoConnect);
        config.SaveSettings();

        DialogResult = true;
        // Close dialog - this would be handled by the view
    }

    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
        // Close dialog - this would be handled by the view
    }

    private void UpdateConnectionStatus()
    {
        if (Connection.Instance?.Connected == true)
        {
            ConnectionStatus = "Connected";
            StatusColor = Brushes.Green;
        }
        else
        {
            ConnectionStatus = "Not connected";
            StatusColor = Brushes.Gray;
        }
    }
}