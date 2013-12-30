namespace JSLintNet
{
    using System;
    using JSLintNet.Models;

    /// <summary>
    /// Records JSLint errors and builds a report.
    /// </summary>
    public interface IJSLintReportBuilder : IDisposable
    {
        /// <summary>
        /// Gets or sets the source directory.
        /// </summary>
        /// <value>
        /// The source directory.
        /// </value>
        string SourceDirectory { get; set; }

        /// <summary>
        /// Gets or sets the settings file.
        /// </summary>
        /// <value>
        /// The settings file.
        /// </value>
        string SettingsFile { get; set; }

        /// <summary>
        /// Gets the processed file count.
        /// </summary>
        /// <value>
        /// The processed file count.
        /// </value>
        int ProcessedFileCount { get; }

        /// <summary>
        /// Gets the count of files with errors.
        /// </summary>
        /// <value>
        /// The count of files with errors.
        /// </value>
        int ErrorFileCount { get; }

        /// <summary>
        /// Gets the error count.
        /// </summary>
        /// <value>
        /// The error count.
        /// </value>
        int ErrorCount { get; }

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="data">The JSLint data.</param>
        void AddFile(string file, IJSLintData data);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        string ToString();
    }
}
