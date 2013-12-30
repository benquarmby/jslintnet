namespace JSLintNet.QualityTools.Expectations
{
    /// <summary>
    /// An expectation that can be reversed to a negative state.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    public interface IReversibleExpectation<T> : IExpectation<T>
    {
        /// <summary>
        /// Gets the negative expectation.
        /// </summary>
        /// <value>
        /// The negative expectation.
        /// </value>
        IExpectation<T> Not { get; }
    }
}
