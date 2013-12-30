namespace JSLintNet
{
    using System;
    using JSLintNet.Models;

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
        /// A <see cref="IJSLintData"/> containing any validation errors.
        /// </returns>
        IJSLintData Lint(string source);

        /// <summary>
        /// Validates the specified source using JSLint with the provided options.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// A <see cref="IJSLintData" /> containing any validation errors.
        /// </returns>
        IJSLintData Lint(string source, JSLintOptions options);
    }
}
