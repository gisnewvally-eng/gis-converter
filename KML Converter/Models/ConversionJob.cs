using GISUniversalConverterPro.Converters;

namespace GISUniversalConverterPro.Models
{
    /// <summary>
    /// Represents a single input file queued for conversion.
    /// </summary>
    public sealed class ConversionJob
    {
        public string SourcePath { get; set; } = string.Empty;
        public string OutputDirectory { get; set; } = string.Empty;
        public ConversionFormat InputFormat { get; set; } = ConversionFormat.KML;
        public string OutputName { get; set; } = string.Empty;
        public bool UseArcGISIfAvailable { get; set; } = true;
        public string FileName { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string Status { get; set; } = "Ready";
        public ConversionFormat OutputFormat { get; set; } = ConversionFormat.GeoPackage;
    }
}
