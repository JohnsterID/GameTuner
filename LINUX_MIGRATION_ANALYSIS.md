# GameTuner Linux Migration Analysis

## Current State Analysis

### Technology Stack
- **Current Framework**: Windows Forms (.NET Framework 3.5)
- **Target Framework**: .NET 3.5 (main branch) vs .NET 8.0-windows (net8.0 branch)
- **UI Architecture**: Traditional Windows Forms with custom controls
- **Configuration**: Windows Registry-based
- **Platform**: Windows-only

### Main UI Structure (from frmMainForm.cs analysis)

```
GameTuner Main Window (944x564)
├── MenuStrip (MainMenu)
│   ├── File Menu
│   │   ├── New Panel
│   │   ├── Open Panel
│   │   ├── Save Panel
│   │   ├── Save Panel As...
│   │   └── Exit
│   ├── Connection Menu
│   │   ├── Change Connection
│   │   ├── Force Disconnect
│   │   └── Refresh Lua States
│   ├── Admin Menu
│   │   └── Edit Project Panels
│   └── Help Menu
│       └── About GameTuner...
├── TabControl (ctrlMainFormTabs) - Main content area
│   ├── Lua Console Tab
│   │   └── LuaConsole (UserControl)
│   │       ├── ToolStrip (ConsoleTools)
│   │       ├── RichTextBox (Output)
│   │       ├── TextBox (Input)
│   │       └── ComboBox (Lua State selector)
│   ├── "* New Panel *" Tab (tabCreator)
│   └── [Dynamic CustomUI panels]
│       └── CustomUI (UserControl)
│           ├── Various game tuning controls
│           ├── ActionButtons
│           ├── ValueControls (Float, Integer, String, Boolean)
│           ├── SelectionLists
│           └── TableViews
└── StatusStrip (ctrlStatusStrip)
    └── Connection Status Label
```

### Key Components Identified

1. **Main Form (frmMainForm)**
   - Window size: 944x564 pixels
   - Menu system with File, Connection, Admin, Help
   - Tab-based interface for multiple panels
   - Status bar showing connection status

2. **LuaConsole (UserControl)**
   - Console output (RichTextBox with syntax highlighting)
   - Command input (TextBox with autocomplete)
   - Lua state selector (ComboBox)
   - Toolbar with console controls

3. **CustomUI (UserControl)**
   - Dynamic panel builder for game tuning
   - Various control types: ActionButton, ValueControl, SelectionList, TableView
   - Serializable panel data structure

4. **Game Tuning Controls**
   - ActionButton: Execute Lua commands
   - ValueControl variants: FloatControl, IntegerControl, StringControl, BooleanControl
   - SelectionList/MultiselectList: List-based controls
   - TableView: Data grid functionality

## Linux Support Options Analysis

### Option 1: Avalonia UI (RECOMMENDED) ⭐

**Pros:**
- True cross-platform support (Linux, Windows, macOS)
- Modern XAML-based UI framework
- MVVM architecture fits well with existing business logic
- Excellent performance and native look/feel
- Rich control library that can replicate Windows Forms functionality
- Strong .NET ecosystem integration
- Can target .NET 8.0 for modern features

**Cons:**
- Requires significant UI rewrite (~60-80% of UI code)
- Learning curve for XAML/MVVM if team unfamiliar
- Some Windows Forms concepts need architectural changes

**Migration Effort:** High (3-6 months)
**Long-term Viability:** Excellent

### Option 2: .NET MAUI

**Pros:**
- Microsoft's official cross-platform framework
- Good .NET integration

**Cons:**
- Primarily mobile-focused, desktop support secondary
- Less mature for complex desktop applications
- Still requires complete UI rewrite
- Performance concerns for complex UIs

**Migration Effort:** High (4-8 months)
**Long-term Viability:** Good but uncertain for desktop

### Option 3: Mono + Windows Forms (DEPRECATED)

**Pros:**
- Minimal code changes required
- Can reuse existing Windows Forms code

**Cons:**
- Mono's Windows Forms implementation is incomplete and buggy
- Many controls don't work properly on Linux
- Performance issues
- Limited long-term viability (Microsoft deprecated Mono)
- Already attempted and removed from project

**Migration Effort:** Low (1-2 months)
**Long-term Viability:** Poor (deprecated)

### Option 4: Web-based UI (Blazor Server/WASM)

**Pros:**
- Cross-platform through browser
- Can reuse C# business logic
- Modern web technologies

**Cons:**
- Different paradigm from desktop app
- Network latency issues for real-time game tuning
- Limited desktop integration
- May not be suitable for real-time scenarios

**Migration Effort:** High (4-6 months)
**Long-term Viability:** Good but different use case

## Recommended Migration Path: Avalonia UI

### Phase 1: Project Structure Setup
1. Create new Avalonia projects alongside existing Windows Forms
2. Set up shared business logic library
3. Implement cross-platform configuration system (replace Registry)

### Phase 2: Core Infrastructure
1. Migrate connection management and Lua state handling
2. Create base MVVM infrastructure
3. Implement cross-platform messaging system

### Phase 3: Main Window Migration
1. Create MainWindow.axaml with menu structure
2. Implement MainWindowViewModel
3. Create tab system using Avalonia TabView

### Phase 4: Component Migration
1. **LuaConsole**: Create AvaloniaEdit-based console with syntax highlighting
2. **CustomUI**: Implement dynamic panel system with Avalonia controls
3. **Game Controls**: Create Avalonia versions of ActionButton, ValueControls, etc.

### Phase 5: Testing and Polish
1. Cross-platform testing (Linux, Windows, macOS)
2. Performance optimization
3. UI/UX refinements

## Technical Considerations

### Configuration Migration
- **Current**: Windows Registry (`HKEY_CURRENT_USER\Software\GameTuner\GameTuner\`)
- **Target**: JSON/XML files in user data directory
- **Linux Path**: `~/.config/gametuner/` or `~/.local/share/gametuner/`

### Dependencies to Address
- Replace Windows-specific APIs
- Cross-platform file dialogs
- Cross-platform process management
- Network communication (should be cross-platform already)

### Performance Considerations
- Avalonia has excellent performance for complex UIs
- Real-time game tuning requirements should be met
- Memory usage similar to Windows Forms

## Branch Strategy Recommendation

**Start from `main` branch** because:
1. Most current codebase (includes "remove mono" commit)
2. Targets .NET 3.5 which is more portable than net8.0-windows
3. Contains the complete, working Windows Forms implementation
4. Can upgrade to .NET 8.0 during Avalonia migration

## Detailed UI Analysis from Screenshots

### Main Window (944x564 pixels)
- **Theme**: Dark theme with black backgrounds, blue tab highlights
- **Menu Bar**: File, Connection, Admin, Help (matches current implementation)
- **Status Bar**: Shows connection status ("Disconnected" / "Connected")

### Tab Structure (11+ tabs visible)
1. **Lua Console** - Command-line interface with console output and input
2. **Active Player** - Complex game tuning interface (primary focus)
3. **Audio Logging** - Audio-related logging controls
4. **Audio** - Audio system controls
5. **Game** - General game controls
6. **Lua Mem Tracking** - Memory tracking for Lua
7. **Map** - Map-related controls
8. **Players** - Player management
9. **Selected City** - City-specific controls
10. **Selected Unit** - Unit-specific controls
11. **Table Browser** - Data table browser
12. **Network** - Network-related controls
13. **"* New Panel *"** - Panel creator tab

### Active Player Tab Components (Most Complex)
- **Tech Controls**: Grant All Techs, Remove All Techs, Grant All Techs (All Players), etc.
- **Era Controls**: Ancient Era Techs, Classical Era Techs, Medieval Era Techs, etc.
- **Resource Controls**: 
  - Gold: [input field] + "1000 Gold" button + "Player 1 DoW" button
  - Culture: "1000 Culture" button
  - Faith: "100 Faith" button
- **Research Controls**:
  - Current Research ID: [input field]
  - Research Progress: [input field]
- **Building Controls**: "Recall Building Maint" button
- **Action Button**: "Add One Tech"

### Control Types Identified
1. **ActionButton**: Simple command buttons (Grant All Techs, etc.)
2. **ValueControl**: Input field + associated buttons (Gold, Research ID, etc.)
3. **GroupedControls**: Related buttons grouped together (Era Techs)
4. **ComboBox**: Dropdown selections (Lua State selector)
5. **TextInput**: Command input, value inputs
6. **ConsoleOutput**: Rich text display with syntax highlighting

## Implementation Progress

### ✅ Completed
1. Created `avalonia` branch from `main`
2. Created Avalonia MVVM project structure
3. Implemented basic main window with menu and tabs
4. Created placeholder LuaConsole and PanelCreator views
5. Set up MVVM infrastructure with CommunityToolkit

### 🔄 In Progress
1. Dark theme implementation
2. Complex tab content views
3. Game tuning control components

### 📋 Next Steps

#### Phase 1: Core UI Infrastructure (1-2 weeks)
1. ✅ Basic Avalonia project setup
2. 🔄 Implement dark theme styling
3. 🔄 Create sophisticated tab content views
4. ⏳ Implement game tuning control components (ActionButton, ValueControl, etc.)
5. ⏳ Create Active Player tab layout

#### Phase 2: Business Logic Integration (2-3 weeks)
1. ⏳ Extract and migrate GameTuner.Framework to .NET 8
2. ⏳ Implement cross-platform configuration system
3. ⏳ Migrate connection management and Lua integration
4. ⏳ Implement command execution system

#### Phase 3: Advanced Features (2-3 weeks)
1. ⏳ Migrate all remaining tab types
2. ⏳ Implement panel creator functionality
3. ⏳ Add file operations (save/load panels)
4. ⏳ Implement about dialog and other dialogs

#### Phase 4: Testing and Polish (1-2 weeks)
1. ⏳ Cross-platform testing (Linux, Windows, macOS)
2. ⏳ Performance optimization
3. ⏳ UI/UX refinements
4. ⏳ Documentation and deployment

## Migration Benefits

This migration will result in a truly cross-platform GameTuner that:
- ✅ Maintains all current functionality
- ✅ Gains Linux and macOS support
- ✅ Uses modern .NET 8.0 framework
- ✅ Provides better performance and maintainability
- ✅ Enables future enhancements and features

**Total Estimated Timeline: 6-10 weeks**