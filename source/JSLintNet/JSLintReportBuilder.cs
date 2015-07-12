namespace JSLintNet
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Text;
    using JSLintNet.Properties;

    /// <summary>
    /// Records JSLint errors and builds a report in HTML format.
    /// </summary>
    internal class JSLintReportBuilder
    {
        private Dictionary<string, ReportFile> files;

        private List<string> settingsFiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintReportBuilder"/> class.
        /// </summary>
        public JSLintReportBuilder()
        {
            this.files = new Dictionary<string, ReportFile>();
            this.settingsFiles = new List<string>();
        }

        /// <summary>
        /// Gets or sets the source directory.
        /// </summary>
        /// <value>
        /// The source directory.
        /// </value>
        public string SourceDirectory { get; set; }

        /// <summary>
        /// Gets the settings files.
        /// </summary>
        /// <value>
        /// The settings files.
        /// </value>
        public IList<string> SettingsFiles
        {
            get
            {
                return this.settingsFiles;
            }
        }

        /// <summary>
        /// Gets the processed file count.
        /// </summary>
        /// <value>
        /// The processed file count.
        /// </value>
        public int ProcessedFileCount { get; private set; }

        /// <summary>
        /// Gets the count of files with errors.
        /// </summary>
        /// <value>
        /// The count of files with errors.
        /// </value>
        public int ErrorFileCount { get; private set; }

        /// <summary>
        /// Gets the error count.
        /// </summary>
        /// <value>
        /// The error count.
        /// </value>
        public int ErrorCount { get; private set; }

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="data">The JSLint data.</param>
        public void AddFile(string file, IJSLintData data)
        {
            this.ProcessedFileCount += 1;

            if (!this.files.ContainsKey(file) && data.Warnings.Count > 0)
            {
                this.ErrorFileCount += 1;
                this.ErrorCount += data.Warnings.Count;

                var reportFile = new ReportFile()
                {
                    ErrorCount = data.Warnings.Count,
                    ErrorReport = data.Report
                };

                this.files.Add(file, reportFile);
            }
        }

        /// <summary>
        /// Adds the settings files.
        /// </summary>
        /// <param name="settingsFiles">The settings files.</param>
        public void AddSettings(IList<string> settingsFiles)
        {
            this.settingsFiles.AddRange(settingsFiles);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var fileBuilder = new StringBuilder();

            foreach (var file in this.files.Keys)
            {
                var reportFile = this.files[file];

                fileBuilder.AppendFormat(
                    Resources.ReportFileFormat,
                    SecurityElement.Escape(file),
                    reportFile.ErrorCount,
                    reportFile.ErrorReport);
            }

            return string.Format(
                Resources.ReportDocumentFormat,
                Resources.ReportDocumentStyle,
                Resources.ReportTitle,
                DateTime.Now.ToString("s"),
                this.SourceDirectory,
                this.SettingsFiles.Count > 0 ? string.Join(", ", this.SettingsFiles) : Resources.ReportNoSettingsFiles,
                this.ProcessedFileCount,
                this.ErrorFileCount,
                this.ErrorCount,
                fileBuilder.ToString());
        }

        private class ReportFile
        {
            public string ErrorReport { get; set; }

            public int ErrorCount { get; set; }
        }
    }
}
