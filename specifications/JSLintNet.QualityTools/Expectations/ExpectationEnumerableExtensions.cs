namespace JSLintNet.QualityTools.Expectations
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public static class ExpectationEnumerableExtensions
    {
        public static void ToBeEmpty<E>(this IExpectation<E> expectation)
            where E : IEnumerable
        {
            ExpectationHelper.PassFail(!expectation.Actual.GetEnumerator().MoveNext(), expectation, expectation.Actual.ToString());
        }

        public static void ToContain<E, T>(this IExpectation<E> expectation, T expected)
            where E : IEnumerable<T>
        {
            ExpectationHelper.PassFail(expectation.Actual.Contains(expected), expectation, expectation.Actual.ToString());
        }
    }
}
