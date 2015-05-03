namespace JSLintNet.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// Contains a JSLint result.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by JSON serializer.")]
    public class JSLintData : IJSLintData
    {
        /// <summary>
        /// Gets the edition of JSLint that did the analysis.
        /// </summary>
        /// <value>
        /// The edition.
        /// </value>
        [JsonProperty("edition")]
        public string Edition { get; private set; }

        /// <summary>
        /// Gets the array of strings representing each of the imports.
        /// </summary>
        /// <value>
        /// The imports.
        /// </value>
        [JsonProperty("imports")]
        public IList<string> Imports { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the file is a JSON text.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the file is a JSON text; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("json")]
        public bool Json { get; private set; }

        /// <summary>
        /// Gets a value indicating whether an import or export statement was used.
        /// </summary>
        /// <value>
        /// <c>true</c> if an import or export statement was used; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("module")]
        public bool Module { get; private set; }

        /// <summary>
        /// Gets a value indicating whether no warnings were generated.
        /// </summary>
        /// <value>
        /// <c>true</c> if no warnings were generated; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("ok")]
        public bool OK { get; private set; }

        /// <summary>
        /// Gets the property object.
        /// </summary>
        /// <value>
        /// The property object.
        /// </value>
        [JsonProperty("property")]
        public IDictionary<string, int> Property { get; private set; }

        /// <summary>
        /// Gets a value indicating whether JSLint was unable to finish.
        /// </summary>
        /// <value>
        /// <c>true</c> if JSLint was unable to finish; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("stop")]
        public bool Stop { get; private set; }

        /// <summary>
        /// Gets the array of warning objects.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        [JsonProperty("warnings")]
        public IList<IJSLintWarning> Warnings { get; private set; }

        /// <summary>
        /// Gets the HTML report.
        /// </summary>
        /// <value>
        /// The HTML report.
        /// </value>
        [JsonProperty("report")]
        public string Report { get; private set; }
    }
}
