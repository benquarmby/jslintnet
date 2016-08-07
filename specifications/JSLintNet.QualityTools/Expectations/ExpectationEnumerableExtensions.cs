namespace JSLintNet.QualityTools.Expectations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ExpectationEnumerableExtensions
    {
        public static void ToBeEmpty<TItem>(this IExpectation<IEnumerable<TItem>> expectation)
        {
            ExpectationHelper.PassFail(!expectation.Actual.Any(), expectation);
        }

        public static void ToContain<TItem>(this IExpectation<IEnumerable<TItem>> expectation, TItem expected)
        {
            ExpectationHelper.PassFail(expectation.Actual.Contains(expected), expectation, expected.ToString());
        }

        public static void ToContain<TItem>(this IExpectation<IEnumerable<TItem>> expectation, Func<TItem, bool> predicate)
        {
            ExpectationHelper.PassFail(expectation.Actual.Any(predicate), expectation, predicate.ToString());
        }
    }
}
