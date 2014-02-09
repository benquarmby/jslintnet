namespace JSLintNet.QualityTools.Expectations
{
    public static class ExpectationBooleanExtensions
    {
        /// <summary>
        /// Matcher that tests if the actual is true.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        public static void ToBeTrue(this IExpectation<bool> expectation)
        {
            ExpectationHelper.PassFail(expectation.Actual, expectation);
        }

        /// <summary>
        /// Matcher that tests if the actual is false.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        public static void ToBeFalse(this IExpectation<bool> expectation)
        {
            ExpectationHelper.PassFail(!expectation.Actual, expectation);
        }

        /// <summary>
        /// Matcher that tests if the actual is true.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        public static void ToBeTrue(this IExpectation<bool?> expectation)
        {
            var actual = expectation.Actual;

            ExpectationHelper.PassFail(actual.GetValueOrDefault(), expectation);
        }

        /// <summary>
        /// Matcher that tests if the actual is false.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        public static void ToBeFalse(this IExpectation<bool?> expectation)
        {
            var actual = expectation.Actual;

            ExpectationHelper.PassFail(actual.HasValue && !actual.Value, expectation);
        }

        /// <summary>
        /// Matcher that tests if the actual is false or null.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        public static void ToBeNullOrFalse(this IExpectation<bool?> expectation)
        {
            var actual = expectation.Actual;

            ExpectationHelper.PassFail(!actual.GetValueOrDefault(), expectation);
        }
    }
}
