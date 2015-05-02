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
                    TolerateBitwiseOperators = true
                };

                var merge = new JSLintOptions()
                {
                    MaximumErrors = 33,
                    AssumeBrowser = true,
                    TolerateEval = false
                };

                target.Merge(merge);

                I.Expect(target.MaximumErrors).ToBe(33);
                I.Expect(target.AssumeBrowser).ToBeTrue();
                I.Expect(target.TolerateBitwiseOperators).ToBeTrue();
                I.Expect(target.TolerateEval).ToBeFalse();
                I.Expect(target.TolerateMessyWhitespace).ToBeNull();
            }
        }
    }
}
