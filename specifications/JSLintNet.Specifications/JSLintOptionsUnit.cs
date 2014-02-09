namespace JSLintNet.Specifications
{
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using Xunit;

    public class JSLintOptionsUnit
    {
        public class Merge : UnitBase
        {
            [Fact(DisplayName = "Should merge options")]
            public void Spec01()
            {
                var target = new JSLintOptions()
                {
                    MaximumErrors = 22,
                    AssumeBrowser = false,
                    TolerateUnusedParameters = true
                };

                var merge = new JSLintOptions()
                {
                    MaximumErrors = 33,
                    AssumeBrowser = true,
                    TolerateDebuggerStatements = false
                };

                target.Merge(merge);

                I.Expect(target.MaximumErrors).ToBe(33);
                I.Expect(target.AssumeBrowser).ToBeTrue();
                I.Expect(target.TolerateUnusedParameters).ToBeTrue();
                I.Expect(target.TolerateDebuggerStatements).ToBeFalse();
                I.Expect(target.TolerateStupidPractices).ToBeNull();
            }

            [Fact(DisplayName = "Should add unique globals")]
            public void Spec02()
            {
                var target = new JSLintOptions();
                target.PredefinedGlobals.Add("jQuery", false);

                var merge = new JSLintOptions();
                merge.PredefinedGlobals.Add("$", false);

                target.Merge(merge);

                I.Expect(target.PredefinedGlobals).ToContainKey("$");
                I.Expect(target.PredefinedGlobals).ToContainKey("jQuery");
            }

            [Fact(DisplayName = "Should overwrite duplicate globals")]
            public void Spec03()
            {
                var target = new JSLintOptions();
                target.PredefinedGlobals.Add("jasmine", false);

                var merge = new JSLintOptions();
                merge.PredefinedGlobals.Add("jasmine", true);

                target.Merge(merge);

                I.Expect(target.PredefinedGlobals["jasmine"]).ToBeTrue();
            }
        }
    }
}
