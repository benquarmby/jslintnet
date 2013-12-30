namespace JSLintNet
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    public partial class JSLintOptions
    {
        private Dictionary<string, bool> predefinedGlobals;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintOptions"/> class.
        /// </summary>
        public JSLintOptions()
        {
            this.predefinedGlobals = new Dictionary<string, bool>();
        }

        /// <summary>
        /// Gets the predefined globals.
        /// </summary>
        /// <value>
        /// The predefined globals.
        /// </value>
        /// <remarks>
        /// JSLint "predef" option.
        /// </remarks>
        [JsonProperty("predef")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "JSLint option names are non words.")]
        public Dictionary<string, bool> PredefinedGlobals
        {
            get
            {
                return this.predefinedGlobals;
            }
        }
    }
}
