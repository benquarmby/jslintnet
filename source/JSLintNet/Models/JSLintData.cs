namespace JSLintNet.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// Contains the result of a JSLint validation.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by JSON serializer.")]
    public class JSLintData : IJSLintData
    {
        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        [JsonProperty("warnings")]
        public IList<IJSLintWarning> Warnings { get; private set; }

        /// <summary>
        /// Gets the functions.
        /// </summary>
        /// <value>
        /// The functions.
        /// </value>
        [JsonProperty("functions")]
        public IList<IJSLintFunction> Functions { get; private set; }

        /// <summary>
        /// Gets the global list.
        /// </summary>
        /// <value>
        /// The global list.
        /// </value>
        [JsonProperty("global")]
        public IList<string> Global { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the source was JSON.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the source was JSON; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("json")]
        public bool Json { get; private set; }

        /// <summary>
        /// Gets the HTML error report.
        /// </summary>
        /// <value>
        /// The HTML error report.
        /// </value>
        [JsonProperty("error_report")]
        public string ErrorReport { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the last error in this instance is a stopping error. A stopping error means that JSLint was not confident enough to continue.
        /// </summary>
        /// <value>
        /// <c>true</c> if the last error in this instance is a stopping error; otherwise, <c>false</c>.
        /// </value>
        public bool Stop { get; private set; }
    }
}
