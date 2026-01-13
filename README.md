# AmbientHue

A Windows application that dynamically adapts the color of Philips Hue lamps to match the average color displayed on your screen. Perfect for creating immersive ambient lighting while watching movies, gaming, or enjoying multimedia content.

## Features

- **Real-time Screen Color Capture**: Continuously monitors your screen and extracts color information
- **Optimized Performance**: Intelligent downsampling and frame rate limiting for minimal CPU/GPU usage
- **Multiple Color Capture Methods**:
  - **Average**: Simple average of all screen pixels (classic method)
  - **Grid Sampling**: Fast method that samples pixels at regular intervals (recommended for performance)
  - **Center Weighted**: Emphasizes center of screen where content is typically focused
  - **Edge Sampling**: Samples screen periphery for ambient edge lighting effect
  - **Quantize**: Color quantization algorithm for distinct colors
  - **Dominant**: Identifies the most dominant color on screen
  - **Prominent**: Advanced color prominence detection
- **Philips Hue Integration**: Seamless connection with Philips Hue Bridge and lamps
- **System Tray Application**: Runs quietly in the background with minimal UI footprint
- **Configurable Settings**: Choose which light to control and preferred color capture method
- **Low Latency**: Optimized for smooth, responsive color transitions at ~10 fps (configurable)

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
│   │   ├── CaptureAverage.cs          # Average all pixels
│   │   ├── CaptureGridSampling.cs     # Fast grid-based sampling
│   │   ├── CaptureCenterWeighted.cs   # Center-focused sampling
│   │   ├── CaptureEdgeSampling.cs     # Edge/periphery sampling
│   │   ├── CaptureDominant.cs         # Dominant color detection
│   │   ├── ProminentColor.cs          # Prominent color detection
│   │   └── Quantize.cs                # Color quantization
│   ├── ViewModel/           # MVVM ViewModels
│   ├── AmbientCapture.cs    # Screen capture logic with optimization
│   ├── HueConfiguration.cs  # Hue settings management
│   └── ConfigurationWindow.xaml # Configuration UI
└── README.md
```

## How It Works

1. **Screen Capture**: The application captures the current screen content at ~10 fps (configurable)
2. **Downsampling**: Screen is downsampled 4x for faster processing without quality loss
3. **Color Analysis**: The selected algorithm processes the downsampled image to determine the representative color
4. **Color Conversion**: The RGB color is converted to the Hue lamp's color space
5. **Lamp Update**: The color is sent to the Philips Hue Bridge, which updates the selected lamp
6. **Rate Limiting**: Intelligent delays prevent CPU overload while maintaining smooth transitions

## Performance Optimization

The application includes several optimizations to ensure efficient operation during video playback:

- **Downsampling**: Screen captured at 1/4 resolution (4x smaller) for 16x faster processing
- **Frame Rate Limiting**: Updates capped at ~10 fps to balance responsiveness with CPU usage
- **Efficient Sampling Methods**: New grid-based and edge sampling methods process fewer pixels
- **Instance Reuse**: Capture method objects created once and reused to reduce allocations
- **Async Delays**: Proper async/await patterns prevent blocking and reduce CPU load

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

Konrad Lukasik

## Acknowledgments

- Philips for the Hue API
- The developers of all the open-source libraries used in this project