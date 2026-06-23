using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GISUniversalConverterPro
{
    partial class MainForm
    {
        private IContainer components = null;

        private MenuStrip mainMenuStrip;
        private ToolStrip mainToolStrip;
        private ToolStripButton addFilesToolStripButton;
        private ToolStripButton browseOutputToolStripButton;
        private ToolStripButton convertToolStripButton;
        private ToolStripButton cancelToolStripButton;
        private Panel headerPanel;
        private Label titleLabel;
        private Label subtitleLabel;
        private FlowLayoutPanel actionPanel;
        private Button addFilesButton;
        private Button removeButton;
        private Button clearButton;
        private Button browseOutputButton;
        private Button convertButton;
        private Button cancelButton;
        private Button openOutputFolderButton;
        private Label outputLabel;
        private TextBox outputPathTextBox;
        private ListView filesListView;
        private ColumnHeader fileNameColumnHeader;
        private ColumnHeader sizeColumnHeader;
        private ColumnHeader statusColumnHeader;
        private ProgressBar progressBar;
        private RichTextBox logRichTextBox;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private ToolStripStatusLabel totalFilesStatusLabel;
        private ToolStripStatusLabel readyFilesStatusLabel;
        private TableLayoutPanel rootLayoutPanel;
        private TableLayoutPanel outputLayoutPanel;

        private ToolStripMenuItem fileMenuItem;
        private ToolStripMenuItem addFilesMenuItem;
        private ToolStripMenuItem browseOutputMenuItem;
        private ToolStripMenuItem openOutputFolderMenuItem;
        private ToolStripMenuItem convertMenuItem;
        private ToolStripMenuItem exitMenuItem;
        private ToolStripMenuItem toolsMenuItem;
        private ToolStripMenuItem helpMenuItem;
        private ToolStripMenuItem aboutMenuItem;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            mainMenuStrip = new MenuStrip();
            fileMenuItem = new ToolStripMenuItem();
            addFilesMenuItem = new ToolStripMenuItem();
            browseOutputMenuItem = new ToolStripMenuItem();
            openOutputFolderMenuItem = new ToolStripMenuItem();
            convertMenuItem = new ToolStripMenuItem();
            exitMenuItem = new ToolStripMenuItem();
            toolsMenuItem = new ToolStripMenuItem();
            helpMenuItem = new ToolStripMenuItem();
            aboutMenuItem = new ToolStripMenuItem();
            mainToolStrip = new ToolStrip();
            addFilesToolStripButton = new ToolStripButton();
            browseOutputToolStripButton = new ToolStripButton();
            convertToolStripButton = new ToolStripButton();
            cancelToolStripButton = new ToolStripButton();
            headerPanel = new Panel();
            titleLabel = new Label();
            subtitleLabel = new Label();
            actionPanel = new FlowLayoutPanel();
            addFilesButton = new Button();
            removeButton = new Button();
            clearButton = new Button();
            browseOutputButton = new Button();
            convertButton = new Button();
            cancelButton = new Button();
            openOutputFolderButton = new Button();
            outputLabel = new Label();
            outputPathTextBox = new TextBox();
            filesListView = new ListView();
            fileNameColumnHeader = new ColumnHeader();
            sizeColumnHeader = new ColumnHeader();
            statusColumnHeader = new ColumnHeader();
            progressBar = new ProgressBar();
            logRichTextBox = new RichTextBox();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            totalFilesStatusLabel = new ToolStripStatusLabel();
            readyFilesStatusLabel = new ToolStripStatusLabel();
            rootLayoutPanel = new TableLayoutPanel();
            outputLayoutPanel = new TableLayoutPanel();

            SuspendLayout();

            mainMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                fileMenuItem,
                toolsMenuItem,
                helpMenuItem
            });
            mainMenuStrip.RenderMode = ToolStripRenderMode.System;
            mainMenuStrip.Dock = DockStyle.Top;
            mainMenuStrip.RightToLeft = RightToLeft.Yes;

            fileMenuItem.Text = "ملف";
            fileMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                addFilesMenuItem,
                browseOutputMenuItem,
                openOutputFolderMenuItem,
                new ToolStripSeparator(),
                convertMenuItem,
                new ToolStripSeparator(),
                exitMenuItem
            });

            addFilesMenuItem.Text = "إضافة ملفات";
            addFilesMenuItem.Click += addFilesMenuItem_Click;

            browseOutputMenuItem.Text = "تحديد مجلد الإخراج";
            browseOutputMenuItem.Click += browseOutputMenuItem_Click;

            openOutputFolderMenuItem.Text = "فتح مجلد الإخراج";
            openOutputFolderMenuItem.Click += openOutputFolderMenuItem_Click;

            convertMenuItem.Text = "تحويل";
            convertMenuItem.Click += convertMenuItem_Click;

            exitMenuItem.Text = "خروج";
            exitMenuItem.Click += exitMenuItem_Click;

            toolsMenuItem.Text = "أدوات";
            helpMenuItem.Text = "مساعدة";
            aboutMenuItem.Text = "حول التطبيق";
            helpMenuItem.DropDownItems.Add(aboutMenuItem);
            aboutMenuItem.Click += aboutMenuItem_Click;

            mainToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            mainToolStrip.Items.AddRange(new ToolStripItem[]
            {
                addFilesToolStripButton,
                browseOutputToolStripButton,
                convertToolStripButton,
                cancelToolStripButton
            });
            mainToolStrip.Dock = DockStyle.Top;
            mainToolStrip.RightToLeft = RightToLeft.Yes;

            addFilesToolStripButton.Text = "إضافة ملفات";
            addFilesToolStripButton.Click += addFilesToolStripButton_Click;
            browseOutputToolStripButton.Text = "تحديد الإخراج";
            browseOutputToolStripButton.Click += browseOutputButton_Click;
            convertToolStripButton.Text = "تحويل";
            convertToolStripButton.Click += convertButton_Click;
            cancelToolStripButton.Text = "إلغاء";
            cancelToolStripButton.Click += cancelButton_Click;

            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Padding = new Padding(14, 10, 14, 8);
            headerPanel.BackColor = Color.FromArgb(245, 248, 252);

            titleLabel.Text = "GIS Universal Converter Pro";
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point);
            titleLabel.ForeColor = Color.FromArgb(27, 56, 93);
            titleLabel.Dock = DockStyle.Top;
            titleLabel.TextAlign = ContentAlignment.MiddleRight;
            titleLabel.Margin = new Padding(0, 0, 0, 6);

            subtitleLabel.Text = "محول GIS شامل يدعم التحويلات المتعددة عبر المحركات الداخلية أو ArcGIS Pro";
            subtitleLabel.AutoSize = true;
            subtitleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            subtitleLabel.ForeColor = Color.FromArgb(89, 108, 124);
            subtitleLabel.Dock = DockStyle.Top;
            subtitleLabel.TextAlign = ContentAlignment.MiddleRight;
            headerPanel.Controls.Add(subtitleLabel);
            headerPanel.Controls.Add(titleLabel);

            actionPanel.AutoSize = true;
            actionPanel.Padding = new Padding(12);
            actionPanel.FlowDirection = FlowDirection.RightToLeft;
            actionPanel.WrapContents = true;
            actionPanel.BackColor = Color.White;
            actionPanel.Dock = DockStyle.Fill;

            addFilesButton.Text = "إضافة ملفات";
            addFilesButton.Width = 120;
            addFilesButton.Height = 36;
            addFilesButton.Click += addFilesButton_Click;
            actionPanel.Controls.Add(addFilesButton);

            removeButton.Text = "إزالة المحدد";
            removeButton.Width = 115;
            removeButton.Height = 36;
            removeButton.Click += removeButton_Click;
            actionPanel.Controls.Add(removeButton);

            clearButton.Text = "مسح";
            clearButton.Width = 95;
            clearButton.Height = 36;
            clearButton.Click += clearButton_Click;
            actionPanel.Controls.Add(clearButton);

            browseOutputButton.Text = "تحديد الإخراج";
            browseOutputButton.Width = 130;
            browseOutputButton.Height = 36;
            browseOutputButton.Click += browseOutputButton_Click;
            actionPanel.Controls.Add(browseOutputButton);

            convertButton.Text = "تحويل";
            convertButton.Width = 95;
            convertButton.Height = 36;
            convertButton.Click += convertButton_Click;
            actionPanel.Controls.Add(convertButton);

            cancelButton.Text = "إلغاء";
            cancelButton.Width = 95;
            cancelButton.Height = 36;
            cancelButton.Click += cancelButton_Click;
            actionPanel.Controls.Add(cancelButton);

            openOutputFolderButton.Text = "فتح مجلد الإخراج";
            openOutputFolderButton.Width = 155;
            openOutputFolderButton.Height = 36;
            openOutputFolderButton.Click += openOutputFolderButton_Click;
            actionPanel.Controls.Add(openOutputFolderButton);

            outputLayoutPanel.AutoSize = true;
            outputLayoutPanel.ColumnCount = 2;
            outputLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            outputLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            outputLayoutPanel.RowCount = 1;
            outputLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            outputLayoutPanel.Padding = new Padding(12, 4, 12, 4);
            outputLayoutPanel.Dock = DockStyle.Fill;
            outputLayoutPanel.BackColor = Color.Transparent;

            outputLabel.AutoSize = true;
            outputLabel.Text = "مجلد الإخراج:";
            outputLabel.Anchor = AnchorStyles.Right;
            outputLabel.Margin = new Padding(0, 0, 8, 0);
            outputLabel.TextAlign = ContentAlignment.MiddleRight;
            outputLayoutPanel.Controls.Add(outputLabel, 0, 0);

            outputPathTextBox.ReadOnly = true;
            outputPathTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            outputLayoutPanel.Controls.Add(outputPathTextBox, 1, 0);

            filesListView.Dock = DockStyle.Fill;
            filesListView.View = View.Details;
            filesListView.FullRowSelect = true;
            filesListView.GridLines = true;
            filesListView.MultiSelect = true;
            filesListView.HideSelection = false;
            filesListView.RightToLeftLayout = true;
            filesListView.Columns.AddRange(new[]
            {
                fileNameColumnHeader,
                sizeColumnHeader,
                statusColumnHeader
            });
            fileNameColumnHeader.Text = "اسم الملف";
            sizeColumnHeader.Text = "الحجم";
            statusColumnHeader.Text = "الحالة";
            fileNameColumnHeader.Width = 420;
            sizeColumnHeader.Width = 140;
            statusColumnHeader.Width = 160;

            progressBar.Dock = DockStyle.Top;
            progressBar.Height = 18;
            progressBar.Value = 0;
            progressBar.Margin = new Padding(0, 4, 0, 4);

            logRichTextBox.Dock = DockStyle.Fill;
            logRichTextBox.ReadOnly = true;
            logRichTextBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            logRichTextBox.BorderStyle = BorderStyle.FixedSingle;
            logRichTextBox.Margin = new Padding(0, 6, 0, 0);

            statusStrip.Dock = DockStyle.Bottom;
            statusStrip.Items.Add(statusLabel);
            statusStrip.Items.Add(new ToolStripSeparator());
            statusStrip.Items.Add(totalFilesStatusLabel);
            statusStrip.Items.Add(new ToolStripSeparator());
            statusStrip.Items.Add(readyFilesStatusLabel);
            statusLabel.Text = "جاهز";
            statusLabel.Spring = true;
            totalFilesStatusLabel.Text = "Total Files: 0";
            readyFilesStatusLabel.Text = "Ready Files: 0";

            rootLayoutPanel.Dock = DockStyle.Fill;
            rootLayoutPanel.ColumnCount = 1;
            rootLayoutPanel.RowCount = 6;
            rootLayoutPanel.Padding = new Padding(12, 8, 12, 8);
            rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayoutPanel.Controls.Add(headerPanel, 0, 0);
            rootLayoutPanel.Controls.Add(actionPanel, 0, 1);
            rootLayoutPanel.Controls.Add(outputLayoutPanel, 0, 2);
            rootLayoutPanel.Controls.Add(filesListView, 0, 3);
            rootLayoutPanel.Controls.Add(progressBar, 0, 4);
            rootLayoutPanel.Controls.Add(logRichTextBox, 0, 5);

            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(250, 252, 255);
            ClientSize = new Size(900, 700);
            Controls.Add(statusStrip);
            Controls.Add(rootLayoutPanel);
            Controls.Add(mainToolStrip);
            Controls.Add(mainMenuStrip);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.Sizable;
            MainMenuStrip = mainMenuStrip;
            MinimumSize = new Size(900, 650);
            Name = "MainForm";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "GIS Universal Converter Pro";
            AllowDrop = true;
            DragEnter += MainForm_DragEnter;
            DragDrop += MainForm_DragDrop;

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
