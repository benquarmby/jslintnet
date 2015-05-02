namespace JSLintNet.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Contains the result of a JSLint validation.
    /// </summary>
    public interface IJSLintData
    {
        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        IList<IJSLintWarning> Warnings { get; }

        /// <summary>
        /// Gets the functions.
        /// </summary>
        /// <value>
        /// The functions.
        /// </value>
        IList<IJSLintFunction> Functions { get; }

        /// <summary>
        /// Gets the global list.
        /// </summary>
        /// <value>
        /// The global list.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Global", Justification = "This needs to match the JSLint property of the same name.")]
        IList<string> Global { get; }

        /// <summary>
        /// Gets a value indicating whether the source was JSON.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the source was JSON; otherwise, <c>false</c>.
        /// </value>
        bool Json { get; }

        /// <summary>
        /// Gets the HTML error report.
        /// </summary>
        /// <value>
        /// The HTML error report.
        /// </value>
        string ErrorReport { get; }

        /// <summary>
        /// Gets a value indicating whether the last error in this instance is a stopping error. A stopping error means that JSLint was not confident enough to continue.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the last error in this instance is a stopping error; otherwise, <c>false</c>.
        /// </value>
        bool Stop { get; }
    }
}
