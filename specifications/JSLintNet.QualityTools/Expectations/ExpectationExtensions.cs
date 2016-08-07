namespace JSLintNet.QualityTools.Expectations
{
    public static class ExpectationExtensions
    {
        public static void ToBe<TActual>(this IExpectation<TActual> expectation, TActual expected)
        {
            var pass = expectation.Actual == null ? expected == null : expectation.Actual.Equals(expected);

            ExpectationHelper.PassFail(pass, expectation, expected);
        }
    }
}
