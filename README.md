# AmbientHue

A Windows application that dynamically adapts the color of Philips Hue lamps to match the average color displayed on your screen. Perfect for creating immersive ambient lighting while watching movies, gaming, or enjoying multimedia content.

## Features

- **Real-time Screen Color Capture**: Continuously monitors your screen and extracts color information
- **Multiple Color Capture Methods**:
  - **Average**: Simple average of all screen pixels
  - **Quantize**: Color quantization algorithm
  - **Dominant**: Identifies the most dominant color on screen
  - **Prominent**: Advanced color prominence detection
- **Philips Hue Integration**: Seamless connection with Philips Hue Bridge and lamps
- **System Tray Application**: Runs quietly in the background with minimal UI footprint
- **Configurable Settings**: Choose which light to control and preferred color capture method
- **Low Latency**: Optimized for smooth, responsive color transitions

## Requirements

- **Operating System**: Windows (WPF application)
- **Runtime**: .NET 8.0 or higher
- **Hardware**: Philips Hue Bridge and at least one Philips Hue lamp
- **Network**: Local network connection to Philips Hue Bridge

## Installation

### From Release

1. Download the latest release from the [Releases](../../releases) page
2. Extract the archive to your desired location
3. Run `AmbientHue.exe`

### From Source

See the [Building from Source](#building-from-source) section below.

## Usage

1. **Launch the Application**: Run `AmbientHue.exe`
2. **Access Configuration**: Right-click the system tray icon and select "Configuration"
3. **Locate Bridge**: Click the "Locate" button to find your Philips Hue Bridge on the network
4. **Register Application**: 
   - Click the "Register" button
   - Press the physical button on your Philips Hue Bridge
   - Wait for successful registration
5. **Select Light**: Choose which Hue lamp you want to control from the dropdown
6. **Choose Capture Method**: Select your preferred color capture algorithm
7. **Minimize to Tray**: Click "Hide Window" to run in the background

The application will now continuously adapt your selected Hue lamp's color to match the screen content.

## Building from Source

### Prerequisites

- Visual Studio 2022 or later (or any IDE with .NET 8.0 support)
- .NET 8.0 SDK or later
- NuGet package manager (integrated in modern IDEs)

### Build Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/klukasik/AmbientHue.git
   cd AmbientHue
   ```

2. Restore and build the solution:
   ```bash
   # Using .NET CLI (recommended)
   cd AmbientHue
   dotnet restore
   dotnet build --configuration Release
   
   # Or using MSBuild
   msbuild AmbientHue.sln /p:Configuration=Release
   ```

3. The compiled executable will be located in:
   - Debug: `AmbientHue/bin/Debug/net8.0-windows/AmbientHue.exe`
   - Release: `AmbientHue/bin/Release/net8.0-windows/AmbientHue.exe`

### Using Visual Studio

1. Open `AmbientHue/AmbientHue.sln` in Visual Studio
2. Restore NuGet packages (should happen automatically)
3. Build the solution (F6 or Build → Build Solution)
4. Run the application (F5)

## Technology Stack

- **Framework**: .NET 8.0 (LTS)
- **UI**: WPF (Windows Presentation Foundation)
- **Architecture**: MVVM (Model-View-ViewModel) pattern
- **Key Libraries**:
  - [Q42.HueApi](https://github.com/Q42/Q42.HueApi) v3.24.0 - Philips Hue API integration
  - [Q42.HueApi.ColorConverters](https://github.com/Q42/Q42.HueApi) v3.23.2 - Color conversion utilities
  - [AForge.NET](http://www.aforgenet.com/) v2.2.5 - Image processing and computer vision
  - [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) v8.3.2 - Modern MVVM framework
  - [Microsoft.Extensions.DependencyInjection](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) v8.0.1 - Dependency injection
  - [Hardcodet.NotifyIcon.Wpf](https://github.com/hardcodet/wpf-notifyicon) v1.1.0 - System tray icon support
  - [Newtonsoft.Json](https://www.newtonsoft.com/json) v13.0.4 - JSON serialization

## Project Structure

```
AmbientHue/
├── AmbientHue/              # Main application directory
│   ├── CaptureColor/        # Color capture algorithms
│   │   ├── CaptureAverage.cs
│   │   ├── CaptureDominant.cs
│   │   ├── ProminentColor.cs
│   │   └── Quantize.cs
│   ├── ViewModel/           # MVVM ViewModels
│   ├── AmbientCapture.cs    # Screen capture logic
│   ├── HueConfiguration.cs  # Hue settings management
│   └── ConfigurationWindow.xaml # Configuration UI
└── README.md
```

## How It Works

1. **Screen Capture**: The application captures the current screen content at regular intervals
2. **Color Analysis**: The selected algorithm processes the screen image to determine the representative color
3. **Color Conversion**: The RGB color is converted to the Hue lamp's color space
4. **Lamp Update**: The color is sent to the Philips Hue Bridge, which updates the selected lamp

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

Konrad Lukasik

## Acknowledgments

- Philips for the Hue API
- The developers of all the open-source libraries used in this project