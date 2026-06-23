using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GISUniversalConverterPro.Core;
using GISUniversalConverterPro.Models;
using GISUniversalConverterPro.Services;

namespace GISUniversalConverterPro
{
    public partial class MainForm : Form
    {
        private readonly List<ConversionJob> _jobs = new();
        private readonly ArcGISDetector _arcGISDetector;
        private readonly SettingsService _settingsService;
        private readonly LoggingService _loggingService;
        private readonly OutputService _outputService;
        private ApplicationSettings _settings = new();
        private string _outputDirectory = string.Empty;
        private string _engineName = "Internal";

        public MainForm()
        {
            _arcGISDetector = new ArcGISDetector();
            _settingsService = new SettingsService();
            _loggingService = new LoggingService();
            _outputService = new OutputService();

            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            _settings = _settingsService.Load();
            var outputDirectory = _outputService.EnsureOutputDirectory(_settings.OutputDirectory);
            SetOutputDirectory(outputDirectory, save: false);

            var isArcGISInstalled = _arcGISDetector.IsArcGISInstalled();
            UpdateEngineStatus(isArcGISInstalled);
            UpdateStatus("جاهز");
            UpdateStatusStrip();
            AppendLog($"{ApplicationConstants.ApplicationName} initialized.");
        }

        private void UpdateEngineStatus(bool isArcGISInstalled)
        {
            _engineName = _settings.UseArcGISIfAvailable && isArcGISInstalled ? "ArcGIS Pro" : "Internal";
            statusLabel.Text = $"{ApplicationConstants.ApplicationName} • Engine: {_engineName}";
            _loggingService.Log($"Engine status initialized: {_engineName}");
        }

        private void SetOutputDirectory(string path, bool save)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = _outputService.EnsureOutputDirectory(string.Empty);
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            _outputDirectory = path;
            outputPathTextBox.Text = _outputDirectory;
            if (save)
            {
                _settings.OutputDirectory = _outputDirectory;
                _settingsService.Save(_settings);
            }
        }

        private bool TryAddFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            if (!File.Exists(filePath))
            {
                AppendLog($"Skipped missing file: {filePath}");
                return false;
            }

            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (extension != ".kml" && extension != ".kmz")
            {
                return false;
            }

            if (_jobs.Any(job => string.Equals(job.FullPath, filePath, StringComparison.OrdinalIgnoreCase)))
            {
                AppendLog($"Duplicate file skipped: {filePath}");
                return false;
            }

            var fileInfo = new FileInfo(filePath);
            var job = new ConversionJob
            {
                FileName = fileInfo.Name,
                FullPath = filePath,
                FileType = extension.TrimStart('.').ToUpperInvariant(),
                FileSize = fileInfo.Length,
                Status = "Ready",
                SourcePath = filePath,
                OutputDirectory = _outputDirectory,
                OutputName = Path.GetFileNameWithoutExtension(filePath),
                UseArcGISIfAvailable = _settings.UseArcGISIfAvailable
            };

            _jobs.Add(job);
            var item = new ListViewItem(job.FileName)
            {
                Tag = job
            };
            item.SubItems.Add(job.FileSize > 0 ? $"{job.FileSize / 1024} KB" : "0 KB");
            item.SubItems.Add(job.Status);
            filesListView.Items.Add(item);

            _loggingService.Log($"Added file: {job.FileName}");
            UpdateStatusStrip();
            return true;
        }

        private void AddFiles()
        {
            using var dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "ملفات KML و KMZ|*.kml;*.kmz|جميع الملفات|*.*",
                Title = "اختر ملفات KML أو KMZ"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var addedCount = 0;
            foreach (var file in dialog.FileNames.OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
            {
                if (TryAddFile(file))
                {
                    addedCount++;
                }
            }

            AppendLog($"Added {addedCount} supported file(s) to the list.");
            UpdateStatus($"تمت إضافة {addedCount} ملف(ات)");
        }

        private void RemoveSelectedFiles()
        {
            var selectedItems = filesListView.SelectedItems.Cast<ListViewItem>().ToList();
            if (selectedItems.Count == 0)
            {
                return;
            }

            foreach (var item in selectedItems)
            {
                if (item.Tag is ConversionJob job)
                {
                    _jobs.Remove(job);
                    _loggingService.Log($"Removed file: {job.FileName}");
                }

                filesListView.Items.Remove(item);
            }

            AppendLog("Removed the selected file(s).");
            UpdateStatusStrip();
            UpdateStatus($"تمت إزالة {selectedItems.Count} ملف(ات)");
        }

        private void ClearFiles()
        {
            filesListView.Items.Clear();
            _jobs.Clear();
            AppendLog("Cleared all files from the list.");
            UpdateStatusStrip();
            UpdateStatus("تمت إزالة جميع الملفات");
            _loggingService.Log("Cleared all files from the list.");
        }

        private void BrowseOutputDirectory()
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "اختر مجلد الإخراج",
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                SetOutputDirectory(dialog.SelectedPath, save: true);
                AppendLog($"Selected output folder: {_outputDirectory}");
                UpdateStatus("تم اختيار مجلد الإخراج");
            }
        }

        private void ConvertFiles()
        {
            if (_jobs.Count == 0)
            {
                MessageBox.Show(this, "يرجى إضافة ملفات KML أو KMZ أولاً.", ApplicationConstants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AppendLog("Conversion is not implemented yet. File management layer is ready.");
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = 100;
            UpdateStatus("جاهز للمرحلة التالية");
        }

        private void CancelConversion()
        {
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = 0;
            UpdateStatus("تم إلغاء العملية");
            AppendLog("Conversion cancelled by the user.");
        }

        private void OpenOutputFolder()
        {
            if (string.IsNullOrWhiteSpace(_outputDirectory))
            {
                return;
            }

            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = _outputDirectory,
                UseShellExecute = true
            });
        }

        private void UpdateStatusStrip()
        {
            totalFilesStatusLabel.Text = $"Total Files: {_jobs.Count}";
            readyFilesStatusLabel.Text = $"Ready Files: {_jobs.Count(job => string.Equals(job.Status, "Ready", StringComparison.OrdinalIgnoreCase))}";
        }

        private void UpdateStatus(string message)
        {
            statusLabel.Text = $"{ApplicationConstants.ApplicationName} • Engine: {_engineName} • {message}";
        }

        private void AppendLog(string message)
        {
            logRichTextBox.AppendText($"{DateTime.Now:HH:mm:ss} {message}{Environment.NewLine}");
            logRichTextBox.SelectionStart = logRichTextBox.Text.Length;
            logRichTextBox.ScrollToCaret();
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) != true)
            {
                return;
            }

            var droppedFiles = (string[]?)e.Data.GetData(DataFormats.FileDrop);
            if (droppedFiles == null || droppedFiles.Length == 0)
            {
                return;
            }

            var addedCount = 0;
            foreach (var file in droppedFiles.OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
            {
                if (TryAddFile(file))
                {
                    addedCount++;
                }
            }

            if (addedCount > 0)
            {
                AppendLog($"Dropped {addedCount} supported file(s).");
                UpdateStatus($"تمت إضافة {addedCount} ملف(ات) عبر السحب والإفلات");
            }
        }

        private void addFilesButton_Click(object sender, EventArgs e) => AddFiles();
        private void removeButton_Click(object sender, EventArgs e) => RemoveSelectedFiles();
        private void clearButton_Click(object sender, EventArgs e) => ClearFiles();
        private void browseOutputButton_Click(object sender, EventArgs e) => BrowseOutputDirectory();
        private void convertButton_Click(object sender, EventArgs e) => ConvertFiles();
        private void cancelButton_Click(object sender, EventArgs e) => CancelConversion();
        private void openOutputFolderButton_Click(object sender, EventArgs e) => OpenOutputFolder();

        private void addFilesToolStripButton_Click(object sender, EventArgs e) => AddFiles();
        private void browseOutputToolStripButton_Click(object sender, EventArgs e) => BrowseOutputDirectory();
        private void convertToolStripButton_Click(object sender, EventArgs e) => ConvertFiles();
        private void cancelToolStripButton_Click(object sender, EventArgs e) => CancelConversion();

        private void addFilesMenuItem_Click(object sender, EventArgs e) => AddFiles();
        private void browseOutputMenuItem_Click(object sender, EventArgs e) => BrowseOutputDirectory();
        private void convertMenuItem_Click(object sender, EventArgs e) => ConvertFiles();
        private void openOutputFolderMenuItem_Click(object sender, EventArgs e) => OpenOutputFolder();
        private void exitMenuItem_Click(object sender, EventArgs e) => Close();
        private void aboutMenuItem_Click(object sender, EventArgs e) => MessageBox.Show(this, $"{ApplicationConstants.ApplicationName}\nتحويل ملفات GIS عبر محركات داخلية أو ArcGIS Pro", "حول التطبيق", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
