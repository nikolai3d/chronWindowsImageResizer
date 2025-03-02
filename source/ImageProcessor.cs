using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ChronImageResizer
{
    public class ImageProcessor
    {
        private const int MaxDimension = 2048;

        /// <summary>
        /// Processes an image file, resizing if necessary and saving to a new file
        /// </summary>
        /// <param name="inputPath">Path to the source image file</param>
        /// <param name="maxDimension">Maximum dimension (width or height) for the output image</param>
        /// <returns>Path to the output file or null if processing failed</returns>
        public string? ProcessImage(string inputPath, int maxDimension = MaxDimension)
        {
            try
            {
                // Load the image
                using (var image = Image.Load(inputPath))
                {
                    // Check if resizing is needed
                    bool needsResize = image.Width > maxDimension || image.Height > maxDimension;
                    
                    if (!needsResize)
                    {
                        // No resize needed, just create a copy
                        string noResizeOutputPath = GenerateOutputPath(inputPath, false);
                        File.Copy(inputPath, noResizeOutputPath, false);
                        return noResizeOutputPath;
                    }
                    
                    // Calculate new dimensions maintaining aspect ratio
                    int newWidth, newHeight;
                    
                    if (image.Width > image.Height)
                    {
                        // Landscape orientation
                        newWidth = maxDimension;
                        newHeight = (int)Math.Round((double)image.Height * maxDimension / image.Width);
                    }
                    else
                    {
                        // Portrait orientation
                        newHeight = maxDimension;
                        newWidth = (int)Math.Round((double)image.Width * maxDimension / image.Height);
                    }
                    
                    // Resize the image
                    image.Mutate(x => x.Resize(newWidth, newHeight));
                    
                    // Save to output file
                    string outputPath = GenerateOutputPath(inputPath, true);
                    image.Save(outputPath);
                    
                    return outputPath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing image: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Generates an output file path based on the input path
        /// </summary>
        /// <param name="inputPath">Original file path</param>
        /// <param name="isResized">Whether the image was resized</param>
        /// <returns>Output file path</returns>
        private string GenerateOutputPath(string inputPath, bool isResized)
        {
            string? directory = Path.GetDirectoryName(inputPath);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(inputPath);
            string extension = Path.GetExtension(inputPath);
            
            string baseName = isResized
                ? $"{fileNameWithoutExt}_resized{extension}"
                : $"{fileNameWithoutExt}_copy{extension}";
            
            // Use current directory if directory is null
            directory ??= Directory.GetCurrentDirectory();
            
            string outputPath = Path.Combine(directory, baseName);
            
            // Handle file name conflicts by adding a number suffix
            int counter = 1;
            while (File.Exists(outputPath))
            {
                string numberedName = isResized
                    ? $"{fileNameWithoutExt}_resized_{counter}{extension}"
                    : $"{fileNameWithoutExt}_copy_{counter}{extension}";
                
                outputPath = Path.Combine(directory, numberedName);
                counter++;
            }
            
            return outputPath;
        }
    }
}