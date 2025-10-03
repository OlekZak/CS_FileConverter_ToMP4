using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace FileConverter
{
    class Program
    {
        private static bool recursive = true; // default to recursive when drag & drop
        private static string? outputDirectory = null;

        static async Task Main(string[] args)
        {
            // ✅ Set FFmpeg executable path
            FFmpeg.SetExecutablesPath(@"C:\\ffmpeg\\bin");

            List<string> directories = new List<string>();

            if (args.Length > 0)
            {
                foreach (string path in args)
                {
                    if (Directory.Exists(path))
                        directories.Add(path);
                }

                if (directories.Count == 0)
                {
                    Console.WriteLine("No valid directories were provided.");
                    return;
                }

                Console.WriteLine("Drag & drop mode: Recursive search is enabled by default.");
                recursive = true;
                outputDirectory = null;
            }
            else
            {
                Console.WriteLine("Enter the full path(s) to the directory containing files to convert.");
                Console.WriteLine("You can separate multiple directories with a semicolon (;)");
                string? input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    directories = input.Split(';')
                                       .Select(p => p.Trim())
                                       .Where(Directory.Exists)
                                       .ToList();
                }

                if (directories.Count == 0)
                {
                    Console.WriteLine("No valid directories provided.");
                    return;
                }

                Console.Write("Do you want to include subfolders? (y/n): ");
                string? choice = Console.ReadLine();
                recursive = choice?.Trim().ToLower() == "y";

                Console.WriteLine("Enter output directory for converted files (press Enter for default 'Converted' folder inside first input directory):");
                string? outDirInput = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(outDirInput))
                {
                    outputDirectory = outDirInput.Trim();
                    Directory.CreateDirectory(outputDirectory);
                }
                else
                {
                    outputDirectory = Path.Combine(directories[0], "Converted");
                    Directory.CreateDirectory(outputDirectory);
                }
            }

            foreach (string directoryPath in directories)
            {
                string targetOutputDir = outputDirectory ?? Path.Combine(directoryPath, "Converted");
                Directory.CreateDirectory(targetOutputDir);
                await Run(directoryPath, targetOutputDir);
            }

            Console.WriteLine("\nAll the selected files have been. Press Enter to exit.");
            Console.ReadLine();
        }

        private static async Task Run(string directoryPath, string targetOutputDir)
        {
            ICollection<FileInfo> col = GetFilesToConvert(directoryPath).ToList();
            Queue<FileInfo> filesToConvert = new Queue<FileInfo>(col);

            Console.WriteLine($"\nFound {filesToConvert.Count} files to convert in \"{directoryPath}\""
                            + (recursive ? " (including subfolders)" : ""));

            await RunConversion(filesToConvert, directoryPath, targetOutputDir);
        }

        private static IEnumerable<FileInfo> GetFilesToConvert(string directoryPath)
        {
            SearchOption option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return new DirectoryInfo(directoryPath).GetFiles("*.*", option)
                                                   .Where(x => !x.Extension.Equals(".mp4", StringComparison.OrdinalIgnoreCase));
        }

        private static async Task RunConversion(Queue<FileInfo> filesToConvert, string baseInputDir, string baseOutputDir)
        {
            int totalFiles = filesToConvert.Count;
            int fileIndex = 0;

            while (filesToConvert.Count > 0)
            {
                fileIndex++;
                FileInfo fileToConvert = filesToConvert.Dequeue();

                string relativePath = Path.GetRelativePath(baseInputDir, fileToConvert.DirectoryName!);
                string targetDir = Path.Combine(baseOutputDir, relativePath);
                Directory.CreateDirectory(targetDir);

                string outputFileName = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(fileToConvert.Name) + ".mp4");

                if (File.Exists(outputFileName))
                {
                    Console.WriteLine($"\nSkipping [{fileToConvert.FullName}] as it's already in .mp4 format as {outputFileName}");
                    continue;
                }

                Console.WriteLine($"\n📊 File {fileIndex} of {totalFiles}");
                Console.WriteLine($"🎬 Converting: {fileToConvert.FullName}");

                var conversion = await FFmpeg.Conversions.FromSnippet.Convert(fileToConvert.FullName, outputFileName);

                DateTime startTime = DateTime.Now;

                conversion.OnProgress += (sender, args) =>
                {
                    if (args.TotalLength != TimeSpan.Zero && args.Duration > TimeSpan.Zero)
                    {
                        double progress = args.Duration.TotalSeconds / args.TotalLength.TotalSeconds;
                        int barWidth = 30;
                        int filled = (int)(progress * barWidth);

                        string bar = "[" + new string('#', filled) + new string('-', barWidth - filled) + "]";

                        // ETA calculation
                        TimeSpan elapsed = DateTime.Now - startTime;
                        double estimatedTotalSeconds = elapsed.TotalSeconds / progress;
                        TimeSpan eta = TimeSpan.FromSeconds(estimatedTotalSeconds - elapsed.TotalSeconds);

                        Console.CursorLeft = 0;
                        Console.Write($"{bar} {progress:P0}  Elapsed: {elapsed:mm\\:ss}  ETA: {eta:mm\\:ss}");
                    }
                };

                await conversion.Start();
                Console.WriteLine($"\nFinished converting {outputFileName}");
            }
        }
    }
}
