namespace GISUniversalConverterPro.Models
{
    /// <summary>
    /// Contains the outcome of a conversion operation.
    /// </summary>
    public sealed class ConversionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }
}
