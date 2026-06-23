using System.IO;

namespace GISUniversalConverterPro.Services
{
    public sealed class OutputService
    {
        public string EnsureOutputDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GIS Universal Converter Pro");
            }

            Directory.CreateDirectory(path);
            return path;
        }
    }
}
