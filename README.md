# GameTuner

GameTuner is an independent, open-source, live game-tuning console and supporting framework, for interoperability with games and development tools you own.

## Overview

GameTuner is a Windows application for developers and modders who want to tune game parameters, debug gameplay systems, and adjust game behavior in real time. It is an independent codebase that provides modern tooling for game-development workflows.

## System Requirements

- **Operating System**: Windows 7 or later
- **.NET Framework**: 3.5 or later
- **Development**: Visual Studio 2022 (for building from source)
- **Runtime**: Windows Forms support

## Quick Start

### For Users
1. Download the latest release from the Releases page
2. Extract the archive to your desired location
3. Run `GameTuner.exe`
4. Follow the in-application help for connecting to your game

### For Developers
1. Clone this repository
2. Open `GameTuner.sln` in Visual Studio 2022
3. Build the solution (Ctrl+Shift+B)
4. Run the application (F5)

## Building from Source

### Prerequisites
- Visual Studio 2022 with .NET Framework development workload
- Windows SDK
- Git

### Build Steps
```bash
git clone https://github.com/JohnsterID/GameTuner.git
cd GameTuner
# Open GameTuner.sln in Visual Studio 2022
# Build -> Build Solution
```

## Usage

### Connecting to a Game
1. Launch GameTuner
2. Configure connection settings in the Connection dialog
3. **Verify** you have legal rights to modify the target game
4. Ensure your game supports the tuning protocol
5. Click "Connect" to establish a live connection

### Creating Custom Panels
1. Use the Panel Builder to create custom UI layouts
2. Bind controls to game parameters
3. Save your panel configuration for reuse

## Configuration

GameTuner stores configuration in the Windows Registry under:
- `HKEY_CURRENT_USER\Software\GameTuner\GameTuner\`

Key settings include:
- **Assets**: Asset file locations
- **Projects**: Project file paths
- **Tools**: External tool configurations
- **Config**: Application preferences

## Similar Projects

GameTuner is in the spirit of other open-source game-tooling and re-implementation projects:

- [Thyme](https://github.com/TheAssemblyArmada/Thyme) -- C&C Generals: Zero Hour
- [Devilution](https://github.com/diasurgical/devilution) -- Diablo
- [OpenRCT2](https://github.com/OpenRCT2/OpenRCT2) -- RollerCoaster Tycoon 2

## License

This project is licensed under the GPLv3 License - see the [LICENSE](LICENSE) file for details.

## Legal Notice

This project is **not** affiliated with, endorsed by, or connected to Firaxis Games, 2K Games, or Aspyr Media. All trademarks are the property of their respective owners.

**To use this software you must legally own the game and development tools you connect it to.** This project does not distribute game assets, SDK binaries, or any copyrighted content belonging to others.

See [DISCLAIMER.md](DISCLAIMER.md) for the full legal notice.
