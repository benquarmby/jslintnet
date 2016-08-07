namespace JSLintNet.QualityTools.Expectations
{
    /// <summary>
    /// An expectation that can be reversed to a negative state.
    /// </summary>
    /// <typeparam name="TActual">Any type.</typeparam>
    public interface IReversibleExpectation<out TActual> : IExpectation<TActual>
    {
        /// <summary>
        /// Gets the negative expectation.
        /// </summary>
        /// <value>
        /// The negative expectation.
        /// </value>
        IExpectation<TActual> Not { get; }
    }
}
