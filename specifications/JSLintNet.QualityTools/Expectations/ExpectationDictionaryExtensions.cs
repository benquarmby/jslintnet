namespace JSLintNet.QualityTools.Expectations
{
    using System.Collections;

    public static class ExpectationDictionaryExtensions
    {
        public static void ToContainKey<E>(this IExpectation<E> expectation, object key)
            where E : IDictionary
        {
            ExpectationHelper.PassFail(expectation.Actual.Contains(key), expectation, key.ToString());
        }
    }
}
