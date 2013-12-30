namespace JSLintNet.Specifications.Json
{
    using JSLintNet.Json;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using Xunit;

    public class JsonProviderIntegration
    {
        public abstract class JsonProviderIntegrationBase : IntegrationBase
        {
            public JsonProviderIntegrationBase()
            {
                this.Instance = new JsonProvider();
            }

            internal JsonProvider Instance { get; private set; }
        }

        public class SerializeSettings : JsonProviderIntegrationBase
        {
            [Fact(DisplayName = "Should correctly serialize output enum")]
            public void Spec01()
            {
                var settings = new JSLintNetSettings()
                {
                    Output = Output.Warning
                };

                var actual = this.Instance.SerializeSettings(settings);

                I.Expect(actual).ToContain(@"""output"": ""Warning""");
            }

            [Fact(DisplayName = "Should correctly serialize booleans")]
            public void Spec02()
            {
                var settings = new JSLintNetSettings()
                {
                    RunOnBuild = true
                };

                var actual = this.Instance.SerializeSettings(settings);

                I.Expect(actual).ToContain(@"""runOnSave"": false");
                I.Expect(actual).ToContain(@"""runOnBuild"": true");
                I.Expect(actual).ToContain(@"""cancelBuild"": false");
            }

            [Fact(DisplayName = "Should correctly serialize ignore list")]
            public void Spec03()
            {
                var settings = new JSLintNetSettings();

                settings.Ignore.Add(@"\mypath\");
                settings.Ignore.Add(@"\ignore\libraries\*.js");

                var actual = this.Instance.SerializeSettings(settings);

                I.Expect(actual).ToMatch(@"""ignore"": \[\s*""\\\\mypath\\\\"",\s*""\\\\ignore\\\\libraries\\\\\*\.js""\s*\]");
            }

            [Fact(DisplayName = "Should correctly serialize options")]
            public void Spec04()
            {
                var settings = new JSLintNetSettings()
                {
                    Options = new JSLintOptions()
                    {
                        AssumeBrowser = true,
                        TolerateUnusedParameters = true
                    }
                };
                settings.Options.PredefinedGlobals.Add("MyGlobal", false);

                var actual = this.Instance.SerializeSettings(settings);

                I.Expect(actual).ToMatch(@"""options"": \{\s*""predef"": \{\s*""MyGlobal"": false\s*\},\s*""browser"": true,\s*""unparam"": true\s*\}");
            }
        }

        public class DeserializeSettings : JsonProviderIntegrationBase
        {
            [Fact(DisplayName = "Should correctly deserialize output enum")]
            public void Spec01()
            {
                var value = @"{""output"":""message""}";

                var actual = this.Instance.DeserializeSettings(value);

                I.Expect(actual.Output).ToBe(Output.Message);
            }

            [Fact(DisplayName = "Should correctly deserialize booleans")]
            public void Spec02()
            {
                var value = @"{""runOnBuild"":true}";

                var actual = this.Instance.DeserializeSettings(value);

                I.Expect(actual.RunOnSave).ToBeFalse();
                I.Expect(actual.RunOnBuild).ToBeTrue();
                I.Expect(actual.CancelBuild).ToBeFalse();
            }

            [Fact(DisplayName = "Should correctly deserialize ignore list")]
            public void Spec03()
            {
                var value = @"{""ignore"":[""\\mypath\\"",""\\ignore\\libraries\\*.js""]}";

                var actual = this.Instance.DeserializeSettings(value);

                I.Expect(actual.Ignore).ToContain(@"\mypath\");
                I.Expect(actual.Ignore).ToContain(@"\ignore\libraries\*.js");
                I.Expect(actual.Ignore.Count).ToBe(2);
            }

            [Fact(DisplayName = "Should correctly deserialize options")]
            public void Spec04()
            {
                var value = @"{""options"":{""predef"":{""MyGlobal"":true},""browser"":true,""unparam"":true}}";

                var actual = this.Instance.DeserializeSettings(value);
                var options = actual.Options;

                I.Expect(options.PredefinedGlobals["MyGlobal"]).ToBeTrue();
                I.Expect(options.AssumeBrowser.Value).ToBeTrue();
                I.Expect(options.TolerateUnusedParameters.Value).ToBeTrue();
            }
        }
    }
}
