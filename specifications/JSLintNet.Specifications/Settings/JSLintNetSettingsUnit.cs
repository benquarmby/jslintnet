namespace JSLintNet.Specifications.Settings
{
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using JSLintNet.Settings;
    using Xunit;

    public class JSLintNetSettingsUnit
    {
        public class NormalizeIgnore : UnitBase
        {
            [Fact(DisplayName = "Should replace forward slashes with back slashes")]
            public void Spec01()
            {
                var instance = new JSLintNetSettings();

                var ignore = instance.Ignore;
                ignore.Add(@"some/path");
                ignore.Add(@"/some/other/path");
                ignore.Add(@"other/path");

                var actual = instance.NormalizeIgnore();

                I.Expect(actual[0]).ToContain(@"some\path");
                I.Expect(actual[1]).ToContain(@"\some\other\path");
                I.Expect(actual[2]).ToContain(@"other\path");
            }

            [Fact(DisplayName = "Should add directory separator prefix where absent")]
            public void Spec02()
            {
                var instance = new JSLintNetSettings();

                var ignore = instance.Ignore;
                ignore.Add(@"some\path");
                ignore.Add(@"\some\other\path");
                ignore.Add(@"other\path");

                var actual = instance.NormalizeIgnore();

                I.Expect(actual[0]).ToStartWith(@"\");
                I.Expect(actual[2]).ToStartWith(@"\");
            }

            [Fact(DisplayName = "Should not add directory separator suffix to js file")]
            public void Spec03()
            {
                var instance = new JSLintNetSettings();

                var ignore = instance.Ignore;
                ignore.Add(@"path\to\file.js");

                var actual = instance.NormalizeIgnore();

                I.Expect(actual[0]).ToStartWith(@"\path\to\file.js");
            }

            [Fact(DisplayName = "Should not add directory separator suffix to json file")]
            public void Spec04()
            {
                var instance = new JSLintNetSettings();

                var ignore = instance.Ignore;
                ignore.Add(@"root\data.json");

                var actual = instance.NormalizeIgnore();

                I.Expect(actual[0]).ToBe(@"\root\data.json");
            }

            [Fact(DisplayName = "Should add directory separator suffix where absent")]
            public void Spec05()
            {
                var instance = new JSLintNetSettings();

                var ignore = instance.Ignore;
                ignore.Add(@"path\to");
                ignore.Add(@"another\path\");
                ignore.Add(@"looks/like/a.file");

                var actual = instance.NormalizeIgnore();

                I.Expect(actual[0]).ToEndWith(@"\");
                I.Expect(actual[2]).ToEndWith(@"\");
            }
        }

        public class Merge : UnitBase
        {
            [Fact(DisplayName = "Should override the output setting")]
            public void Spec01()
            {
                var target = new JSLintNetSettings()
                {
                    Output = Output.Error
                };

                var merge = new JSLintNetSettings()
                {
                    Output = Output.Message
                };

                target.Merge(merge);

                I.Expect(target.Output).Not.ToBe(Output.Error);
                I.Expect(target.Output).ToBe(Output.Message);
            }

            [Fact(DisplayName = "Should add to ignore list")]
            public void Spec02()
            {
                var target = new JSLintNetSettings();
                target.Ignore.Add("target");

                var merge = new JSLintNetSettings();
                merge.Ignore.Add("merge");

                target.Merge(merge);

                I.Expect(target.Ignore).ToContain("target");
                I.Expect(target.Ignore).ToContain("merge");
            }

            [Fact(DisplayName = "Should not add duplicate to ignore list")]
            public void Spec03()
            {
                var target = new JSLintNetSettings();
                target.Ignore.Add("unique");

                var merge = new JSLintNetSettings();
                merge.Ignore.Add("unique");

                target.Merge(merge);

                I.Expect(target.Ignore).ToContain("unique");
                I.Expect(target.Ignore.Count).ToBe(1);
            }

            [Fact(DisplayName = "Should merge options")]
            public void Spec04()
            {
                var target = new JSLintNetSettings();
                target.Options = new JSLintOptions()
                {
                    MaximumErrors = 2,
                    AssumeBrowser = true
                };

                var merge = new JSLintNetSettings();
                merge.Options = new JSLintOptions()
                {
                    MaximumErrors = 4,
                    AssumeES6 = true
                };

                target.Merge(merge);

                I.Expect(target.Options.MaximumErrors).ToBe(4);
                I.Expect(target.Options.AssumeBrowser).ToBeTrue();
                I.Expect(target.Options.AssumeES6).ToBeTrue();
                I.Expect(target.Options.TolerateEval).ToBeNull();
            }

            [Fact(DisplayName = "Should replace null target options")]
            public void Spec05()
            {
                var target = new JSLintNetSettings();
                target.Options = null;

                var merge = new JSLintNetSettings();
                merge.Options = new JSLintOptions();

                target.Merge(merge);

                I.Expect(target.Options).Not.ToBeNull();
                I.Expect(target.Options).ToBe(merge.Options);
            }

            [Fact(DisplayName = "Should not replace null merge options")]
            public void Spec06()
            {
                var target = new JSLintNetSettings();
                target.Options = new JSLintOptions();

                var merge = new JSLintNetSettings();
                merge.Options = null;

                target.Merge(merge);

                I.Expect(target.Options).Not.ToBeNull();
            }

            [Fact(DisplayName = "Should merge settings")]
            public void Spec07()
            {
                var target = new JSLintNetSettings();
                target.ErrorLimit = 50;
                target.RunOnBuild = false;
                target.CancelBuild = true;

                var merge = new JSLintNetSettings();
                merge.ErrorLimit = 100;
                merge.RunOnBuild = true;
                merge.RunOnSave = false;

                target.Merge(merge);

                I.Expect(target.ErrorLimit).ToBe(100);
                I.Expect(target.RunOnBuild).ToBeTrue();
                I.Expect(target.CancelBuild).ToBeTrue();
                I.Expect(target.RunOnSave).ToBeFalse();
            }
        }
    }
}
