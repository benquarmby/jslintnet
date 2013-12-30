namespace JSLintNet.Models
{
    /// <summary>
    /// Contains the details of a JSLint error.
    /// </summary>
    public interface IJSLintError
    {
        /// <summary>
        /// Gets the line (relative to 0) at which the lint was found.
        /// </summary>
        int Line { get; }

        /// <summary>
        /// Gets the character (relative to 0) at which the lint was found.
        /// </summary>
        int Character { get; }

        /// <summary>
        /// Gets the problem.
        /// </summary>
        string Reason { get; }

        /// <summary>
        /// Gets the text line in which the problem occurred.
        /// </summary>
        string Evidence { get; }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        string Code { get; }
    }
}
