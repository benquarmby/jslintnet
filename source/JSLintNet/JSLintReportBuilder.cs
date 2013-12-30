namespace JSLintNet
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Text;
    using JSLintNet.Models;
    using JSLintNet.Properties;

    /// <summary>
    /// Records JSLint errors and builds a report in HTML format.
    /// </summary>
    public class JSLintReportBuilder : IJSLintReportBuilder
    {
        private Dictionary<string, ReportFile> files;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintReportBuilder"/> class.
        /// </summary>
        public JSLintReportBuilder()
        {
            this.files = new Dictionary<string, ReportFile>();
        }

        /// <summary>
        /// Gets or sets the source directory.
        /// </summary>
        /// <value>
        /// The source directory.
        /// </value>
        public string SourceDirectory { get; set; }

        /// <summary>
        /// Gets or sets the settings file.
        /// </summary>
        /// <value>
        /// The settings file.
        /// </value>
        public string SettingsFile { get; set; }

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

            if (!this.files.ContainsKey(file) && data.Errors.Count > 0)
            {
                this.ErrorFileCount += 1;
                this.ErrorCount += data.Errors.Count;

                var reportFile = new ReportFile()
                {
                    ErrorCount = data.Errors.Count,
                    ErrorReport = data.ErrorReport
                };

                this.files.Add(file, reportFile);
            }
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
                "JSLint Report",
                DateTime.Now.ToString("s"),
                this.SourceDirectory,
                string.IsNullOrEmpty(this.SettingsFile) ? "None" : this.SettingsFile,
                this.ProcessedFileCount,
                this.ErrorFileCount,
                this.ErrorCount,
                fileBuilder.ToString());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.files != null)
                {
                    this.files.Clear();
                    this.files = null;
                }
            }
        }

        private class ReportFile
        {
            public string ErrorReport { get; set; }

            public int ErrorCount { get; set; }
        }
    }
}
