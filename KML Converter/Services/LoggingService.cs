using System.IO;

namespace GISUniversalConverterPro.Services
{
    public sealed class LoggingService
    {
        private readonly string _logPath;

        public bool IsEnabled { get; set; } = true;

        public LoggingService()
        {
            var directory = Path.Combine(AppContext.BaseDirectory, "Logs");
            Directory.CreateDirectory(directory);
            _logPath = Path.Combine(directory, "application.log");
        }

        public void Log(string message)
        {
            if (!IsEnabled || string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            try
            {
                File.AppendAllText(_logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}{Environment.NewLine}");
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException || ex is DirectoryNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine($"Logging failed: {ex.Message}");
            }
        }
    }
}
