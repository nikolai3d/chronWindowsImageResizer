using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace ChronImageResizer
{
    public class ContextMenuManager 
    {
        private const string MenuName = "Resize to 2048px";
        private const string RegistryKeyPath = @"*\shell\ChronImageResizer";
        private const string CommandKeyName = "command";
        
        /// <summary>
        /// Installs the context menu entry for image files
        /// </summary>
        /// <returns>True if installation was successful</returns>
        public bool Install()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("Context menu installation is only supported on Windows.");
                return false;
            }

            try
            {
                // Get the path to the executable
                string exePath = Assembly.GetExecutingAssembly().Location;
                
                // Handle .NET Core/5+ single file deployment
                if (exePath.EndsWith(".dll"))
                {
                    exePath = exePath.Replace(".dll", ".exe");
                }
                
                // Make sure the path is quoted for command line
                string command = $"\"{exePath}\" \"%1\"";
                
                // Create the shell context menu entry
                using (var key = Registry.ClassesRoot.CreateSubKey(RegistryKeyPath))
                {
                    if (key == null)
                    {
                        Console.WriteLine("Failed to create registry key. Are you running as administrator?");
                        return false;
                    }
                    
                    // Set the display name for the context menu
                    key.SetValue(null, MenuName);
                    
                    // Set the icon (optional) - using our own executable
                    key.SetValue("Icon", exePath);
                    
                    // Create the command subkey
                    using (var cmdKey = key.CreateSubKey(CommandKeyName))
                    {
                        if (cmdKey == null)
                        {
                            Console.WriteLine("Failed to create command registry key.");
                            return false;
                        }
                        
                        cmdKey.SetValue(null, command);
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error installing context menu: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Uninstalls the context menu entry
        /// </summary>
        /// <returns>True if uninstallation was successful</returns>
        public bool Uninstall()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("Context menu uninstallation is only supported on Windows.");
                return false;
            }

            try
            {
                // Delete the registry key for the context menu
                Registry.ClassesRoot.DeleteSubKeyTree(RegistryKeyPath, false);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uninstalling context menu: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if the platform supports context menu operations
        /// </summary>
        /// <returns>True if platform supports context menu operations</returns>
        public bool IsPlatformSupported()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }
    }
}