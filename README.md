# GameTuner

GameTuner is an open source reimplementation of Firaxis FireTurner2 and Firaxis.Framework functionality, developed through reverse engineering for interoperability and educational purposes.

## Overview

GameTuner is a Windows application designed for game developers and modders who need to tune game parameters, debug gameplay systems, and modify game behavior in real-time. This independent reimplementation provides modern tooling for game development workflows while maintaining compatibility with established development practices.

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

## License

This project is licensed under the GPLv3 License - see the [LICENSE](LICENSE) file for details.

## Legal Notice

GameTuner is an independent open source reimplementation developed for:
- **Educational and research purposes**
- **Interoperability** with legally owned games and development tools
- **Community modding** and development support

**No Affiliation:** This project has no affiliation with Firaxis Games or the original software creators. Firaxis Games has not endorsed and does not support this product.

Please see [DISCLAIMER.md](DISCLAIMER.md) for complete legal information.
