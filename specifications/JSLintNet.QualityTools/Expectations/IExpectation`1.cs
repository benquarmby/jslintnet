namespace JSLintNet.QualityTools.Expectations
{
    /// <summary>
    /// An expectation.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    public interface IExpectation<T>
    {
        /// <summary>
        /// Gets the actual value.
        /// </summary>
        /// <value>
        /// The actual value.
        /// </value>
        T Actual { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IExpectation`1" /> is positive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if positive; otherwise, <c>false</c>.
        /// </value>
        bool Positive { get; }

        /// <summary>
        /// Matcher that determines whether the actual and expected values are the same.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        void ToBe(T expected);

        /// <summary>
        /// Matcher that determines whether the actual value is null.
        /// </summary>
        void ToBeNull();

        /// <summary>
        /// Matcher that determines whether the actual value type is the same as the given type.
        /// </summary>
        /// <typeparam name="S">The type to compare.</typeparam>
        void ToBeOfType<S>();
    }
}
