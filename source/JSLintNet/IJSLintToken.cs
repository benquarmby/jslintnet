namespace JSLintNet
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains the details of a JSLint token.
    /// </summary>
    public interface IJSLintToken
    {
        /// <summary>
        /// Gets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        string Id { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IJSLintToken"/> is an identifier.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this <see cref="IJSLintToken"/> is an identifier; otherwise, <c>false</c>.
        /// </value>
        bool Identifier { get; }

        /// <summary>
        /// Gets the start column index.
        /// </summary>
        /// <value>
        /// The start column index.
        /// </value>
        int From { get; }

        /// <summary>
        /// Gets the end column index.
        /// </summary>
        /// <value>
        /// The end column index.
        /// </value>
        int Thru { get; }

        /// <summary>
        /// Gets the line index.
        /// </summary>
        /// <value>
        /// The line index.
        /// </value>
        int Line { get; }

        /// <summary>
        /// Gets the token value.
        /// </summary>
        /// <value>
        /// The token value.
        /// </value>
        IList<string> Value { get; }

        /// <summary>
        /// Gets the directive type.
        /// </summary>
        /// <value>
        /// The directive type.
        /// </value>
        string Directive { get; }
    }
}
