using ChronImageResizer;
using System;
using System.IO;
using System.Reflection;

namespace ImageResizer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    // If no arguments, show usage info or handle installation
                    HandleInstallation();
                    return;
                }

                // Process each file path provided
                foreach (string filePath in args)
                {
                    if (File.Exists(filePath))
                    {
                        Console.WriteLine($"Processing: {filePath}");

                        // Process the image
                        var processor = new ImageProcessor();
                        string? outputPath = processor.ProcessImage(filePath, 2048);

                        if (!string.IsNullOrEmpty(outputPath))
                        {
                            Console.WriteLine($"Successfully processed to: {outputPath}");
                        }
                        else
                        {
                            Console.WriteLine("Failed to process the image.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"File not found: {filePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Always wait for key press to keep console open during testing
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void HandleInstallation()
        {
            Console.WriteLine("ImageResizer - Right-click tool to resize images");
            Console.WriteLine("=============================================");
            Console.WriteLine();
            Console.WriteLine("1. Install context menu (requires admin rights)");
            Console.WriteLine("2. Uninstall context menu (requires admin rights)");
            Console.WriteLine("3. Exit");
            Console.WriteLine();
            Console.Write("Choose an option (1-3): ");

            var key = Console.ReadKey();
            Console.WriteLine();

            var manager = new ContextMenuManager();

            switch (key.KeyChar)
            {
                case '1':
                    if (manager.Install())
                        Console.WriteLine("Successfully installed context menu.");
                    else
                        Console.WriteLine("Failed to install context menu. Try running as administrator.");
                    break;

                case '2':
                    if (manager.Uninstall())
                        Console.WriteLine("Successfully uninstalled context menu.");
                    else
                        Console.WriteLine("Failed to uninstall context menu. Try running as administrator.");
                    break;

                case '3':
                default:
                    return;
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}