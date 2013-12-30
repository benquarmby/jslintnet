namespace JSLintNet.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a JSLint function.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by JSON serializer.")]
    public class JSLintFunction : IJSLintFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintFunction"/> class.
        /// </summary>
        [JsonConstructor]
        internal JSLintFunction()
        {
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("name")]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <value>
        /// The line.
        /// </value>
        [JsonProperty("line")]
        public int Line { get; private set; }

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        [JsonProperty("level")]
        public int Level { get; private set; }

        /// <summary>
        /// Gets the parameter list.
        /// </summary>
        /// <value>
        /// The parameter list.
        /// </value>
        [JsonProperty("parameter")]
        public IList<string> Parameter { get; private set; }

        /// <summary>
        /// Gets the variable list.
        /// </summary>
        /// <value>
        /// The variable list.
        /// </value>
        [JsonProperty("var")]
        public IList<string> Var { get; private set; }

        /// <summary>
        /// Gets the exception list.
        /// </summary>
        /// <value>
        /// The exception list.
        /// </value>
        [JsonProperty("exception")]
        public IList<string> Exception { get; private set; }

        /// <summary>
        /// Gets the closure list.
        /// </summary>
        /// <value>
        /// The closure list.
        /// </value>
        [JsonProperty("closure")]
        public IList<string> Closure { get; private set; }

        /// <summary>
        /// Gets the outer list.
        /// </summary>
        /// <value>
        /// The outer list.
        /// </value>
        [JsonProperty("outer")]
        public IList<string> Outer { get; private set; }

        /// <summary>
        /// Gets the global list.
        /// </summary>
        /// <value>
        /// The global list.
        /// </value>
        [JsonProperty("global")]
        public IList<string> Global { get; private set; }

        /// <summary>
        /// Gets the label list.
        /// </summary>
        /// <value>
        /// The label list.
        /// </value>
        [JsonProperty("label")]
        public IList<string> Label { get; private set; }
    }
}
