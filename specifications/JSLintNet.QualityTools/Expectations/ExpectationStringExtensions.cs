namespace JSLintNet.QualityTools.Expectations
{
    using System.Text.RegularExpressions;

    public static class ExpectationStringExtensions
    {
        /// <summary>
        /// Matcher that passes if the actual starts with the expected.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="expected">The expected value.</param>
        public static void ToStartWith(this IExpectation<string> expectation, string expected)
        {
            var pass = expectation.Actual != null && expectation.Actual.StartsWith(expected);

            ExpectationHelper.PassFail(pass, expectation, expected);
        }

        /// <summary>
        /// Matcher that passes if the actual ends with the expected.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="expected">The expected value.</param>
        public static void ToEndWith(this IExpectation<string> expectation, string expected)
        {
            var pass = expectation.Actual != null && expectation.Actual.EndsWith(expected);

            ExpectationHelper.PassFail(pass, expectation, expected);
        }

        /// <summary>
        /// Matcher that passes if the actual contains the expected.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="expected">The expected value.</param>
        public static void ToContain(this IExpectation<string> expectation, string expected)
        {
            var pass = expectation.Actual != null && expectation.Actual.Contains(expected);

            ExpectationHelper.PassFail(pass, expectation, expected);
        }

        public static void ToMatch(this IExpectation<string> expectation, string pattern)
        {
            var regex = new Regex(pattern);

            ExpectationHelper.PassFail(regex.IsMatch(expectation.Actual), expectation, pattern.ToString());
        }

        public static void ToMatch(this IExpectation<string> expectation, string pattern, RegexOptions options)
        {
            var regex = new Regex(pattern, options);

            ExpectationHelper.PassFail(regex.IsMatch(expectation.Actual), expectation, pattern.ToString());
        }

        public static void ToMatch(this IExpectation<string> expectation, Regex pattern)
        {
            ExpectationHelper.PassFail(pattern.IsMatch(expectation.Actual), expectation, pattern.ToString());
        }
    }
}
