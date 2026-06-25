using GISUniversalConverterPro.Engines;
using GISUniversalConverterPro.Interfaces;
using GISUniversalConverterPro.Models;

namespace GISUniversalConverterPro.Services
{
    /// <summary>
    /// Selects and runs the appropriate conversion engine.
    /// </summary>
    public sealed class ConversionManager
    {
        private readonly ArcGISEngine _arcGisEngine;
        private readonly InternalEngine _internalEngine = new();
        private readonly LoggingService? _loggingService;

        public ConversionManager(LoggingService? loggingService = null)
        {
            _loggingService = loggingService;
            _arcGisEngine = new ArcGISEngine(loggingService);
        }

        public IConversionEngine GetActiveEngine(bool preferArcGIS)
        {
            if (preferArcGIS && _arcGisEngine.IsAvailable)
            {
                _loggingService?.Log("ArcGIS Pro engine selected.");
                return _arcGisEngine;
            }

            if (preferArcGIS)
            {
                _loggingService?.Log("ArcGIS Pro engine is unavailable; falling back to internal engine.");
            }
            else
            {
                _loggingService?.Log("Internal engine selected by settings.");
            }

            return _internalEngine;
        }

        public async Task<ConversionResult> RunAsync(ConversionJob job, CancellationToken cancellationToken, Action<int, string>? progressReporter = null, LoggingService? loggingService = null)
        {
            var startedAt = DateTime.UtcNow;
            var engine = GetActiveEngine(job.UseArcGISIfAvailable);
            var result = new ConversionResult
            {
                StartedAt = startedAt,
                CompletedAt = startedAt,
                OutputPath = job.OutputDirectory
            };

            var effectiveLogging = loggingService ?? _loggingService;
            try
            {
                if (engine is InternalEngine internalEngine)
                {
                    internalEngine.Job = job;
                    internalEngine.ProgressReporter = progressReporter;
                    internalEngine.SetCancellationToken(cancellationToken);
                }
                else if (engine is ArcGISEngine arcGisEngine)
                {
                    arcGisEngine.Job = job;
                    arcGisEngine.ProgressReporter = progressReporter;
                    arcGisEngine.SetCancellationToken(cancellationToken);
                }

                engine.Initialize();
                if (!engine.Validate())
                {
                    result.Success = false;
                    result.ErrorMessage = $"Engine validation failed for {engine.Name}";
                    result.Message = result.ErrorMessage;
                    effectiveLogging?.Log($"Validation failed: {result.ErrorMessage}");
                    return result;
                }

                await Task.Run(engine.Convert, cancellationToken);
                result.Success = true;
                result.Message = $"تم اختيار المحرك: {engine.Name}";
                effectiveLogging?.Log($"Completed conversion for {job.FileName}");
            }
            catch (OperationCanceledException)
            {
                result.Success = false;
                result.ErrorMessage = "Conversion cancelled by user.";
                result.Message = result.ErrorMessage;
                effectiveLogging?.Log($"Cancelled conversion for {job.FileName}");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.Message = ex.Message;
                effectiveLogging?.Log($"Failed conversion for {job.FileName}: {ex.Message}");
            }
            finally
            {
                result.CompletedAt = DateTime.UtcNow;
                result.Duration = result.CompletedAt - startedAt;
            }

            return result;
        }
    }
}
