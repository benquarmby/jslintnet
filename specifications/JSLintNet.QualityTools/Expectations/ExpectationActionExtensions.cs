namespace JSLintNet.QualityTools.Expectations
{
    using System;

    public static class ExpectationActionExtensions
    {
        public static T ToThrow<T>(this IExpectation<Action> expectation)
            where T : Exception
        {
            var typeOfT = typeof(T);
            var exception = Capture(expectation.Actual);

            ExpectationHelper.PassFail(exception != null && typeOfT.IsAssignableFrom(exception.GetType()), expectation, typeOfT.FullName);

            return (T)exception;
        }

        public static Exception ToThrow(this IExpectation<Action> expectation)
        {
            var exception = Capture(expectation.Actual);

            ExpectationHelper.PassFail(exception != null, expectation);

            return exception;
        }

        private static Exception Capture(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }
    }
}
