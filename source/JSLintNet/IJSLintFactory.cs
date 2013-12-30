namespace JSLintNet
{
    /// <summary>
    /// Creates JSLint related services.
    /// </summary>
    public interface IJSLintFactory
    {
        /// <summary>
        /// Creates and returns an implementation of <see cref="IJSLintContext"/>.
        /// </summary>
        /// <returns>
        /// An implementation of <see cref="IJSLintContext"/>.
        /// </returns>
        IJSLintContext CreateContext();

        /// <summary>
        /// Creates and returns an implementation of <see cref="IJSLintReportBuilder"/>.
        /// </summary>
        /// <returns>
        /// An implementation of <see cref="IJSLintReportBuilder"/>.
        /// </returns>
        IJSLintReportBuilder CreateReportBuilder();
    }
}
