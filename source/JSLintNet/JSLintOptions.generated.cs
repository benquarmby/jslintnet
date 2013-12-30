namespace JSLintNet
{
    using System.CodeDom.Compiler;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides a simple way to create and manage the options used by JSLint.
    /// </summary>
    [GeneratedCode("TextTemplatingFileGenerator", "")]
    public partial class JSLintOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether assignment expressions should be allowed.
        /// </summary>
        /// <value>
        /// <c>true</c> if assignment expressions should be allowed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "ass" option.
        /// </remarks>
        [JsonProperty("ass")]
        public bool? TolerateAssignmentExpressions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether bitwise operators should be allowed.
        /// </summary>
        /// <value>
        /// <c>true</c> if bitwise operators should be allowed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "bitwise" option.
        /// </remarks>
        [JsonProperty("bitwise")]
        public bool? TolerateBitwiseOperators { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the standard browser globals should be predefined.
        /// </summary>
        /// <value>
        /// <c>true</c> if the standard browser globals should be predefined; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "browser" option.
        /// </remarks>
        [JsonProperty("browser")]
        public bool? AssumeBrowser { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Google Closure idioms should be tolerated.
        /// </summary>
        /// <value>
        /// <c>true</c> if Google Closure idioms should be tolerated; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "closure" option.
        /// </remarks>
        [JsonProperty("closure")]
        public bool? TolerateGoogleClosure { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the continuation statement should be tolerated.
        /// </summary>
        /// <value>
        /// <c>true</c> if the continuation statement should be tolerated; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "continue" option.
        /// </remarks>
        [JsonProperty("continue")]
        public bool? TolerateContinue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Couch DB globals should be predefined.
        /// </summary>
        /// <value>
        /// <c>true</c> if Couch DB globals should be predefined; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "couch" option.
        /// </remarks>
        [JsonProperty("couch")]
        public bool? AssumeCouchDB { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether debugger statements should be allowed.
        /// </summary>
        /// <value>
        /// <c>true</c> if debugger statements should be allowed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "debug" option.
        /// </remarks>
        [JsonProperty("debug")]
        public bool? TolerateDebuggerStatements { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if browser globals that are useful in development should be predefined.
        /// </summary>
        /// <value>
        /// <c>true</c> if if browser globals that are useful in development should be predefined; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "devel" option.
        /// </remarks>
        [JsonProperty("devel")]
        public bool? AssumeConsole { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether == should be allowed.
        /// </summary>
        /// <value>
        /// <c>true</c> if == should be allowed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "eqeq" option.
        /// </remarks>
        [JsonProperty("eqeq")]
        public bool? TolerateDoubleEquals { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether eval should be allowed.
        /// </summary>
        /// <value>
        /// <c>true</c> if eval should be allowed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "evil" option.
        /// </remarks>
        [JsonProperty("evil")]
        public bool? TolerateEval { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether for in statements need not filter.
        /// </summary>
        /// <value>
        /// <c>true</c> if for in statements need not filter; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "forin" option.
        /// </remarks>
        [JsonProperty("forin")]
        public bool? TolerateUnfilteredForIn { get; set; }

        /// <summary>
        /// Gets or sets the indentation factor.
        /// </summary>
        /// <value>
        /// The indentation factor.
        /// </value>
        /// <remarks>
        /// JSLint "indent" option.
        /// </remarks>
        [JsonProperty("indent")]
        public int? IndentationFactor { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of errors to allow.
        /// </summary>
        /// <value>
        /// The maximum number of errors to allow.
        /// </value>
        /// <remarks>
        /// JSLint "maxerr" option.
        /// </remarks>
        [JsonProperty("maxerr")]
        public int? MaximumErrors { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of a source line.
        /// </summary>
        /// <value>
        /// The maximum length of a source line.
        /// </value>
        /// <remarks>
        /// JSLint "maxlen" option.
        /// </remarks>
        [JsonProperty("maxlen")]
        public int? MaximumLineLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether constructor names capitalization is ignored.
        /// </summary>
        /// <value>
        /// <c>true</c> if constructor names capitalization is ignored; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "newcap" option.
        /// </remarks>
        [JsonProperty("newcap")]
        public bool? TolerateUncapitalizedConstructors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Node.js globals should be predefined.
        /// </summary>
        /// <value>
        /// <c>true</c> if Node.js globals should be predefined; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "node" option.
        /// </remarks>
        [JsonProperty("node")]
        public bool? AssumeNode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether names may have dangling _.
        /// </summary>
        /// <value>
        /// <c>true</c> if names may have dangling _; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "nomen" option.
        /// </remarks>
        [JsonProperty("nomen")]
        public bool? TolerateDanglingUnderscores { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the scan should stop on first error.
        /// </summary>
        /// <value>
        /// <c>true</c> if the scan should stop on first error; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "passfail" option.
        /// </remarks>
        [JsonProperty("passfail")]
        public bool? StopOnFirstError { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether increment/decrement should be allowed.
        /// </summary>
        /// <value>
        /// <c>true</c> if increment/decrement should be allowed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "plusplus" option.
        /// </remarks>
        [JsonProperty("plusplus")]
        public bool? TolerateIncrementDecrement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all property names must be declared with /*properties*/.
        /// </summary>
        /// <value>
        /// <c>true</c> if all property names must be declared with /*properties*/; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "properties" option.
        /// </remarks>
        [JsonProperty("properties")]
        public bool? PropertiesDeclared { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the . should be allowed in regexp literals.
        /// </summary>
        /// <value>
        /// <c>true</c> if the . should be allowed in regexp literals; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "regexp" option.
        /// </remarks>
        [JsonProperty("regexp")]
        public bool? TolerateInsecureRegExp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Rhino environment globals should be predefined.
        /// </summary>
        /// <value>
        /// <c>true</c> if the Rhino environment globals should be predefined; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "rhino" option.
        /// </remarks>
        [JsonProperty("rhino")]
        public bool? AssumeRhino { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether unused parameters should be tolerated.
        /// </summary>
        /// <value>
        /// <c>true</c> if unused parameters should be tolerated; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "unparam" option.
        /// </remarks>
        [JsonProperty("unparam")]
        public bool? TolerateUnusedParameters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ES5 'use strict'; pragma is not required.
        /// </summary>
        /// <value>
        /// <c>true</c> if the ES5 'use strict'; pragma is not required; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "sloppy" option.
        /// </remarks>
        [JsonProperty("sloppy")]
        public bool? TolerateMissingUseStrict { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether blocking ('...Sync') methods can be used.
        /// </summary>
        /// <value>
        /// <c>true</c> if blocking ('...Sync') methods can be used; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "stupid" option.
        /// </remarks>
        [JsonProperty("stupid")]
        public bool? TolerateStupidPractices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all forms of subscript notation are tolerated.
        /// </summary>
        /// <value>
        /// <c>true</c> if all forms of subscript notation are tolerated; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "sub" option.
        /// </remarks>
        [JsonProperty("sub")]
        public bool? TolerateInefficientSubscripting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether TODO comments are tolerated.
        /// </summary>
        /// <value>
        /// <c>true</c> if TODO comments are tolerated; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "todo" option.
        /// </remarks>
        [JsonProperty("todo")]
        public bool? TolerateToDoComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multiple var statements per function should be allowed.
        /// </summary>
        /// <value>
        /// <c>true</c> if multiple var statements per function should be allowed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "vars" option.
        /// </remarks>
        [JsonProperty("vars")]
        public bool? TolerateManyVarStatements { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sloppy whitespace is tolerated.
        /// </summary>
        /// <value>
        /// <c>true</c> if sloppy whitespace is tolerated; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// JSLint "white" option.
        /// </remarks>
        [JsonProperty("white")]
        public bool? TolerateMessyWhitespace { get; set; }
    }
}
