namespace JSLintNet
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a context for running JSLint.
    /// </summary>
    public interface IJSLintContext : IDisposable
    {
        /// <summary>
        /// Validates the specified source using JSLint.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        /// A <see cref="IJSLintData" /> instance containing any validation warnings.
        /// </returns>
        IJSLintData Lint(string source);

        /// <summary>
        /// Validates the specified source using JSLint with the provided options.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// A <see cref="IJSLintData" /> instance containing any validation warnings.
        /// </returns>
        IJSLintData Lint(string source, JSLintOptions options);

        /// <summary>
        /// Validates the specified source using JSLint with the provided options and global variables.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        /// <param name="globalVariables">The global variables.</param>
        /// <returns>
        /// A <see cref="IJSLintData" /> instance containing any validation warnings.
        /// </returns>
        IJSLintData Lint(string source, JSLintOptions options, IList<string> globalVariables);
    }
}
