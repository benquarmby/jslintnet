namespace JSLintNet
{
    /// <summary>
    /// Contains the details of a JSLint error.
    /// </summary>
    public interface IJSLintWarning
    {
        /// <summary>
        /// Gets the line (relative to 0) at which the lint was found.
        /// </summary>
        int Line { get; }

        /// <summary>
        /// Gets the character (relative to 0) at which the lint was found.
        /// </summary>
        int Column { get; }

        /// <summary>
        /// Gets the problem.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        string Code { get; }
    }
}
