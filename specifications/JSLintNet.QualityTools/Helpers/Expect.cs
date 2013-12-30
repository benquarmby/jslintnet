namespace JSLintNet.QualityTools.Helpers
{
    using System.Text.RegularExpressions;
    using Xunit.Sdk;

    public static class Expect
    {
        public static void ToMatch(string pattern, string actual)
        {
            ToMatch(new Regex(pattern), actual);
        }

        public static void ToMatch(string pattern, RegexOptions options, string actual)
        {
            ToMatch(new Regex(pattern, options), actual);
        }

        public static void ToMatch(Regex pattern, string actual)
        {
            if (!pattern.IsMatch(actual))
            {
                throw new AssertException("Expected \"" + actual + "\" to match pattern \"" + pattern.ToString() + "\"");
            }
        }

        public static void ToStartWith(string value, string actual)
        {
            if (!actual.StartsWith(value))
            {
                throw new AssertException("Expected \"" + actual + "\" to start with \"" + value + "\"");
            }
        }

        public static void NotToStartWith(string value, string actual)
        {
            if (actual.StartsWith(value))
            {
                throw new AssertException("Expected \"" + actual + "\" not to start with \"" + value + "\"");
            }
        }

        public static void ToEndWith(string value, string actual)
        {
            if (!actual.EndsWith(value))
            {
                throw new AssertException("Expected \"" + actual + "\" to end with \"" + value + "\"");
            }
        }

        public static void NotToEndWith(string value, string actual)
        {
            if (actual.EndsWith(value))
            {
                throw new AssertException("Expected \"" + actual + "\" not to end with \"" + value + "\"");
            }
        }

        public static void ToBeGreaterThan(int expected, int actual)
        {
            if (!(actual > expected))
            {
                throw new AssertException("Expected \"" + actual + "\" to be greater than \"" + expected + "\"");
            }
        }

        public static void ToBeLessThan(int expected, int actual)
        {
            if (!(actual < expected))
            {
                throw new AssertException("Expected \"" + actual + "\" to be less than \"" + expected + "\"");
            }
        }
    }
}
