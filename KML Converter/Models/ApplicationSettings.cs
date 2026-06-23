namespace GISUniversalConverterPro.Models
{
    public sealed class ApplicationSettings
    {
        public string OutputDirectory { get; set; } = string.Empty;
        public bool UseArcGISIfAvailable { get; set; } = true;
        public bool EnableLogging { get; set; } = true;
        public string PreferredLanguage { get; set; } = "ar";
    }
}
