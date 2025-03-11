using System;
using System.IO;
using System.Reflection;
using Xunit;
using System.Linq;
using System.Xml.Linq;

namespace ChronoImageResizer.Tests
{
    public class IconBundlingTests
    {
        [Fact]
        public void Test_AppIcon_Is_Bundled()
        {
            // Get the directory where the test assembly is located
            string testAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string? testDirectory = Path.GetDirectoryName(testAssemblyPath);
            
            Assert.NotNull(testDirectory);
            
            // Navigate up to the main application output directory
            // The exact path may need to be adjusted based on your build configuration
            string appDirectory = Path.GetFullPath(Path.Combine(testDirectory, "..", "..", "..", "..", "chronWindowsImageResizer", "bin", "Debug", "net8.0", "win-x64"));
            
            // For Release configuration if needed
            if (!Directory.Exists(appDirectory))
            {
                appDirectory = Path.GetFullPath(Path.Combine(testDirectory, "..", "..", "..", "..", "chronWindowsImageResizer", "bin", "Release", "net8.0", "win-x64"));
            }
            
            // Assert that the app directory exists
            Assert.True(Directory.Exists(appDirectory), $"Application directory not found at {appDirectory}");
            
            // Check if app.ico exists in the application directory
            string iconPath = Path.Combine(appDirectory, "app.ico");
            bool iconExists = File.Exists(iconPath);
            
            // Assert that the icon file exists
            Assert.True(iconExists, $"Icon file not found at {iconPath}");
            
            // Optionally verify that the file is actually an icon file by checking its size
            if (iconExists)
            {
                FileInfo iconInfo = new FileInfo(iconPath);
                Assert.True(iconInfo.Length > 0, "Icon file exists but is empty");
                
                // Typical .ico files are at least a few KB in size
                Assert.True(iconInfo.Length > 1000, $"Icon file is suspiciously small ({iconInfo.Length} bytes)");
            }
        }

        [Fact]
        public void Test_AppIcon_Is_Referenced_In_ContextMenu()
        {
            // Load the main application assembly
            string testAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string? testDirectory = Path.GetDirectoryName(testAssemblyPath);
            
            Assert.NotNull(testDirectory);
            
            // Look for the main application assembly in possible locations
            string[] possibleAppPaths = new[] 
            {
                Path.Combine(testDirectory, "chronWindowsImageResizer.dll"),
                Path.Combine(testDirectory, "..", "..", "..", "..", "chronWindowsImageResizer", "bin", "Debug", "net8.0", "win-x64", "chronWindowsImageResizer.dll"),
                Path.Combine(testDirectory, "..", "..", "..", "..", "chronWindowsImageResizer", "bin", "Release", "net8.0", "win-x64", "chronWindowsImageResizer.dll")
            };
            
            string? appAssemblyPath = possibleAppPaths.FirstOrDefault(File.Exists);
            
            Assert.True(appAssemblyPath != null, "Could not find main application assembly");
            
            // Load the main assembly
            Assembly mainAssembly = Assembly.LoadFrom(appAssemblyPath);
            
            // Look for the ContextMenuManager class
            Type? contextMenuManagerType = mainAssembly.GetTypes()
                .FirstOrDefault(t => t.Name == "ContextMenuManager");
            
            Assert.True(contextMenuManagerType != null, "ContextMenuManager class not found in assembly");
            
            // Get the Install method
            MethodInfo? installMethod = contextMenuManagerType.GetMethod("Install");
            
            Assert.True(installMethod != null, "Install method not found in ContextMenuManager class");
            
            // Get the source code file
            string sourceDir = Path.GetFullPath(Path.Combine(testDirectory, "..", "..", "..", "..", "..", "source"));
            string contextMenuManagerFile = Path.Combine(sourceDir, "ContextMenuManager.cs");
            
            Assert.True(File.Exists(contextMenuManagerFile), $"Source file not found at {contextMenuManagerFile}");
            
            // Read the file and check for the icon registry value code
            string sourceCode = File.ReadAllText(contextMenuManagerFile);
            
            // Check if the source code contains the proper icon registry format code
            bool containsIconRegistryFormat = sourceCode.Contains("iconRegistryValue") && 
                                             sourceCode.Contains("\",0") && 
                                             sourceCode.Contains("key.SetValue(\"Icon\"");
            
            Assert.True(containsIconRegistryFormat, "Source code does not contain proper icon registry format");
        }

        [Fact]
        public void Test_Project_File_Includes_Icon()
        {
            // Get the directory where the test assembly is located
            string testAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string? testDirectory = Path.GetDirectoryName(testAssemblyPath);
            
            Assert.NotNull(testDirectory);
            
            // Path to the project file
            string projectFilePath = Path.GetFullPath(Path.Combine(testDirectory, "..", "..", "..", "..", "chronWindowsImageResizer", "chronWindowsImageResizer.csproj"));
            
            // Assert project file exists
            Assert.True(File.Exists(projectFilePath), $"Project file not found at {projectFilePath}");
            
            // Load project file as XML
            XDocument projectDoc = XDocument.Load(projectFilePath);
            XNamespace ns = projectDoc.Root?.Name.Namespace ?? XNamespace.None; // Default namespace
            
            // Check if the ApplicationIcon property is set
            var applicationIconElement = projectDoc.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "ApplicationIcon");
            
            Assert.True(applicationIconElement != null, "ApplicationIcon property not found in project file");
            
            // The ApplicationIcon element should contain the path to app.ico
            Assert.Contains("app.ico", applicationIconElement.Value, StringComparison.OrdinalIgnoreCase);
            
            // Check if the app.ico is included as Content
            var contentElements = projectDoc.Descendants()
                .Where(e => e.Name.LocalName == "Content" && 
                          e.Attribute("Include") != null && 
                          e.Attribute("Include")!.Value.Contains("app.ico", StringComparison.OrdinalIgnoreCase))
                .ToList();
                
            Assert.True(contentElements.Count > 0, "app.ico not found as Content in project file");
            
            // Now find CopyToOutputDirectory elements that are children of these Content elements
            var copyToOutputSettings = contentElements
                .SelectMany(content => content.Elements())
                .Where(e => e.Name.LocalName == "CopyToOutputDirectory")
                .ToList();
            
            Assert.True(copyToOutputSettings.Count > 0, "CopyToOutputDirectory setting not found for app.ico");
            
            // Verify the setting is "Always" or "PreserveNewest"
            string copyToOutputValue = copyToOutputSettings.First().Value;
            Assert.True(
                copyToOutputValue == "Always" || copyToOutputValue == "PreserveNewest", 
                $"CopyToOutputDirectory setting is '{copyToOutputValue}', but should be 'Always' or 'PreserveNewest'"
            );
        }
    }
} 