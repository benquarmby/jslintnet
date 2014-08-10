namespace JSLintNet.Specifications.UI
{
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using JSLintNet.Settings;
    using JSLintNet.UI.ViewModels;
    using Xunit;

    public class SettingsViewModelUnit
    {
        public class PredefinedGlobalsGet
        {
            [Fact(DisplayName = "Should convert predefined globals in model to space separated string")]
            public void Spec01()
            {
                using (var testable = new PredefinedGlobalsTestable())
                {
                    var globals = testable.Model.Options.PredefinedGlobals;
                    globals.Add("a", false);
                    globals.Add("b", false);
                    globals.Add("c", false);
                    globals.Add("d", false);

                    var actual = testable.Instance.PredefinedGlobals;

                    I.Expect(actual).ToBe("a b c d");
                }
            }
        }

        public class PredefinedGlobalsSet
        {
            [Fact(DisplayName = "Should add new predefined globals to model")]
            public void Spec01()
            {
                using (var testable = new PredefinedGlobalsTestable())
                {
                    var actual = testable.Model.Options.PredefinedGlobals;
                    actual.Add("a", false);

                    testable.Instance.PredefinedGlobals = "a b";

                    I.Expect(actual.Count).ToBe(2);
                    I.Expect(actual.ContainsKey("a"));
                    I.Expect(actual.ContainsKey("b"));
                }
            }

            [Fact(DisplayName = "Should add predefined globals separated by whitespace, commas, semi-colons or quotes")]
            public void Spec02()
            {
                using (var testable = new PredefinedGlobalsTestable())
                {
                    var actual = testable.Model.Options.PredefinedGlobals;
                    testable.Instance.PredefinedGlobals = "a b\tc\nd,e;f'g\"h";

                    I.Expect(actual.Count).ToBe(8);
                    I.Expect(actual.ContainsKey("a"));
                    I.Expect(actual.ContainsKey("h"));
                }
            }

            [Fact(DisplayName = "Should remove missing predefined globals from model")]
            public void Spec03()
            {
                using (var testable = new PredefinedGlobalsTestable())
                {
                    var actual = testable.Model.Options.PredefinedGlobals;
                    actual.Add("a", false);
                    actual.Add("b", false);
                    actual.Add("c", false);
                    actual.Add("d", false);

                    testable.Instance.PredefinedGlobals = "a d";

                    I.Expect(actual.Count).ToBe(2);
                    I.Expect(actual.ContainsKey("b"));
                    I.Expect(actual.ContainsKey("c"));
                }
            }

            [Fact(DisplayName = "Should not change writable state of existing global")]
            public void Spec04()
            {
                using (var testable = new PredefinedGlobalsTestable())
                {
                    var actual = testable.Model.Options.PredefinedGlobals;
                    actual.Add("a", true);

                    testable.Instance.PredefinedGlobals = "a b";

                    I.Expect(actual["a"]).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should clear globals from model when set to null")]
            public void Spec05()
            {
                using (var testable = new PredefinedGlobalsTestable())
                {
                    var actual = testable.Model.Options.PredefinedGlobals;
                    actual.Add("a", false);
                    actual.Add("b", false);
                    actual.Add("c", false);

                    testable.Instance.PredefinedGlobals = null;

                    I.Expect(actual.Count).ToBe(0);
                }
            }
        }

        private class PredefinedGlobalsTestable : TestableBase<SettingsViewModel>
        {
            public PredefinedGlobalsTestable()
            {
                var settings = new JSLintNetSettings();
                settings.Options = new JSLintOptions();

                this.Model = settings;
            }

            public JSLintNetSettings Model { get; set; }

            protected override SettingsViewModel Construct()
            {
                return new SettingsViewModel(this.Model);
            }
        }
    }
}
