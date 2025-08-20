using System.Reflection;
using System.Text.Json;
using System.Net.Sockets;
using System.Net;

namespace GameTuner.Core;

// Replacement for GameTuner.Framework.ErrorHandling
public static class ErrorHandling
{
    public static string AppName { get; set; } = "GameTuner";
    public static string AppVersion { get; set; } = "1.0.0";
    public static bool ShowErrorMessages { get; set; } = true;

    public static void Error(Exception ex, string message, ErrorLevel level)
    {
        // Simple console logging - can be replaced with proper logging framework
        Console.WriteLine($"[{level}] {AppName} v{AppVersion}: {message}");
        Console.WriteLine($"Exception: {ex}");
        
        // TODO: Implement proper logging (Serilog, NLog, etc.)
    }

    public static void CatchUnhandledExceptions()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Error((Exception)e.ExceptionObject, "Unhandled exception", ErrorLevel.SendReport);
        };
    }
}

public enum ErrorLevel
{
    ShowMessage,
    SendReport
}

// Replacement for GameTuner.Framework.ApplicationHelper
public static class ApplicationHelper
{
    public static string ProductVersion
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetName().Version?.ToString() ?? "1.0.0";
        }
    }

    public static string LocalUserCommonAppDataPath
    {
        get
        {
            // Cross-platform equivalent
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "GameTuner");
        }
    }
}

// Replacement for GameTuner.Framework.GenericObjectSerializer
public class GenericObjectSerializer
{
    public object? Data { get; private set; }

    public GenericObjectSerializer() { }

    public GenericObjectSerializer(object data)
    {
        Data = data;
    }

    // Simplified JSON-based serialization for cross-platform compatibility
    public string Serialize()
    {
        if (Data == null) return string.Empty;
        return JsonSerializer.Serialize(Data, new JsonSerializerOptions { WriteIndented = true });
    }

    public T? Deserialize<T>()
    {
        if (Data is string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        return default(T);
    }
}

// Simplified replacement for GameTuner.Framework.SocketConnection
public abstract class SocketConnection : IDisposable
{
    protected TcpClient? _tcpClient;
    protected NetworkStream? _stream;
    
    public IPEndPoint? ConnectionTarget { get; set; }
    public bool Connected => _tcpClient?.Connected ?? false;

    public event EventHandler<Exception>? ConnectionLost;

    public virtual async Task<bool> ConnectAsync()
    {
        if (ConnectionTarget == null) return false;

        try
        {
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(ConnectionTarget.Address, ConnectionTarget.Port);
            _stream = _tcpClient.GetStream();
            return true;
        }
        catch (Exception ex)
        {
            ErrorHandling.Error(ex, "Connection failed", ErrorLevel.ShowMessage);
            return false;
        }
    }

    public virtual void Disconnect()
    {
        _stream?.Close();
        _tcpClient?.Close();
        _stream = null;
        _tcpClient = null;
    }

    protected virtual void OnConnectionLost(Exception ex)
    {
        ConnectionLost?.Invoke(this, ex);
    }

    public void Dispose()
    {
        Disconnect();
    }
}
