using System.IO;
using GISUniversalConverterPro.Converters;
using GISUniversalConverterPro.Interfaces;
using GISUniversalConverterPro.Models;
using GISUniversalConverterPro.Services;
using MaxRev.Gdal.Core;
using OSGeo.OGR;
using OSGeo.OSR;

namespace GISUniversalConverterPro.Engines
{
    /// <summary>
    /// Converts KML/KMZ inputs using the built-in GDAL/OGR engine.
    /// </summary>
    public sealed class InternalEngine : IConversionEngine
    {
        private CancellationToken _cancellationToken;
        private bool _cancelRequested;

        public string Name => "Internal Conversion Engine";
        public bool IsAvailable => true;
        public ConversionJob? Job { get; set; }
        public Action<int, string>? ProgressReporter { get; set; }

        public void Initialize()
        {
            try
            {
                GdalBase.ConfigureAll();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize the bundled GDAL runtime. Verify that the GDAL runtime package is deployed for win-x64.", ex);
            }
        }

        public void Convert()
        {
            if (Job is null)
            {
                throw new InvalidOperationException("No conversion job is assigned to the internal engine.");
            }

            if (!Validate())
            {
                throw new InvalidOperationException("The conversion job is invalid.");
            }

            ReportProgress(5, "Initializing GDAL/OGR engine...");
            if (_cancelRequested || _cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(_cancellationToken);
            }

            var inputPath = Path.GetFullPath(Job.FullPath);
            var outputRoot = OutputService.EnsureWritableOutputDirectory(Job.OutputDirectory);
            var safeName = SanitizeFileName(Path.GetFileNameWithoutExtension(Job.FullPath));
            var outputDirectory = Path.Combine(outputRoot, safeName);
            Directory.CreateDirectory(outputDirectory);

            ReportProgress(10, $"Reading input file: {Job.FileName}");
            if (_cancelRequested || _cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(_cancellationToken);
            }

            using var dataSource = Ogr.Open(inputPath, 0);
            if (dataSource is null)
            {
                throw new InvalidDataException($"Unable to read input data source: {Job.FullPath}");
            }

            var layerCount = dataSource.GetLayerCount();
            for (var index = 0; index < layerCount; index++)
            {
                if (_cancelRequested || _cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(_cancellationToken);
                }

                using var layer = dataSource.GetLayerByIndex(index);
                if (layer is null)
                {
                    continue;
                }

                var layerName = string.IsNullOrWhiteSpace(layer.GetName()) ? $"layer_{index + 1}" : layer.GetName();
                var outputBaseName = Path.Combine(outputDirectory, $"{safeName}_{layerName}");

                WriteShapefile(layer, outputBaseName);
                WriteGeoPackage(layer, outputBaseName);
                WriteGeoJson(layer, outputBaseName);

                ReportProgress(10 + ((index + 1) * 80 / Math.Max(1, layerCount)), $"Completed layer {layerName}");
            }

            ReportProgress(100, "Conversion completed successfully.");
        }

        public bool Validate()
        {
            if (Job is null)
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

            if (string.IsNullOrWhiteSpace(Job.OutputDirectory))
            {
                return false;
            }

            return true;
        }

        public void Cancel()
        {
            _cancelRequested = true;
        }

        public void SetCancellationToken(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _cancelRequested = false;
        }

        private static string SanitizeFileName(string value)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(value.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
            return string.IsNullOrWhiteSpace(sanitized) ? "conversion" : sanitized;
        }

        private void ReportProgress(int percent, string message)
        {
            ProgressReporter?.Invoke(percent, message);
        }

        private void WriteShapefile(Layer layer, string outputBaseName)
        {
            var outputPath = $"{outputBaseName}.shp";
            var driver = Ogr.GetDriverByName("ESRI Shapefile");
            using var outputDataSource = driver.CreateDataSource(outputPath, Array.Empty<string>());
            using var outputLayer = outputDataSource.CreateLayer(Path.GetFileNameWithoutExtension(outputPath), CreateSpatialReference(), layer.GetGeomType(), null);
            CopyLayerSchema(layer, outputLayer);
            CopyFeatures(layer, outputLayer);
        }

        private void WriteGeoPackage(Layer layer, string outputBaseName)
        {
            var outputPath = $"{outputBaseName}.gpkg";
            var driver = Ogr.GetDriverByName("GPKG");
            using var outputDataSource = driver.CreateDataSource(outputPath, Array.Empty<string>());
            using var outputLayer = outputDataSource.CreateLayer(Path.GetFileNameWithoutExtension(outputPath), CreateSpatialReference(), layer.GetGeomType(), null);
            CopyLayerSchema(layer, outputLayer);
            CopyFeatures(layer, outputLayer);
        }

        private void WriteGeoJson(Layer layer, string outputBaseName)
        {
            var outputPath = $"{outputBaseName}.geojson";
            var driver = Ogr.GetDriverByName("GeoJSON");
            using var outputDataSource = driver.CreateDataSource(outputPath, Array.Empty<string>());
            using var outputLayer = outputDataSource.CreateLayer(Path.GetFileNameWithoutExtension(outputPath), CreateSpatialReference(), layer.GetGeomType(), null);
            CopyLayerSchema(layer, outputLayer);
            CopyFeatures(layer, outputLayer);
        }

        private static SpatialReference CreateSpatialReference()
        {
            var spatialReference = new SpatialReference(string.Empty);
            spatialReference.ImportFromEPSG(4326);
            return spatialReference;
        }

        private static void CopyLayerSchema(Layer sourceLayer, Layer targetLayer)
        {
            for (var index = 0; index < sourceLayer.GetLayerDefn().GetFieldCount(); index++)
            {
                var fieldDefinition = sourceLayer.GetLayerDefn().GetFieldDefn(index);
                targetLayer.CreateField(fieldDefinition, 1);
            }
        }

        private static void CopyFeatures(Layer sourceLayer, Layer targetLayer)
        {
            sourceLayer.ResetReading();
            while (true)
            {
                using var feature = sourceLayer.GetNextFeature();
                if (feature is null)
                {
                    break;
                }

                using var clonedFeature = feature.Clone();
                targetLayer.CreateFeature(clonedFeature);
            }
        }
    }
}
