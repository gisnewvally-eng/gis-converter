using System.IO;

namespace GISUniversalConverterPro.Services
{
    public sealed class LoggingService
    {
        private readonly string _logPath;

        public LoggingService()
        {
            var directory = Path.Combine(AppContext.BaseDirectory, "Logs");
            Directory.CreateDirectory(directory);
            _logPath = Path.Combine(directory, "application.log");
        }

        public void Log(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            File.AppendAllText(_logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}{Environment.NewLine}");
        }
    }
}
