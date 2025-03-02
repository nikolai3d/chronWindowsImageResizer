# Windows Image Resizer


![CI](https://github.com/nikolai3d/chronWindowsImageResizer/workflows/CI/badge.svg)

A simple utility that adds a context menu option to Windows Explorer for quickly resizing images to a maximum dimension of 2048 pixels while preserving aspect ratio.

## Features

- Resize images to a maximum dimension of 2048px
- Preserve original aspect ratio
- Adds a convenient "Resize to 2048px" option to Windows Explorer context menu
- Simple console interface for installation/uninstallation
- Automatically handles output file naming to avoid overwriting original files

## Prerequisites

- Windows operating system
- .NET 8.0 or newer
- Visual Studio 2022 or newer
- Administrative privileges (for context menu installation)

## Installation

### Step 1: Clone the Repository

Clone or download this repository

### Step 2: Build and Publish the App

#### From Visual Studio 

1. Open the solution in Visual Studio
2. Build the solution in Release mode
3. Publish the application

#### From Command Line

```bash
dotnet clean ./projects
dotnet build ./projects
dotnet publish ./projects -c Release -r win-x64 --self-contained
```

### Step 3: Install the Context Menu Integration

1. Open a Command Prompt or PowerShell window in Administrator mode:
   - Press Windows key
   - Type "cmd" or "powershell"
   - Right-click on the result and select "Run as administrator"

2. Navigate to the directory containing the published application , e.g. `projects\chronWindowsImageResizer\bin\Release\net8.0\win-x64\publish\`
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

- Uses SixLabors.ImageSharp NuGet package for image processing
- Registry modifications are only supported on Windows, for other OSs context menu would be installed differently (more research needed)
- Maximum dimension can be customized by modifying the code

## Running Tests

To run the unit test suite from the terminal:

```bash
# Run the complete test suite
dotnet test .\projects\
```

This will execute all tests in the projects directory and display the results in the terminal.

To run only specific tests, you can use the following command, e.g.:

```bash
dotnet test .\projects\ --filter "FullyQualifiedName~ImageResizer.Tests.BasicTests"
```

## License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

Copyright (c) 2025 Dr. Nikolai Svakhin

## Acknowledgements

- SixLabors.ImageSharp library
- Claude AI, Sonnet 3.7, https://claude.ai/