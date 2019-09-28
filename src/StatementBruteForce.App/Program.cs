using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StatementBruteForce.Core;

namespace StatementBruteForce.App
{
    internal class Program
    {
        private static readonly string _inputDirectory = $@"{AppDomain.CurrentDomain.BaseDirectory}\input\";
        private static readonly string _outputDirectory = $@"{AppDomain.CurrentDomain.BaseDirectory}\output\";

        private static async Task Main(string[] args)
        {
            await CleanOutputDirectory();
            var files = await ListInputFiles();
            var striataDocuments =
                files.FindAll(match: f =>
                    string.Equals(a: f.Extension, b: ".emc", comparisonType: StringComparison.OrdinalIgnoreCase));
            var pdfDocuments =
                files.FindAll(match: f =>
                    string.Equals(a: f.Extension, b: ".pdf", comparisonType: StringComparison.OrdinalIgnoreCase));


            var seed =
                SouthAfricanIdentityNumberUtil.ParseIdentityNumberStringToModel(identityNumber: "190620****084");
            var identityNumbers =
                SouthAfricanIdentityNumberUtil.GenerateValidIdentityNumbers(seedModel: seed,
                    genderType: GenderType.Male);

            var document = pdfDocuments.First();
            var sw = new Stopwatch();
            sw.Start();
            var password =
                BruteForceUtil.BruteForcePdfFile(identityNumbers: identityNumbers, documentPath: document.FullName);
            sw.Stop();
            if (!string.IsNullOrEmpty(value: password))
            {
                document.CopyTo(destFileName: $"{_outputDirectory}{password}_{document.Name}");
            }

            Console.WriteLine(value: $"password: {password} {sw.ElapsedMilliseconds}ms");
        }

        private static async Task CleanOutputDirectory()
        {
            var outputDirectoryInfo = new DirectoryInfo(path: _outputDirectory);
            if (!outputDirectoryInfo.Exists)
            {
                outputDirectoryInfo.Create();
                return;
            }

            foreach (var file in outputDirectoryInfo.EnumerateFiles())
            {
                file.Delete();
            }

            foreach (var dir in outputDirectoryInfo.EnumerateDirectories())
            {
                dir.Delete(recursive: true);
            }
        }

        private static async Task<List<FileInfo>> ListInputFiles()
        {
            var inputDirectoryInfo = new DirectoryInfo(path: _inputDirectory);
            if (!inputDirectoryInfo.Exists)
            {
                throw new ArgumentException(message: "Input folder missing.");
            }

            return inputDirectoryInfo.EnumerateFiles().ToList();
        }
    }
}