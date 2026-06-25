using System.Diagnostics;
using GISUniversalConverterPro.Interfaces;
using GISUniversalConverterPro.Models;
using GISUniversalConverterPro.Services;

namespace GISUniversalConverterPro.Engines
{
    public sealed class ArcGISEngine : IConversionEngine
    {
        private readonly ArcGISDetector _detector;
        private readonly LoggingService? _loggingService;
        private ArcGISDetectionResult _detectionResult = new();
        private CancellationToken _cancellationToken;
        private Process? _activeProcess;

        public ArcGISEngine(LoggingService? loggingService = null)
        {
            _loggingService = loggingService;
            _detector = new ArcGISDetector(loggingService);
        }

        public string Name => "ArcGIS Pro Engine";
        public bool IsAvailable => _detector.Detect().CanUseArcGISEngine;
        public ConversionJob? Job { get; set; }
        public Action<int, string>? ProgressReporter { get; set; }

        public void Initialize()
        {
            _detectionResult = _detector.Detect();
            Log($"Initialized {Name}. Available={_detectionResult.CanUseArcGISEngine}; Python={_detectionResult.PythonPath}");
        }

        public void Convert()
        {
            if (Job is null)
            {
                throw new InvalidOperationException("No conversion job is assigned to the ArcGIS engine.");
            }

            if (!Validate())
            {
                throw new InvalidOperationException("ArcGIS Pro and ArcPy are required before running ArcGIS conversion.");
            }

            ThrowIfCancellationRequested();
            var scriptPath = GetScriptPath();
            var outputDirectory = OutputService.EnsureWritableOutputDirectory(Job.OutputDirectory);
            var outputName = string.IsNullOrWhiteSpace(Job.OutputName)
                ? Path.GetFileNameWithoutExtension(Job.FullPath)
                : Job.OutputName;

            ReportProgress(5, "Starting ArcGIS Pro conversion...");
            Log($"Running ArcGIS conversion. Input={Job.FullPath}; OutputDirectory={outputDirectory}; OutputName={outputName}; Script={scriptPath}");

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _detectionResult.PythonPath,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                },
                EnableRaisingEvents = true
            };
            process.StartInfo.ArgumentList.Add(scriptPath);
            process.StartInfo.ArgumentList.Add(Job.FullPath);
            process.StartInfo.ArgumentList.Add(outputDirectory);
            process.StartInfo.ArgumentList.Add(outputName);

            _activeProcess = process;
            process.OutputDataReceived += (_, args) => HandleProcessOutput(args.Data, isError: false);
            process.ErrorDataReceived += (_, args) => HandleProcessOutput(args.Data, isError: true);

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                while (!process.WaitForExit(250))
                {
                    ThrowIfCancellationRequested();
                }

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"ArcGIS conversion failed with exit code {process.ExitCode}.");
                }
            }
            finally
            {
                _activeProcess = null;
            }

            ReportProgress(100, "ArcGIS Pro conversion completed successfully.");
            Log($"ArcGIS conversion completed for {Job.FileName}.");
        }

        public bool Validate()
        {
            if (Job is null)
            {
                return false;
            }

            if (!_detectionResult.CanUseArcGISEngine)
            {
                _detectionResult = _detector.Detect();
            }

            if (!_detectionResult.CanUseArcGISEngine)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(Job.FullPath) || !File.Exists(Job.FullPath))
            {
                return false;
            }

            var extension = Path.GetExtension(Job.FullPath).ToLowerInvariant();
            if (extension != ".kml" && extension != ".kmz")
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(Job.OutputDirectory) && File.Exists(GetScriptPath());
        }

        public void Cancel()
        {
            try
            {
                if (_activeProcess is { HasExited: false })
                {
                    _activeProcess.Kill(true);
                }
            }
            catch (Exception ex)
            {
                Log($"Failed to terminate ArcGIS conversion process: {ex.Message}");
            }
        }

        public void SetCancellationToken(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        private static string GetScriptPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Python", "arcgis_kml_to_layer.py");
        }

        private void HandleProcessOutput(string? data, bool isError)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return;
            }

            var message = isError ? $"ArcGIS error: {data}" : data;
            Log(message);
            ReportProgress(isError ? 50 : 75, message);
        }

        private void ThrowIfCancellationRequested()
        {
            if (!_cancellationToken.IsCancellationRequested)
            {
                return;
            }

            Cancel();
            throw new OperationCanceledException(_cancellationToken);
        }

        private void ReportProgress(int percent, string message)
        {
            ProgressReporter?.Invoke(percent, message);
        }

        private void Log(string message)
        {
            _loggingService?.Log(message);
        }
    }
}
