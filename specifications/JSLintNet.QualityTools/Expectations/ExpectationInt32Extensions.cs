namespace JSLintNet.QualityTools.Expectations
{
    public static class ExpectationInt32Extensions
    {
        public static void ToBeGreaterThan(this IExpectation<int> expectation, int expected)
        {
            ExpectationHelper.PassFail(expectation.Actual > expected, expectation, expected);
        }

        public static void ToBeGreaterThanOrEqualTo(this IExpectation<int> expectation, int expected)
        {
            ExpectationHelper.PassFail(expectation.Actual >= expected, expectation, expected);
        }

        public static void ToBeLessThan(this IExpectation<int> expectation, int expected)
        {
            ExpectationHelper.PassFail(expectation.Actual < expected, expectation, expected);
        }

        public static void ToBeLessThanOrEqualTo(this IExpectation<int> expectation, int expected)
        {
            ExpectationHelper.PassFail(expectation.Actual <= expected, expectation, expected);
        }
    }
}
