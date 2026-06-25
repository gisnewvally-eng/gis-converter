using System.IO;
using GISUniversalConverterPro.Core;

namespace GISUniversalConverterPro.Services
{
    public sealed class OutputService
    {
        public string EnsureOutputDirectory(string path)
        {
            return EnsureWritableOutputDirectory(path);
        }

        public static string EnsureWritableOutputDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ApplicationConstants.DefaultOutputFolderName);
            }

            var fullPath = Path.GetFullPath(path);
            Directory.CreateDirectory(fullPath);
            VerifyWriteAccess(fullPath);
            return fullPath;
        }

        private static void VerifyWriteAccess(string directory)
        {
            var probePath = Path.Combine(directory, $".write-test-{Guid.NewGuid():N}.tmp");
            try
            {
                File.WriteAllText(probePath, string.Empty);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException || ex is DirectoryNotFoundException)
            {
                throw new UnauthorizedAccessException($"Output directory is not writable: {directory}", ex);
            }
            finally
            {
                try
                {
                    if (File.Exists(probePath))
                    {
                        File.Delete(probePath);
                    }
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to delete output directory probe file: {ex.Message}");
                }
            }
        }
    }
}
