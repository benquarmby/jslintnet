namespace JSLintNet.Settings
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    /// <summary>
    /// The type of output for JSLint errors
    /// </summary>
    internal enum Output
    {
        /// <summary>
        /// Output JSLint errors as errors.
        /// </summary>
        Error,

        /// <summary>
        /// Output JSLint errors as warnings.
        /// </summary>
        Warning,

        /// <summary>
        /// Output JSLint errors as messages.
        /// </summary>
        Message
    }

    /// <summary>
    /// Represents the settings available to the JSLint.NET suite.
    /// </summary>
    internal partial class JSLintNetSettings
    {
        /// <summary>
        /// The standard file name for JSLint.NET settings.
        /// </summary>
        public const string FileName = "JSLintNet.json";

        /// <summary>
        /// The default error limit.
        /// </summary>
        public const int DefaultErrorLimit = 1000;

        /// <summary>
        /// The default file limit.
        /// </summary>
        public const int DefaultFileLimit = 1000;

        /// <summary>
        /// The hard exception limit.
        /// </summary>
        public const int ExceptionLimit = 50;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintNetSettings"/> class.
        /// </summary>
        public JSLintNetSettings()
        {
            this.Ignore = new List<string>();
            this.Files = new List<string>();
        }

        /// <summary>
        /// Gets the list of all files used to create this instance.
        /// </summary>
        /// <value>
        /// The list of all files used to create this instance.
        /// </value>
        [JsonIgnore]
        public IList<string> Files { get; private set; }

        /// <summary>
        /// Gets or sets the output type.
        /// </summary>
        /// <value>
        /// The output type.
        /// </value>
        [JsonProperty("output")]
        public Output? Output { get; set; }

        /// <summary>
        /// Gets the list of paths to ignore.
        /// </summary>
        /// <value>
        /// The list of paths to ignore.
        /// </value>
        [JsonProperty("ignore")]
        public IList<string> Ignore { get; private set; }

        /// <summary>
        /// Gets or sets the JSLint options.
        /// </summary>
        /// <value>
        /// The JSLint options.
        /// </value>
        [JsonProperty("options")]
        public JSLintOptions Options { get; set; }

        /// <summary>
        /// Normalizes the ignore list to a new clone.
        /// </summary>
        /// <returns>A normalized clone of the ignore list.</returns>
        public IList<string> NormalizeIgnore()
        {
            var clone = new List<string>();

            if (this.Ignore != null)
            {
                var directorySeparator = Path.DirectorySeparatorChar.ToString();

                for (int i = 0; i < this.Ignore.Count; i++)
                {
                    var normalized = this.Ignore[i];

                    normalized = normalized.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                    if (!normalized.StartsWith(directorySeparator))
                    {
                        normalized = directorySeparator + normalized;
                    }

                    if (!JSLint.CanLint(normalized) && !normalized.EndsWith(directorySeparator))
                    {
                        normalized += directorySeparator;
                    }

                    clone.Add(normalized);
                }
            }

            return clone;
        }

        /// <summary>
        /// Gets the error limit for this instance or the default if one was not defined.
        /// </summary>
        /// <returns>
        /// The error limit for this instance or the default if one was not defined.
        /// </returns>
        public int ErrorLimitOrDefault()
        {
            var nullable = this.ErrorLimit;

            return nullable.HasValue && nullable.Value > 0 ? nullable.Value : JSLintNetSettings.DefaultErrorLimit;
        }

        /// <summary>
        /// Gets the file limit for this instance or the default if one was not defined.
        /// </summary>
        /// <returns>
        /// The file limit for this instance or the default if one was not defined.
        /// </returns>
        public int FileLimitOrDefault()
        {
            var nullable = this.FileLimit;

            return nullable.HasValue && nullable.Value > 0 ? nullable.Value : JSLintNetSettings.DefaultFileLimit;
        }

        /// <summary>
        /// Merges the specified settings into this instance.
        /// </summary>
        /// <param name="merge">The settings to merge.</param>
        public void Merge(JSLintNetSettings merge)
        {
            this.MergeOutput(merge);
            this.MergeOptions(merge);
            this.MergeIgnore(merge);
            this.MergeRoot(merge);
        }

        private void MergeOutput(JSLintNetSettings merge)
        {
            if (merge.Output.HasValue)
            {
                this.Output = merge.Output;
            }
        }

        private void MergeOptions(JSLintNetSettings merge)
        {
            if (merge.Options == null)
            {
                return;
            }

            if (this.Options == null)
            {
                this.Options = merge.Options;

                return;
            }

            this.Options.Merge(merge.Options);
        }

        private void MergeIgnore(JSLintNetSettings merge)
        {
            if (merge.Ignore == null || merge.Ignore.Count < 1)
            {
                return;
            }

            if (this.Ignore == null || this.Ignore.Count < 1)
            {
                this.Ignore = merge.Ignore;

                return;
            }

            foreach (var ignore in merge.Ignore)
            {
                if (!this.Ignore.Contains(ignore))
                {
                    this.Ignore.Add(ignore);
                }
            }
        }
    }
}
