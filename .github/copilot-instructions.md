# AmbientHue - Copilot Instructions

## Project Overview
AmbientHue is a Windows WPF application that adapts the color of Philips Hue lamps to match the average color visible on the screen. This is particularly useful when watching movies for ambient lighting effects.

## Technology Stack
- **Language**: C# (.NET Framework 4.5.2)
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Architecture Pattern**: MVVM (Model-View-ViewModel)
- **Build Tool**: MSBuild
- **Package Manager**: NuGet

## Key Dependencies
- **Q42.HueApi**: Philips Hue API integration
- **MvvmLight**: MVVM framework for WPF
- **AForge**: Image processing and computer vision
- **Hardcodet.NotifyIcon.Wpf**: System tray icon support
- **Newtonsoft.Json**: JSON serialization

## Project Structure
```
AmbientHue/
├── AmbientHue/              # Main application directory
│   ├── CaptureColor/        # Color capture algorithms
│   │   ├── CaptureAverage.cs
│   │   ├── CaptureDominant.cs
│   │   └── ICaptureColor.cs
│   ├── ViewModel/           # MVVM ViewModels
│   │   ├── ConfigurationViewModel.cs
│   │   ├── NotifyIconViewModel.cs
│   │   └── ViewModelLocator.cs
│   ├── AmbientCapture.cs    # Screen capture logic
│   ├── HueConfiguration.cs  # Hue settings management
│   ├── ConfigurationWindow.xaml # Configuration UI
│   └── App.xaml             # Application entry point
```

## Coding Conventions

### General Guidelines
- Follow standard C# coding conventions
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep classes focused on a single responsibility

### MVVM Pattern
- ViewModels should inherit from `ViewModelBase` (MvvmLight)
- Use `ICommand` for UI actions
- Implement `INotifyPropertyChanged` for data binding
- Keep business logic in ViewModels, not in code-behind
- Use dependency injection via `SimpleIoc` for ViewModel registration

### Naming Conventions
- Use PascalCase for classes, methods, properties, and public fields
- Use camelCase for local variables and private fields
- Prefix interfaces with 'I' (e.g., `ICaptureColor`, `IHueConfiguration`)
- Use descriptive names that convey purpose

### Code Organization
- Group related functionality in namespaces
- Keep XAML and code-behind files together
- Place interfaces in the same file or separate interface files
- Use regions sparingly, only for large classes

## Building the Project

### Prerequisites
- Visual Studio 2015 or later
- .NET Framework 4.5.2 SDK
- NuGet package manager

### Build Commands
```bash
# Restore NuGet packages
nuget restore AmbientHue/AmbientHue.sln

# Build the solution (Debug)
msbuild AmbientHue/AmbientHue.sln /p:Configuration=Debug

# Build the solution (Release)
msbuild AmbientHue/AmbientHue.sln /p:Configuration=Release
```

### Build Configurations
- **Debug**: Includes debug symbols, no optimization, allows unsafe blocks
- **Release**: Optimized, minimal debug info

## Key Concepts

### Screen Color Capture
The application captures screen colors using multiple algorithms:
- **Average**: Simple average of all pixels
- **Dominant**: Most prominent color using quantization
Both methods use AForge.NET for image processing

### Hue Integration
- Uses Q42.HueApi for communication with Philips Hue Bridge
- Manages light state and color transitions
- Stores configuration in application settings

### System Tray Operation
- Runs as a system tray application using Hardcodet.NotifyIcon.Wpf
- Provides context menu for configuration and control
- No main window by default

## Common Tasks

### Adding New Color Capture Methods
1. Create a new class implementing `ICaptureColor`
2. Implement the color extraction algorithm
3. Register in the capture method selector
4. Update UI to allow selection

### Modifying Hue Settings
- Settings are stored using `System.Configuration`
- Edit `HueConfiguration.cs` for new settings
- Update `ConfigurationViewModel.cs` for UI binding
- Modify `ConfigurationWindow.xaml` for display

### UI Changes
- XAML files define the visual structure
- Use data binding to connect to ViewModels
- Follow WPF best practices for layouts
- Test with different DPI settings

## Testing Guidelines
- Currently no automated test suite
- Manual testing required for:
  - Screen capture accuracy
  - Hue light color synchronization
  - Configuration persistence
  - System tray interactions

## Dependencies and Security
- Keep NuGet packages updated for security patches
- Be cautious with image processing code (unsafe blocks enabled)
- Validate user input in configuration
- Handle network errors gracefully for Hue API calls

## Performance Considerations
- Screen capture runs in a loop, optimize for efficiency
- Color calculations should be fast (real-time updates)
- Minimize allocations in hot paths
- Consider frame rate limiting to reduce CPU usage

## Known Limitations
- Windows-only application (WPF and screen capture APIs)
- Requires .NET Framework 4.5.2
- Needs Philips Hue Bridge on local network
- Uses older NuGet package management (packages.config)

## Future Improvements
- Migrate to .NET Core / .NET 6+ for modern platform
- Add automated tests
- Implement PackageReference instead of packages.config
- Add more color capture algorithms
- Support multiple monitor setups
