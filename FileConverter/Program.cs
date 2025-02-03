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
        static async Task Main(string[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            // ✅ Set FFmpeg executable path
            FFmpeg.SetExecutablesPath(@"C:\ffmpeg\bin");

            await Run();
        }

        private static async Task Run()
        {
            ICollection<FileInfo> col = GetFilesToConvert("C:\\ToMP4").ToList();
            Queue<FileInfo> filesToConvert = new Queue<FileInfo>(col);

            await Console.Out.WriteLineAsync($"Found {filesToConvert.Count} files to convert.");

            await RunConversion(filesToConvert);

            Console.ReadLine();
        }

        private static IEnumerable<FileInfo> GetFilesToConvert(string directoryPath)
        {
            return new DirectoryInfo(directoryPath).GetFiles().Where(x => x.Extension != ".mp4");
        }

        private static async Task RunConversion(Queue<FileInfo> filesToConvert)
        {
            while (filesToConvert.Count > 0)
            {
                FileInfo fileToConvert = filesToConvert.Dequeue();
                string outputFileName = Path.ChangeExtension(fileToConvert.FullName, ".mp4");

                // ✅ Ensure FFmpeg executables are set
                var conversion = await FFmpeg.Conversions.FromSnippet.Convert(fileToConvert.FullName, outputFileName);
                await conversion.Start();

                await Console.Out.WriteLineAsync($"Finished conversion file [{fileToConvert.Name}]");
            }
        }
    }
}
