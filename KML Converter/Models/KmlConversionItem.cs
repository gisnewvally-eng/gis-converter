namespace KML_Converter.Models
{
    public sealed class KmlConversionItem
    {
        public string SourcePath { get; init; } = string.Empty;
        public string OutputName { get; init; } = string.Empty;
        public string Status { get; set; } = "في الانتظار";
    }
}
