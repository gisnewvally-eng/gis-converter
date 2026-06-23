using System.IO;
using System.Text.Json;
using GISUniversalConverterPro.Models;

namespace GISUniversalConverterPro.Services
{
    public sealed class SettingsService
    {
        private readonly string _settingsPath;

        public SettingsService()
        {
            _settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        }

        public ApplicationSettings Load()
        {
            if (!File.Exists(_settingsPath))
            {
                return new ApplicationSettings();
            }

            try
            {
                var json = File.ReadAllText(_settingsPath);
                var settings = JsonSerializer.Deserialize<ApplicationSettings>(json);
                return settings ?? new ApplicationSettings();
            }
            catch
            {
                return new ApplicationSettings();
            }
        }

        public void Save(ApplicationSettings settings)
        {
            var directory = Path.GetDirectoryName(_settingsPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsPath, json);
        }
    }
}
