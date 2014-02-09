namespace JSLintNet.VisualStudio.Errors
{
    using System.Collections.Generic;
    using JSLintNet.Models;
    using Microsoft.VisualStudio.Shell.Interop;

    internal delegate void ErrorListChangeHandler(object sender, ErrorListChangeEventArgs e);

    internal enum ErrorListAction
    {
        Unknown,
        ClearFile,
        ClearCustom,
        ClearAll,
        AddFile,
        AddCustom
    }

    /// <summary>
    /// Provides error list services for the JSLint.NET Visual Studio package.
    /// </summary>
    internal interface IJSLintErrorListProvider
    {
        event ErrorListChangeHandler ErrorListChange;

        /// <summary>
        /// Gets the error count.
        /// </summary>
        /// <value>
        /// The error count.
        /// </value>
        int ErrorCount { get; }

        /// <summary>
        /// Gets the list of errors for the specified file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>
        /// The list of errors for the specified file.
        /// </returns>
        IList<JSLintErrorTask> GetErrors(string fileName);

        /// <summary>
        /// Adds the JSLint errors to the collection.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="jsLintErrors">The JSLint errors.</param>
        /// <param name="output">The output type.</param>
        /// <param name="hierarchy">The hierarchy.</param>
        void AddJSLintErrors(string fileName, IEnumerable<IJSLintError> jsLintErrors, Output? output, IVsHierarchy hierarchy);

        /// <summary>
        /// Adds a custom error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void AddCustomError(string message, params object[] args);

        /// <summary>
        /// Clears errors for the specified file from the collection.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        void ClearJSLintErrors(string fileName);

        /// <summary>
        /// Clears the custom errors.
        /// </summary>
        void ClearCustomErrors();

        /// <summary>
        /// Clears all errors from the collection.
        /// </summary>
        void ClearAllErrors();
    }
}
