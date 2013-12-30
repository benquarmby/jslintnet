namespace JSLintNet.Specifications
{
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using Xunit;

    public class JSLintNetSettingsUnit
    {
        public class NormalizeIgnore : UnitBase
        {
            [Fact(DisplayName = "Should replace forward slashes with back slashes")]
            public void Spec01()
            {
                using (var testable = new NormalizeIgnoreTestable())
                {
                    var ignore = testable.Instance.Ignore;
                    ignore.Add(@"some/path");
                    ignore.Add(@"/some/other/path");
                    ignore.Add(@"other/path");

                    var actual = testable.Instance.NormalizeIgnore();

                    I.Expect(actual[0]).ToContain(@"some\path");
                    I.Expect(actual[1]).ToContain(@"\some\other\path");
                    I.Expect(actual[2]).ToContain(@"other\path");
                }
            }

            [Fact(DisplayName = "Should add directory separator prefix where absent")]
            public void Spec02()
            {
                using (var testable = new NormalizeIgnoreTestable())
                {
                    var ignore = testable.Instance.Ignore;
                    ignore.Add(@"some\path");
                    ignore.Add(@"\some\other\path");
                    ignore.Add(@"other\path");

                    var actual = testable.Instance.NormalizeIgnore();

                    I.Expect(actual[0]).ToStartWith(@"\");
                    I.Expect(actual[2]).ToStartWith(@"\");
                }
            }

            [Fact(DisplayName = "Should not add directory separator suffix to js file")]
            public void Spec03()
            {
                using (var testable = new NormalizeIgnoreTestable())
                {
                    var ignore = testable.Instance.Ignore;
                    ignore.Add(@"path\to\file.js");

                    var actual = testable.Instance.NormalizeIgnore();

                    I.Expect(actual[0]).ToStartWith(@"\path\to\file.js");
                }
            }

            [Fact(DisplayName = "Should not add directory separator suffix to json file")]
            public void Spec04()
            {
                using (var testable = new NormalizeIgnoreTestable())
                {
                    var ignore = testable.Instance.Ignore;
                    ignore.Add(@"root\data.json");

                    var actual = testable.Instance.NormalizeIgnore();

                    I.Expect(actual[0]).ToBe(@"\root\data.json");
                }
            }

            [Fact(DisplayName = "Should add directory separator suffix where absent")]
            public void Spec05()
            {
                using (var testable = new NormalizeIgnoreTestable())
                {
                    var ignore = testable.Instance.Ignore;
                    ignore.Add(@"path\to");
                    ignore.Add(@"another\path\");
                    ignore.Add(@"looks/like/a.file");

                    var actual = testable.Instance.NormalizeIgnore();

                    I.Expect(actual[0]).ToEndWith(@"\");
                    I.Expect(actual[2]).ToEndWith(@"\");
                }
            }

            private class NormalizeIgnoreTestable : JSLintNetSettingsTestableBase
            {
            }
        }

        private abstract class JSLintNetSettingsTestableBase : TestableBase<JSLintNetSettings>
        {
        }
    }
}
