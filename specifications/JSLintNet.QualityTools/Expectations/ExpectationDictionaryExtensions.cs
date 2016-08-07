namespace JSLintNet.QualityTools.Expectations
{
    using System.Collections;

    public static class ExpectationDictionaryExtensions
    {
        public static void ToContainKey(this IExpectation<IDictionary> expectation, object key)
        {
            ExpectationHelper.PassFail(expectation.Actual.Contains(key), expectation, key.ToString());
        }
    }
}
