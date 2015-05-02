namespace JSLintNet.Specifications.UI
{
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using JSLintNet.Settings;
    using JSLintNet.UI.ViewModels;
    using Xunit;

    public class SettingsViewModelUnit
    {
        public class PredefinedGlobalsGet : UnitBase
        {
            [Fact(DisplayName = "Should convert predefined globals in model to space separated string")]
            public void Spec01()
            {
                using (var testable = new GlobalVariablesTestable())
                {
                    var globals = testable.Model.GlobalVariables;
                    globals.Add("a");
                    globals.Add("b");
                    globals.Add("c");
                    globals.Add("d");

                    var actual = testable.Instance.GlobalVariables;

                    I.Expect(actual).ToBe("a b c d");
                }
            }
        }

        public class PredefinedGlobalsSet : UnitBase
        {
            [Fact(DisplayName = "Should add new predefined globals to model")]
            public void Spec01()
            {
                using (var testable = new GlobalVariablesTestable())
                {
                    var actual = testable.Model.GlobalVariables;
                    actual.Add("a");

                    testable.Instance.GlobalVariables = "a b";

                    I.Expect(actual.Count).ToBe(2);
                    I.Expect(actual.Contains("a"));
                    I.Expect(actual.Contains("b"));
                }
            }

            [Fact(DisplayName = "Should add predefined globals separated by whitespace, commas, semi-colons or quotes")]
            public void Spec02()
            {
                using (var testable = new GlobalVariablesTestable())
                {
                    var actual = testable.Model.GlobalVariables;
                    testable.Instance.GlobalVariables = "a b\tc\nd,e;f'g\"h";

                    I.Expect(actual.Count).ToBe(8);
                    I.Expect(actual.Contains("a"));
                    I.Expect(actual.Contains("h"));
                }
            }

            [Fact(DisplayName = "Should remove missing predefined globals from model")]
            public void Spec03()
            {
                using (var testable = new GlobalVariablesTestable())
                {
                    var actual = testable.Model.GlobalVariables;
                    actual.Add("a");
                    actual.Add("b");
                    actual.Add("c");
                    actual.Add("d");

                    testable.Instance.GlobalVariables = "a d";

                    I.Expect(actual.Count).ToBe(2);
                    I.Expect(actual.Contains("b"));
                    I.Expect(actual.Contains("c"));
                }
            }

            [Fact(DisplayName = "Should clear globals from model when set to null")]
            public void Spec05()
            {
                using (var testable = new GlobalVariablesTestable())
                {
                    var actual = testable.Model.GlobalVariables;
                    actual.Add("a");
                    actual.Add("b");
                    actual.Add("c");

                    testable.Instance.GlobalVariables = null;

                    I.Expect(actual.Count).ToBe(0);
                }
            }
        }

        private class GlobalVariablesTestable : TestFixture<SettingsViewModel>
        {
            public GlobalVariablesTestable()
            {
                var settings = new JSLintNetSettings();
                settings.Options = new JSLintOptions();

                this.Model = settings;
            }

            public JSLintNetSettings Model { get; set; }

            protected override SettingsViewModel Resolve()
            {
                return new SettingsViewModel(this.Model);
            }
        }
    }
}
