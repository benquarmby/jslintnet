namespace JSLintNet.QualityTools.Expectations
{
    using System;

    /// <summary>
    /// Static class used to expose the Expect method.
    /// </summary>
    public static class I
    {
        /// <summary>
        /// Creates a new expectation.
        /// </summary>
        /// <typeparam name="TActual">The type of the actual value.</typeparam>
        /// <param name="actual">The actual value.</param>
        /// <returns>
        /// The expectation.
        /// </returns>
        public static IReversibleExpectation<TActual> Expect<TActual>(TActual actual)
        {
            return new Expectation<TActual>(actual);
        }

        /// <summary>
        /// Creates a new delegate expectation.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>
        /// The expectation.
        /// </returns>
        public static IReversibleExpectation<Action> Expect(Action action)
        {
            return new Expectation<Action>(action);
        }
    }
}
