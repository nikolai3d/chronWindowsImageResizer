# Windows Image Resizer

A simple utility that adds a context menu option to Windows Explorer for quickly resizing images to a maximum dimension of 2048 pixels while preserving aspect ratio.

## Features

- Resize images to a maximum dimension of 2048px
- Preserve original aspect ratio
- Adds a convenient "Resize to 2048px" option to Windows Explorer context menu
- Simple console interface for installation/uninstallation
- Automatically handles output file naming to avoid overwriting original files

## Prerequisites

- Windows operating system
- .NET 6.0 or newer
- Administrative privileges (for context menu installation)

## Installation

### Step 1: Build and Publish the App

1. Clone or download this repository
2. Open the solution in Visual Studio
3. Build the solution in Release mode
4. Publish the application:
   - Right-click on the project in Solution Explorer
   - Select "Publish..."
   - Follow the wizard to create a self-contained application for your platform

Alternatively, you can publish from the command line:

```
# Run this command from the directory containing the .csproj file
dotnet publish -c Release -r win-x64 --self-contained
```

### Step 2: Install the Context Menu Integration

1. Open a Command Prompt or PowerShell window in Administrator mode:
   - Press Windows key
   - Type "cmd" or "powershell"
   - Right-click on the result and select "Run as administrator"

2. Navigate to the directory containing the published application

3. Run the application without any arguments to access the installation menu:
   ```
   ChronImageResizer.exe
   ```

4. You will be presented with the following menu:
   ```
   ImageResizer - Right-click tool to resize images
   =============================================

   1. Install context menu (requires admin rights)
   2. Uninstall context menu (requires admin rights)
   3. Exit

   Choose an option (1-3):
   ```

5. Press `1` to install the context menu integration

6. If successful, you'll see: "Successfully installed context menu."

## Usage

Once installed:

1. Right-click on any image file in Windows Explorer
2. Select "Resize to 2048px" from the context menu
3. The resized image will be saved in the same folder as the original with "_resized" added to the filename

The application will only resize images whose dimensions exceed 2048 pixels. For smaller images, it will create a copy instead.

## Uninstallation

1. Open a Command Prompt or PowerShell window in Administrator mode
2. Run the application without arguments
3. Select option `2` from the menu to uninstall the context menu integration

## Technical Details

- Uses SixLabors.ImageSharp for cross-platform image processing
- Registry modifications are only performed when running on Windows
- Maximum dimension can be customized by modifying the code

## License

[Include your license information here]

## Acknowledgements

- SixLabors.ImageSharp library