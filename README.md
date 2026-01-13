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
- **Runtime**: .NET Framework 4.5.2 or higher
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

- Visual Studio 2015 or later
- .NET Framework 4.5.2 SDK
- NuGet package manager

### Build Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/klukasik/AmbientHue.git
   cd AmbientHue
   ```

2. Restore NuGet packages:
   ```bash
   nuget restore AmbientHue/AmbientHue.sln
   ```

3. Build the solution:
   ```bash
   # Debug build
   msbuild AmbientHue/AmbientHue.sln /p:Configuration=Debug

   # Release build
   msbuild AmbientHue/AmbientHue.sln /p:Configuration=Release
   ```

4. The compiled executable will be located in:
   - Debug: `AmbientHue/bin/Debug/AmbientHue.exe`
   - Release: `AmbientHue/bin/Release/AmbientHue.exe`

### Using Visual Studio

1. Open `AmbientHue/AmbientHue.sln` in Visual Studio
2. Restore NuGet packages (should happen automatically)
3. Build the solution (F6 or Build → Build Solution)
4. Run the application (F5)

## Technology Stack

- **Framework**: .NET Framework 4.5.2
- **UI**: WPF (Windows Presentation Foundation)
- **Architecture**: MVVM (Model-View-ViewModel) pattern
- **Key Libraries**:
  - [Q42.HueApi](https://github.com/Q42/Q42.HueApi) - Philips Hue API integration
  - [AForge.NET](http://www.aforgenet.com/) - Image processing and computer vision
  - [MvvmLight](http://www.mvvmlight.net/) - MVVM framework
  - [Hardcodet.NotifyIcon.Wpf](https://github.com/hardcodet/wpf-notifyicon) - System tray icon support
  - [Newtonsoft.Json](https://www.newtonsoft.com/json) - JSON serialization

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