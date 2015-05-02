namespace JSLintNet.Models
{
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// Contains the details of a JSLint error.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by JSON serializer.")]
    public class JSLintWarning : IJSLintWarning
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintWarning"/> class.
        /// </summary>
        [JsonConstructor]
        internal JSLintWarning()
        {
        }

        /// <summary>
        /// Gets the line (relative to 0) at which the lint was found.
        /// </summary>
        [JsonProperty("line")]
        public int Line { get; private set; }

        /// <summary>
        /// Gets the column (relative to 0) at which the lint was found.
        /// </summary>
        [JsonProperty("column")]
        public int Column { get; private set; }

        /// <summary>
        /// Gets the problem.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; private set; }

        /// <summary>
        /// Gets the text line in which the problem occurred.
        /// </summary>
        [JsonProperty("evidence")]
        public string Evidence { get; private set; }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        [JsonProperty("code")]
        public string Code { get; private set; }
    }
}
