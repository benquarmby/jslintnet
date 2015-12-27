namespace JSLintNet
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains a JSLint result.
    /// </summary>
    public interface IJSLintData
    {
        /// <summary>
        /// Gets the edition of JSLint that did the analysis.
        /// </summary>
        /// <value>
        /// The edition.
        /// </value>
        string Edition { get; }

        /// <summary>
        /// Gets the directives.
        /// </summary>
        /// <value>
        /// The directives.
        /// </value>
        IList<IJSLintToken> Directives { get; }

        /// <summary>
        /// Gets the array of strings representing each of the imports.
        /// </summary>
        /// <value>
        /// The imports.
        /// </value>
        IList<string> Imports { get; }

        /// <summary>
        /// Gets a value indicating whether the file is a JSON text.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the file is a JSON text; otherwise, <c>false</c>.
        /// </value>
        bool Json { get; }

        /// <summary>
        /// Gets a value indicating whether an import or export statement was used.
        /// </summary>
        /// <value>
        ///   <c>true</c> if an import or export statement was used; otherwise, <c>false</c>.
        /// </value>
        bool Module { get; }

        /// <summary>
        /// Gets a value indicating whether no warnings were generated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if no warnings were generated; otherwise, <c>false</c>.
        /// </value>
        bool OK { get; }

        /// <summary>
        /// Gets the property object.
        /// </summary>
        /// <value>
        /// The property object.
        /// </value>
        IDictionary<string, int> Property { get; }

        /// <summary>
        /// Gets the property directive.
        /// </summary>
        /// <value>
        /// The property directive.
        /// </value>
        string PropertyDirective { get; }

        /// <summary>
        /// Gets a value indicating whether JSLint was unable to finish.
        /// </summary>
        /// <value>
        ///   <c>true</c> if JSLint was unable to finish; otherwise, <c>false</c>.
        /// </value>
        bool Stop { get; }

        /// <summary>
        /// Gets the array of warning objects.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        IList<IJSLintWarning> Warnings { get; }

        /// <summary>
        /// Gets the HTML report.
        /// </summary>
        /// <value>
        /// The HTML report.
        /// </value>
        string Report { get; }
    }
}
