using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

namespace StatementBruteForce.Core
{
    /// <summary>
    ///     Simplified brute forcing of Striata encrypted EMC files and password protected PDF files.
    /// </summary>
    public static class BruteForceUtil
    {
        /// <summary>
        ///     Extracts a Striata encrypted document using a trial and error approach.
        /// </summary>
        /// <param name="identityNumbers">List of identity numbers to try.</param>
        /// <param name="documentPath">Path to document file.</param>
        /// <param name="documentExtractionPath">Path to where the document contents should be extracted.</param>
        /// <returns>The correctly guessed password.</returns>
        public static string BruteForceEmcFile(List<string> identityNumbers, string documentPath,
                                               string documentExtractionPath)
        {
            var documentPassword = "";
            var extractionDirectoryInfo = new DirectoryInfo(path: documentExtractionPath);
            if (!extractionDirectoryInfo.Exists)
            {
                extractionDirectoryInfo.Create();
            }

            var cts = new CancellationTokenSource();
            var parallelOptions = new ParallelOptions();
            parallelOptions.CancellationToken = cts.Token;

            try
            {
                Parallel.ForEach(source: identityNumbers, parallelOptions: parallelOptions,
                    body: (identityNumber, state) =>
                    {
                        var process = new Process();
                        process.StartInfo.FileName = "cmd";
                        var outputPathWsl = documentExtractionPath.ToWslPath();
                        var documentPathWsl = documentPath.ToWslPath();

                        process.StartInfo.Arguments =
                            $@"/k ""wsl striata-readerc -password={identityNumber} -outdir='{outputPathWsl}' '{documentPathWsl}'""";

                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();

                        var hadError = false;
                        var reader = process.StandardOutput;
                        var line = reader.ReadLine();
                        process.StandardOutput.DiscardBufferedData();
                        process.Kill();
                        if (!string.IsNullOrEmpty(value: line))
                        {
                            hadError = true;
                        }

                        if (!hadError)
                        {
                            documentPassword = identityNumber;
                            parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                        }
                    });
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(value: e.Message);
            }
            finally
            {
                cts.Dispose();
            }


            return documentPassword;
        }

        /// <summary>
        ///     Cracks the document password using a trial and error approach.
        /// </summary>
        /// <param name="identityNumbers">List of identity numbers to try.</param>
        /// <param name="documentPath">Path to document file.</param>
        /// <returns>The correctly guessed password.</returns>
        public static string BruteForcePdfFile(List<string> identityNumbers, string documentPath)
        {
            var documentPassword = "";

            var documentBytes =
                File.ReadAllBytes(
                    path: documentPath);

            var cts = new CancellationTokenSource();
            var parallelOptions = new ParallelOptions();
            parallelOptions.CancellationToken = cts.Token;

            try
            {
                Parallel.ForEach(source: identityNumbers, parallelOptions: parallelOptions,
                    body: (identityNumber, state) =>
                    {
                        try
                        {
                            var pdfDocument = new PdfLoadedDocument(file: documentBytes, password: identityNumber);
                            documentPassword = identityNumber;

                            parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                        }
                        catch (PdfDocumentException e)
                        {
                            // Can't open an encrypted document. The password is invalid. 
                        }
                    });
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(value: e.Message);
            }
            finally
            {
                cts.Dispose();
            }

            return documentPassword;
        }

        public static string ToWslPath(this string path) =>
            path
                .Replace(oldChar: '\\', newChar: '/')
                .Replace(oldValue: "C:", newValue: "/mnt/c");
    }
}