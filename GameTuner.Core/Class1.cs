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

// Migrated from original GameTuner project
public class LuaState
{
    private readonly string _name;
    private readonly uint _id;

    public string Name => _name;
    public uint ID => _id;

    public LuaState(string name, uint id)
    {
        _name = name;
        _id = id;
    }

    public override string ToString() => _name;
}

// Interface for custom controls (already exists in original GameTuner project)
public interface ICustomControl
{
    void Release();
    void LuaStateChanged(LuaState state, LuaState lastState);
    void CompletedAction(List<string> luaMessages);
    void StartDrag();
    void EndDrag();
    void TabEntered();
    void TabLeft();
}

// Modern cross-platform Connection class
public class Connection : SocketConnection
{
    public delegate void OutputMsgHandler(string outputMsg);
    public delegate void RequestListener(List<string> response);

    private readonly byte[] _readBuffer = new byte[1048576];
    private int _readBufferOffset;
    private byte[]? _message;
    private int _messageIndex;
    private readonly object _responseLock = new object();
    private readonly List<RequestResponse> _responsesToProcess = new List<RequestResponse>();
    private readonly List<RequestResponse> _responses = new List<RequestResponse>();
    private bool _routingMessages;
    private RequestListener? _defaultRequestHandler;
    private readonly List<RequestListener> _requestListeners = new List<RequestListener>();
    private readonly Dictionary<RequestListener, int> _requestListenersIndexMap = new Dictionary<RequestListener, int>();

    public static Connection? Instance { get; private set; }

    public event OutputMsgHandler? OnOutputMsgReceived;

    private struct RequestResponse
    {
        public int Listener;
        public List<string> Messages;
    }

    public static void Init()
    {
        Instance?.Disconnect();
        Instance = new Connection();
    }

    private Connection()
    {
        ConnectionTarget = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4318);
        _defaultRequestHandler = DefaultRequestHandler;
    }

    public async Task<bool> ConnectAsync(IPAddress ip, int port)
    {
        ConnectionTarget = new IPEndPoint(ip, port);
        var result = await base.ConnectAsync();
        if (result)
        {
            _ = Task.Run(ListenForMessagesAsync);
        }
        return result;
    }

    public async Task<bool> ConnectAsync(string ip, int port)
    {
        return await ConnectAsync(IPAddress.Parse(ip), port);
    }

    private async Task ListenForMessagesAsync()
    {
        if (_stream == null) return;

        try
        {
            while (Connected)
            {
                var buffer = new byte[4096];
                var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                
                if (bytesRead == 0)
                {
                    OnConnectionLost(new Exception("Connection closed by remote host"));
                    break;
                }

                ProcessReceivedData(buffer, bytesRead);
            }
        }
        catch (Exception ex)
        {
            OnConnectionLost(ex);
        }
    }

    private void ProcessReceivedData(byte[] buffer, int bytesRead)
    {
        // Copy received data to read buffer
        Array.Copy(buffer, 0, _readBuffer, _readBufferOffset, bytesRead);
        _readBufferOffset += bytesRead;

        // Process complete messages
        ProcessMessages();
    }

    private void ProcessMessages()
    {
        // Simple message processing - in real implementation would parse protocol
        // For now, just convert to string and trigger output event
        if (_readBufferOffset > 0)
        {
            var message = System.Text.Encoding.UTF8.GetString(_readBuffer, 0, _readBufferOffset);
            OnOutputMsgReceived?.Invoke(message);
            _readBufferOffset = 0;
        }
    }

    public async Task<bool> SendCommandAsync(string command)
    {
        if (!Connected || _stream == null) return false;

        try
        {
            var data = System.Text.Encoding.UTF8.GetBytes(command + "\n");
            await _stream.WriteAsync(data, 0, data.Length);
            await _stream.FlushAsync();
            return true;
        }
        catch (Exception ex)
        {
            ErrorHandling.Error(ex, "Failed to send command", ErrorLevel.ShowMessage);
            return false;
        }
    }

    public int AddRequestListener(RequestListener listener)
    {
        lock (_responseLock)
        {
            _requestListeners.Add(listener);
            var index = _requestListeners.Count - 1;
            _requestListenersIndexMap[listener] = index;
            return index;
        }
    }

    public void RemoveRequestListener(RequestListener listener)
    {
        lock (_responseLock)
        {
            if (_requestListenersIndexMap.TryGetValue(listener, out var index))
            {
                _requestListeners.RemoveAt(index);
                _requestListenersIndexMap.Remove(listener);
            }
        }
    }

    private void DefaultRequestHandler(List<string> response)
    {
        // Default handling for responses
        foreach (var message in response)
        {
            OnOutputMsgReceived?.Invoke(message);
        }
    }
}

// Cross-platform configuration system (replaces Windows Registry)
public class GameTunerConfig
{
    private readonly string _configPath;
    private Dictionary<string, object> _settings;

    public static GameTunerConfig Instance { get; } = new GameTunerConfig();

    private GameTunerConfig()
    {
        var appDataPath = ApplicationHelper.LocalUserCommonAppDataPath;
        Directory.CreateDirectory(appDataPath);
        _configPath = Path.Combine(appDataPath, "config.json");
        _settings = LoadSettings();
    }

    private Dictionary<string, object> LoadSettings()
    {
        try
        {
            if (File.Exists(_configPath))
            {
                var json = File.ReadAllText(_configPath);
                return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
            }
        }
        catch (Exception ex)
        {
            ErrorHandling.Error(ex, "Failed to load configuration", ErrorLevel.ShowMessage);
        }
        return new Dictionary<string, object>();
    }

    public void SaveSettings()
    {
        try
        {
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);
        }
        catch (Exception ex)
        {
            ErrorHandling.Error(ex, "Failed to save configuration", ErrorLevel.ShowMessage);
        }
    }

    public T GetValue<T>(string key, T defaultValue = default(T))
    {
        if (_settings.TryGetValue(key, out var value))
        {
            try
            {
                if (value is JsonElement jsonElement)
                {
                    return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
                }
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
        return defaultValue;
    }

    public void SetValue<T>(string key, T value)
    {
        _settings[key] = value;
    }

    public void RemoveValue(string key)
    {
        _settings.Remove(key);
    }

    public bool HasValue(string key)
    {
        return _settings.ContainsKey(key);
    }
}

// Panel configuration data structures (migrated from original)
public class PanelInfo
{
    public bool LockLuaState { get; set; }
    public string ConsoleLuaState { get; set; } = string.Empty;
    public int SelectedPanel { get; set; }
    public List<string> OpenPanels { get; set; } = new List<string>();
}

// User settings structure
public class UserSettings
{
    public Dictionary<string, object> ValueControlSettings { get; set; } = new Dictionary<string, object>();
    public Dictionary<string, PanelInfo> PanelConfig { get; set; } = new Dictionary<string, PanelInfo>();
    public string LastConnectionIP { get; set; } = "127.0.0.1";
    public int LastConnectionPort { get; set; } = 4318;
    public bool AutoConnect { get; set; } = false;
}

// Dynamic Panel System Classes
public class CustomUI
{
    public string PanelName { get; set; } = string.Empty;
    public List<string> CompatibleStates { get; set; } = new();
    public string EnterAction { get; set; } = string.Empty;
    public string ExitAction { get; set; } = string.Empty;
    public string File { get; set; } = string.Empty;
    public bool Dirty { get; set; }
    public bool DefaultPanel { get; set; }

    private List<ActionButton> _actionButtons = new();
    private List<ValueControl> _valueControls = new();

    public List<ActionButton> GetActionButtons() => new(_actionButtons);
    public List<ValueControl> GetValueControls() => new(_valueControls);

    public void AddActionButton(ActionButton button)
    {
        _actionButtons.Add(button);
        Dirty = true;
    }

    public void AddValueControl(ValueControl control)
    {
        _valueControls.Add(control);
        Dirty = true;
    }

    public void Release()
    {
        _actionButtons.Clear();
        _valueControls.Clear();
    }

    // XML Panel Loading System
    public static CustomUI LoadFromXml(string xmlContent)
    {
        try
        {
            var doc = new System.Xml.XmlDocument();
            doc.LoadXml(xmlContent);
            
            var panel = new CustomUI();
            var root = doc.DocumentElement;
            
            if (root?.Name == "Panel")
            {
                panel.PanelName = root.GetAttribute("name") ?? string.Empty;
                panel.EnterAction = root.GetAttribute("enterAction") ?? string.Empty;
                panel.ExitAction = root.GetAttribute("exitAction") ?? string.Empty;
                
                // Load compatible states
                var statesNode = root.SelectSingleNode("CompatibleStates");
                if (statesNode != null)
                {
                    foreach (System.Xml.XmlNode stateNode in statesNode.ChildNodes)
                    {
                        if (stateNode.Name == "State")
                        {
                            panel.CompatibleStates.Add(stateNode.InnerText);
                        }
                    }
                }
                
                // Load action buttons
                var buttonsNode = root.SelectSingleNode("ActionButtons");
                if (buttonsNode != null)
                {
                    foreach (System.Xml.XmlNode buttonNode in buttonsNode.ChildNodes)
                    {
                        if (buttonNode.Name == "ActionButton" && buttonNode is System.Xml.XmlElement buttonElement)
                        {
                            var button = new ActionButton
                            {
                                Text = buttonElement.GetAttribute("text") ?? string.Empty,
                                Action = buttonElement.GetAttribute("action") ?? string.Empty,
                                X = int.TryParse(buttonElement.GetAttribute("x"), out int x) ? x : 0,
                                Y = int.TryParse(buttonElement.GetAttribute("y"), out int y) ? y : 0,
                                Width = int.TryParse(buttonElement.GetAttribute("width"), out int w) ? w : 100,
                                Height = int.TryParse(buttonElement.GetAttribute("height"), out int h) ? h : 30,
                                ToolTip = buttonElement.GetAttribute("tooltip") ?? string.Empty
                            };
                            panel.AddActionButton(button);
                        }
                    }
                }
                
                // Load value controls
                var controlsNode = root.SelectSingleNode("ValueControls");
                if (controlsNode != null)
                {
                    foreach (System.Xml.XmlNode controlNode in controlsNode.ChildNodes)
                    {
                        if (controlNode.Name == "ValueControl" && controlNode is System.Xml.XmlElement controlElement)
                        {
                            var control = new ValueControl
                            {
                                Name = controlElement.GetAttribute("name") ?? string.Empty,
                                Label = controlElement.GetAttribute("label") ?? string.Empty,
                                Value = controlElement.GetAttribute("value") ?? string.Empty,
                                Type = Enum.TryParse<ValueControlType>(controlElement.GetAttribute("type"), out var type) ? type : ValueControlType.Label,
                                X = int.TryParse(controlElement.GetAttribute("x"), out int x) ? x : 0,
                                Y = int.TryParse(controlElement.GetAttribute("y"), out int y) ? y : 0,
                                Width = int.TryParse(controlElement.GetAttribute("width"), out int w) ? w : 100,
                                Height = int.TryParse(controlElement.GetAttribute("height"), out int h) ? h : 20,
                                ReadOnly = bool.TryParse(controlElement.GetAttribute("readonly"), out bool ro) && ro,
                                ToolTip = controlElement.GetAttribute("tooltip") ?? string.Empty,
                                UpdateCommand = controlElement.GetAttribute("updateCommand") ?? string.Empty
                            };
                            panel.AddValueControl(control);
                        }
                    }
                }
            }
            
            return panel;
        }
        catch (Exception ex)
        {
            ErrorHandling.Error(ex, "Failed to load panel from XML", ErrorLevel.ShowMessage);
            return new CustomUI();
        }
    }

    public string SaveToXml()
    {
        try
        {
            var doc = new System.Xml.XmlDocument();
            var root = doc.CreateElement("Panel");
            root.SetAttribute("name", PanelName);
            root.SetAttribute("enterAction", EnterAction);
            root.SetAttribute("exitAction", ExitAction);
            doc.AppendChild(root);
            
            // Save compatible states
            if (CompatibleStates.Count > 0)
            {
                var statesNode = doc.CreateElement("CompatibleStates");
                foreach (var state in CompatibleStates)
                {
                    var stateNode = doc.CreateElement("State");
                    stateNode.InnerText = state;
                    statesNode.AppendChild(stateNode);
                }
                root.AppendChild(statesNode);
            }
            
            // Save action buttons
            if (_actionButtons.Count > 0)
            {
                var buttonsNode = doc.CreateElement("ActionButtons");
                foreach (var button in _actionButtons)
                {
                    var buttonNode = doc.CreateElement("ActionButton");
                    buttonNode.SetAttribute("text", button.Text);
                    buttonNode.SetAttribute("action", button.Action);
                    buttonNode.SetAttribute("x", button.X.ToString());
                    buttonNode.SetAttribute("y", button.Y.ToString());
                    buttonNode.SetAttribute("width", button.Width.ToString());
                    buttonNode.SetAttribute("height", button.Height.ToString());
                    buttonNode.SetAttribute("tooltip", button.ToolTip);
                    buttonsNode.AppendChild(buttonNode);
                }
                root.AppendChild(buttonsNode);
            }
            
            // Save value controls
            if (_valueControls.Count > 0)
            {
                var controlsNode = doc.CreateElement("ValueControls");
                foreach (var control in _valueControls)
                {
                    var controlNode = doc.CreateElement("ValueControl");
                    controlNode.SetAttribute("name", control.Name);
                    controlNode.SetAttribute("label", control.Label);
                    controlNode.SetAttribute("value", control.Value);
                    controlNode.SetAttribute("type", control.Type.ToString());
                    controlNode.SetAttribute("x", control.X.ToString());
                    controlNode.SetAttribute("y", control.Y.ToString());
                    controlNode.SetAttribute("width", control.Width.ToString());
                    controlNode.SetAttribute("height", control.Height.ToString());
                    controlNode.SetAttribute("readonly", control.ReadOnly.ToString());
                    controlNode.SetAttribute("tooltip", control.ToolTip);
                    controlNode.SetAttribute("updateCommand", control.UpdateCommand);
                    controlsNode.AppendChild(controlNode);
                }
                root.AppendChild(controlsNode);
            }
            
            return doc.OuterXml;
        }
        catch (Exception ex)
        {
            ErrorHandling.Error(ex, "Failed to save panel to XML", ErrorLevel.ShowMessage);
            return string.Empty;
        }
    }
}

// Panel Manager for predefined panels
public static class PanelManager
{
    private static readonly Dictionary<string, CustomUI> _predefinedPanels = new();

    static PanelManager()
    {
        InitializePredefinedPanels();
    }

    private static void InitializePredefinedPanels()
    {
        // Active Player Panel (most complex)
        var activePlayerPanel = new CustomUI
        {
            PanelName = "Active Player",
            CompatibleStates = new List<string> { "Main State" }
        };

        // Tech Controls
        activePlayerPanel.AddActionButton(new ActionButton("Grant All Techs", "GrantAllTechs()") { X = 10, Y = 10, Width = 120 });
        activePlayerPanel.AddActionButton(new ActionButton("Remove All Techs", "RemoveAllTechs()") { X = 140, Y = 10, Width = 120 });
        activePlayerPanel.AddActionButton(new ActionButton("Grant All Techs (All Players)", "GrantAllTechsAllPlayers()") { X = 270, Y = 10, Width = 180 });

        // Era Controls
        activePlayerPanel.AddActionButton(new ActionButton("Ancient Era Techs", "GrantAncientTechs()") { X = 10, Y = 50, Width = 120 });
        activePlayerPanel.AddActionButton(new ActionButton("Classical Era Techs", "GrantClassicalTechs()") { X = 140, Y = 50, Width = 120 });
        activePlayerPanel.AddActionButton(new ActionButton("Medieval Era Techs", "GrantMedievalTechs()") { X = 270, Y = 50, Width = 120 });

        // Resource Controls
        activePlayerPanel.AddValueControl(new ValueControl("Gold", "Gold:", ValueControlType.NumericUpDown) { X = 10, Y = 90, Width = 80, Value = "0", UpdateCommand = "SetGold({value})" });
        activePlayerPanel.AddActionButton(new ActionButton("1000 Gold", "AddGold(1000)") { X = 100, Y = 90, Width = 80 });
        activePlayerPanel.AddActionButton(new ActionButton("Player 1 DoW", "DeclareWar(1)") { X = 190, Y = 90, Width = 80 });

        activePlayerPanel.AddActionButton(new ActionButton("1000 Culture", "AddCulture(1000)") { X = 10, Y = 120, Width = 100 });
        activePlayerPanel.AddActionButton(new ActionButton("100 Faith", "AddFaith(100)") { X = 120, Y = 120, Width = 80 });

        // Research Controls
        activePlayerPanel.AddValueControl(new ValueControl("CurrentResearchID", "Current Research ID:", ValueControlType.NumericUpDown) { X = 10, Y = 160, Width = 100, Value = "0", UpdateCommand = "SetCurrentResearch({value})" });
        activePlayerPanel.AddValueControl(new ValueControl("ResearchProgress", "Research Progress:", ValueControlType.NumericUpDown) { X = 120, Y = 160, Width = 100, Value = "0", UpdateCommand = "SetResearchProgress({value})" });

        // Building Controls
        activePlayerPanel.AddActionButton(new ActionButton("Recall Building Maint", "RecallBuildingMaintenance()") { X = 10, Y = 200, Width = 150 });
        activePlayerPanel.AddActionButton(new ActionButton("Add One Tech", "AddOneTech()") { X = 170, Y = 200, Width = 100 });

        _predefinedPanels["Active Player"] = activePlayerPanel;

        // Game Panel
        var gamePanel = new CustomUI
        {
            PanelName = "Game",
            CompatibleStates = new List<string> { "Main State" }
        };

        // Autoplay Controls
        gamePanel.AddActionButton(new ActionButton("1", "Autoplay(1)") { X = 10, Y = 10, Width = 40 });
        gamePanel.AddActionButton(new ActionButton("5", "Autoplay(5)") { X = 60, Y = 10, Width = 40 });
        gamePanel.AddActionButton(new ActionButton("10", "Autoplay(10)") { X = 110, Y = 10, Width = 40 });
        gamePanel.AddActionButton(new ActionButton("50", "Autoplay(50)") { X = 160, Y = 10, Width = 40 });
        gamePanel.AddActionButton(new ActionButton("100", "Autoplay(100)") { X = 210, Y = 10, Width = 40 });
        gamePanel.AddActionButton(new ActionButton("150", "Autoplay(150)") { X = 260, Y = 10, Width = 40 });
        gamePanel.AddActionButton(new ActionButton("200", "Autoplay(200)") { X = 310, Y = 10, Width = 40 });
        gamePanel.AddActionButton(new ActionButton("300", "Autoplay(300)") { X = 360, Y = 10, Width = 40 });

        // Autoplay Configuration
        gamePanel.AddValueControl(new ValueControl("AutoplayTurns", "Autoplay Turns:", ValueControlType.NumericUpDown) { X = 10, Y = 50, Width = 100, Value = "1", UpdateCommand = "SetAutoplayTurns({value})" });
        gamePanel.AddActionButton(new ActionButton("Stop AutoPlay", "StopAutoplay()") { X = 120, Y = 50, Width = 100 });

        // Win Conditions
        gamePanel.AddActionButton(new ActionButton("Win Game - Time", "WinGame('Time')") { X = 10, Y = 90, Width = 100 });
        gamePanel.AddActionButton(new ActionButton("Win Game - Tech", "WinGame('Tech')") { X = 120, Y = 90, Width = 100 });
        gamePanel.AddActionButton(new ActionButton("Win Game - Domination", "WinGame('Domination')") { X = 230, Y = 90, Width = 120 });
        gamePanel.AddActionButton(new ActionButton("Win Game - Culture", "WinGame('Culture')") { X = 360, Y = 90, Width = 100 });
        gamePanel.AddActionButton(new ActionButton("Win Game - Diplomacy", "WinGame('Diplomacy')") { X = 470, Y = 90, Width = 120 });

        _predefinedPanels["Game"] = gamePanel;

        // Audio Panel
        var audioPanel = new CustomUI
        {
            PanelName = "Audio",
            CompatibleStates = new List<string> { "Main State" }
        };

        // Listener Position
        audioPanel.AddValueControl(new ValueControl("ListenerX", "Listener X:", ValueControlType.NumericUpDown) { X = 10, Y = 10, Width = 80, Value = "0", UpdateCommand = "SetListenerPosition({value}, GetListenerY(), GetListenerZ())" });
        audioPanel.AddValueControl(new ValueControl("ListenerY", "Y:", ValueControlType.NumericUpDown) { X = 100, Y = 10, Width = 80, Value = "0", UpdateCommand = "SetListenerPosition(GetListenerX(), {value}, GetListenerZ())" });
        audioPanel.AddValueControl(new ValueControl("ListenerZ", "Z:", ValueControlType.NumericUpDown) { X = 190, Y = 10, Width = 80, Value = "0", UpdateCommand = "SetListenerPosition(GetListenerX(), GetListenerY(), {value})" });

        // Audio Controls
        audioPanel.AddActionButton(new ActionButton("Play Next Song", "PlayNextSong()") { X = 10, Y = 50, Width = 100 });
        audioPanel.AddActionButton(new ActionButton("Toggle War", "ToggleWarMusic()") { X = 120, Y = 50, Width = 80 });

        // Volume Controls
        audioPanel.AddValueControl(new ValueControl("MusicVolume", "Music Volume:", ValueControlType.NumericUpDown) { X = 10, Y = 90, Width = 100, Value = "50", UpdateCommand = "SetMusicVolume({value})" });
        audioPanel.AddValueControl(new ValueControl("EffectsVolume", "Effects Volume:", ValueControlType.NumericUpDown) { X = 120, Y = 90, Width = 100, Value = "50", UpdateCommand = "SetEffectsVolume({value})" });
        audioPanel.AddValueControl(new ValueControl("LeaderVolume", "Leader Volume:", ValueControlType.NumericUpDown) { X = 230, Y = 90, Width = 100, Value = "50", UpdateCommand = "SetLeaderVolume({value})" });
        audioPanel.AddValueControl(new ValueControl("AmbienceVolume", "Ambience Volume:", ValueControlType.NumericUpDown) { X = 340, Y = 90, Width = 100, Value = "50", UpdateCommand = "SetAmbienceVolume({value})" });

        _predefinedPanels["Audio"] = audioPanel;

        // Add other panels with basic structure
        var panels = new[]
        {
            "Audio Logging", "Lua Mem Tracking", "Map", "Players", 
            "Selected City", "Selected Unit", "Table Browser", "Network"
        };

        foreach (var panelName in panels)
        {
            var panel = new CustomUI
            {
                PanelName = panelName,
                CompatibleStates = new List<string> { "Main State" }
            };
            
            // Add placeholder content
            panel.AddActionButton(new ActionButton($"Action 1", $"{panelName}Action1()") { X = 10, Y = 10, Width = 100 });
            panel.AddActionButton(new ActionButton($"Action 2", $"{panelName}Action2()") { X = 120, Y = 10, Width = 100 });
            panel.AddValueControl(new ValueControl($"{panelName}Value", $"{panelName} Value:", ValueControlType.TextBox) { X = 10, Y = 50, Width = 150, Value = "0" });
            
            _predefinedPanels[panelName] = panel;
        }
    }

    public static CustomUI? GetPanel(string panelName)
    {
        return _predefinedPanels.TryGetValue(panelName, out var panel) ? panel : null;
    }

    public static List<string> GetAvailablePanels()
    {
        return new List<string>(_predefinedPanels.Keys);
    }

    public static CustomUI LoadPanelFromFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                var xmlContent = File.ReadAllText(filePath);
                return CustomUI.LoadFromXml(xmlContent);
            }
        }
        catch (Exception ex)
        {
            ErrorHandling.Error(ex, $"Failed to load panel from file: {filePath}", ErrorLevel.ShowMessage);
        }
        
        return new CustomUI();
    }

    public static bool SavePanelToFile(CustomUI panel, string filePath)
    {
        try
        {
            var xmlContent = panel.SaveToXml();
            File.WriteAllText(filePath, xmlContent);
            return true;
        }
        catch (Exception ex)
        {
            ErrorHandling.Error(ex, $"Failed to save panel to file: {filePath}", ErrorLevel.ShowMessage);
            return false;
        }
    }
}

public class ActionButton
{
    public string Text { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; } = 100;
    public int Height { get; set; } = 30;
    public bool Enabled { get; set; } = true;
    public string ToolTip { get; set; } = string.Empty;

    public ActionButton() { }

    public ActionButton(string text, string action)
    {
        Text = text;
        Action = action;
    }

    public async void Execute()
    {
        if (string.IsNullOrEmpty(Action) || Connection.Instance == null)
            return;

        try
        {
            await Connection.Instance.SendCommandAsync(Action);
        }
        catch (Exception ex)
        {
            ErrorHandling.Error(ex, $"Failed to execute action '{Action}'", ErrorLevel.ShowMessage);
        }
    }
}

public enum ValueControlType
{
    Label,
    TextBox,
    CheckBox,
    ComboBox,
    NumericUpDown
}

public class ValueControl
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public ValueControlType Type { get; set; } = ValueControlType.Label;
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; } = 100;
    public int Height { get; set; } = 20;
    public bool ReadOnly { get; set; }
    public string ToolTip { get; set; } = string.Empty;
    public string UpdateCommand { get; set; } = string.Empty;

    public ValueControl() { }

    public ValueControl(string name, string label, ValueControlType type)
    {
        Name = name;
        Label = label;
        Type = type;
    }

    public async void UpdateValue(string newValue)
    {
        if (ReadOnly || Value == newValue)
            return;

        Value = newValue;

        if (!string.IsNullOrEmpty(UpdateCommand) && Connection.Instance != null)
        {
            try
            {
                var command = UpdateCommand.Replace("{value}", newValue);
                await Connection.Instance.SendCommandAsync(command);
            }
            catch (Exception ex)
            {
                ErrorHandling.Error(ex, $"Failed to update value '{Name}'", ErrorLevel.ShowMessage);
            }
        }
    }
}
