using System.Diagnostics;
using Microsoft.Win32;

namespace GISUniversalConverterPro.Services
{
    public sealed class ArcGISDetector
    {
        private static readonly string[] RegistryInstallKeys =
        {
            @"SOFTWARE\ESRI\ArcGISPro",
            @"SOFTWARE\WOW6432Node\ESRI\ArcGISPro"
        };

        private static readonly string[] CommonInstallDirectories =
        {
            @"C:\Program Files\ArcGIS\Pro",
            @"C:\Program Files (x86)\ArcGIS\Pro"
        };

        private readonly LoggingService? _loggingService;

        public ArcGISDetector(LoggingService? loggingService = null)
        {
            _loggingService = loggingService;
        }

        public bool IsArcGISInstalled()
        {
            return Detect().IsArcGISProInstalled;
        }

        public ArcGISDetectionResult Detect()
        {
            var installDirectory = FindInstallDirectory();
            var pythonPath = FindPythonPath(installDirectory);
            var isArcPyAvailable = !string.IsNullOrWhiteSpace(pythonPath) && IsArcPyAvailable(pythonPath);
            var result = new ArcGISDetectionResult
            {
                IsArcGISProInstalled = !string.IsNullOrWhiteSpace(installDirectory),
                InstallDirectory = installDirectory,
                PythonPath = pythonPath,
                IsArcPyAvailable = isArcPyAvailable
            };

            LogDetectionResult(result);
            return result;
        }

        private string FindInstallDirectory()
        {
            var environmentInstallPath = Environment.GetEnvironmentVariable("ARCGIS_PRO_HOME");
            if (IsArcGISProDirectory(environmentInstallPath))
            {
                return Path.GetFullPath(environmentInstallPath!);
            }

            if (OperatingSystem.IsWindows())
            {
                foreach (var registryKey in RegistryInstallKeys)
                {
                    var installDirectory = ReadRegistryInstallDirectory(Registry.LocalMachine, registryKey)
                        ?? ReadRegistryInstallDirectory(Registry.CurrentUser, registryKey);
                    if (IsArcGISProDirectory(installDirectory))
                    {
                        return Path.GetFullPath(installDirectory!);
                    }
                }
            }

            foreach (var directory in CommonInstallDirectories)
            {
                if (IsArcGISProDirectory(directory))
                {
                    return Path.GetFullPath(directory);
                }
            }

            return string.Empty;
        }

        private static string? ReadRegistryInstallDirectory(RegistryKey rootKey, string subKeyName)
        {
            using var key = rootKey.OpenSubKey(subKeyName);
            return key?.GetValue("InstallDir") as string
                ?? key?.GetValue("InstallDir64") as string;
        }

        private static bool IsArcGISProDirectory(string? directory)
        {
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            {
                return false;
            }

            return File.Exists(Path.Combine(directory, "bin", "ArcGISPro.exe"))
                || Directory.Exists(Path.Combine(directory, "bin", "Python", "envs", "arcgispro-py3"));
        }

        private static string FindPythonPath(string installDirectory)
        {
            var environmentPythonPath = Environment.GetEnvironmentVariable("ARCGIS_PRO_PYTHON");
            if (IsExecutableFile(environmentPythonPath))
            {
                return Path.GetFullPath(environmentPythonPath!);
            }

            if (!string.IsNullOrWhiteSpace(installDirectory))
            {
                var bundledPythonPath = Path.Combine(installDirectory, "bin", "Python", "envs", "arcgispro-py3", "python.exe");
                if (IsExecutableFile(bundledPythonPath))
                {
                    return Path.GetFullPath(bundledPythonPath);
                }
            }

            return string.Empty;
        }

        private static bool IsExecutableFile(string? path)
        {
            return !string.IsNullOrWhiteSpace(path) && File.Exists(path);
        }

        private bool IsArcPyAvailable(string pythonPath)
        {
            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = pythonPath,
                        Arguments = "-c \"import arcpy; print('arcpy-ok')\"",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    }
                };

                process.Start();
                if (!process.WaitForExit(30000))
                {
                    process.Kill(true);
                    _loggingService?.Log("ArcPy detection timed out.");
                    return false;
                }

                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                var isAvailable = process.ExitCode == 0 && output.Contains("arcpy-ok", StringComparison.OrdinalIgnoreCase);
                if (!isAvailable)
                {
                    _loggingService?.Log($"ArcPy detection failed. ExitCode={process.ExitCode}; Error={error}");
                }

                return isAvailable;
            }
            catch (Exception ex)
            {
                _loggingService?.Log($"ArcPy detection failed: {ex.Message}");
                return false;
            }
        }

        private void LogDetectionResult(ArcGISDetectionResult result)
        {
            _loggingService?.Log($"ArcGIS detection: Installed={result.IsArcGISProInstalled}; ArcPy={result.IsArcPyAvailable}; InstallDir={result.InstallDirectory}; Python={result.PythonPath}");
        }
    }

    public sealed class ArcGISDetectionResult
    {
        public bool IsArcGISProInstalled { get; init; }
        public bool IsArcPyAvailable { get; init; }
        public string InstallDirectory { get; init; } = string.Empty;
        public string PythonPath { get; init; } = string.Empty;
        public bool CanUseArcGISEngine => IsArcGISProInstalled && IsArcPyAvailable && !string.IsNullOrWhiteSpace(PythonPath);
    }
}
