namespace JSLintNet
{
    /// <summary>
    /// Creates JSLint related services.
    /// </summary>
    public class JSLintFactory : IJSLintFactory
    {
        /// <summary>
        /// Creates and returns an implementation of <see cref="IJSLintContext"/>.
        /// </summary>
        /// <returns>
        /// An implementation of <see cref="IJSLintContext"/>.
        /// </returns>
        public IJSLintContext CreateContext()
        {
            return new JSLintContext();
        }
    }
}
