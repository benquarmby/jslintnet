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
        /// <typeparam name="T">The type of the actual value.</typeparam>
        /// <param name="actual">The actual value.</param>
        /// <returns>
        /// The expectation.
        /// </returns>
        public static IReversibleExpectation<T> Expect<T>(T actual)
        {
            return new Expectation<T>(actual);
        }

        public static IReversibleExpectation<Action> Expect(Action actual)
        {
            return new Expectation<Action>(actual);
        }
    }
}
