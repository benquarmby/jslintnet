namespace JSLintNet.QualityTools.Expectations
{
    using System.Diagnostics;
    using System.Text;
    using Xunit.Sdk;

    public static class ExpectationHelper
    {
        public static void PassFail<T>(bool pass, IExpectation<T> expectation, params object[] args)
        {
            if (pass != expectation.Positive)
            {
                var methodName = new StackTrace().GetFrame(1).GetMethod().Name;
                var english = GetEnglishyPredicate(methodName, expectation, args);

                throw new AssertException(english);
            }
        }

        private static string GetEnglishyPredicate<T>(string methodName, IExpectation<T> expectation, params object[] args)
        {
            var first = true;
            var builder = new StringBuilder()
                .Append("Expected '")
                .Append(expectation.Actual)
                .Append("' ");

            if (!expectation.Positive)
            {
                builder.Append("not ");
            }

            foreach (var item in methodName)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    if (char.IsUpper(item))
                    {
                        builder.Append(' ');
                    }
                }

                builder.Append(char.ToLower(item));
            }

            if (args != null)
            {
                first = true;

                foreach (var item in args)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        builder.Append(',');
                    }

                    builder
                        .Append(" '")
                        .Append(item)
                        .Append('\'');
                }
            }

            builder.Append('.');

            return builder.ToString();
        }
    }
}
