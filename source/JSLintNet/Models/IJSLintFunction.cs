namespace JSLintNet.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a JSLint function.
    /// </summary>
    public interface IJSLintFunction
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <value>
        /// The line.
        /// </value>
        int Line { get; }

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        int Level { get; }

        /// <summary>
        /// Gets the parameter list.
        /// </summary>
        /// <value>
        /// The parameter list.
        /// </value>
        IList<string> Parameter { get; }

        /// <summary>
        /// Gets the variable list.
        /// </summary>
        /// <value>
        /// The variable list.
        /// </value>
        IList<string> Var { get; }

        /// <summary>
        /// Gets the exception list.
        /// </summary>
        /// <value>
        /// The exception list.
        /// </value>
        IList<string> Exception { get; }

        /// <summary>
        /// Gets the closure list.
        /// </summary>
        /// <value>
        /// The closure list.
        /// </value>
        IList<string> Closure { get; }

        /// <summary>
        /// Gets the outer list.
        /// </summary>
        /// <value>
        /// The outer list.
        /// </value>
        IList<string> Outer { get; }

        /// <summary>
        /// Gets the global list.
        /// </summary>
        /// <value>
        /// The global list.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Global", Justification = "This needs to match the JSLint property of the same name.")]
        IList<string> Global { get; }

        /// <summary>
        /// Gets the label list.
        /// </summary>
        /// <value>
        /// The label list.
        /// </value>
        IList<string> Label { get; }
    }
}
