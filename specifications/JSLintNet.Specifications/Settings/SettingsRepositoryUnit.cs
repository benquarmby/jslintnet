namespace JSLintNet.Specifications.Settings
{
    using System;
    using System.Text;
    using JSLintNet.Abstractions;
    using JSLintNet.Json;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using JSLintNet.Settings;
    using Moq;
    using Xunit;

    public class SettingsRepositoryUnit
    {
        public class Load : UnitBase
        {
            [Fact(DisplayName = "Should deserialize settings file if it exists")]
            public void Spec01()
            {
                using (var testable = new LoadTestable())
                {
                    testable.PrimaryExists = true;

                    testable.Instance.Load(JSLintNetSettings.FileName);

                    testable.Verify<IJsonProvider>(x => x.DeserializeSettings("PRIMARY SETTINGS"));
                }
            }

            [Fact(DisplayName = "Should merge settings with active configuration version if it exists")]
            public void Spec02()
            {
                using (var testable = new LoadTestable())
                {
                    testable.PrimaryExists = true;
                    testable.PrimarySettings.CancelBuild = false;

                    testable.ConfigurationExists = true;
                    testable.ConfigurationSettings.CancelBuild = true;

                    testable.Instance.Load(JSLintNetSettings.FileName, "Release");

                    I.Expect(testable.PrimarySettings.CancelBuild).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should never return a null settings instance")]
            public void Spec03()
            {
                using (var testable = new LoadTestable())
                {
                    testable.PrimaryExists = false;

                    var actual = testable.Instance.Load(JSLintNetSettings.FileName);

                    I.Expect(actual).Not.ToBeNull();
                }
            }

            [Fact(DisplayName = "Should attach primary file name to settings instance")]
            public void Spec04()
            {
                using (var testable = new LoadTestable())
                {
                    testable.PrimaryExists = true;

                    var actual = testable.Instance.Load(JSLintNetSettings.FileName);

                    I.Expect(actual.Files).ToContain(JSLintNetSettings.FileName);
                }
            }

            [Fact(DisplayName = "Should attach merged file name to settings instance")]
            public void Spec05()
            {
                using (var testable = new LoadTestable())
                {
                    testable.PrimaryExists = true;
                    testable.ConfigurationExists = true;

                    var actual = testable.Instance.Load(JSLintNetSettings.FileName, "Release");

                    I.Expect(actual.Files).ToContain("JSLintNet.Release.json");
                    I.Expect(actual.Files.Count).ToBe(2);
                }
            }

            [Fact(DisplayName = "Should always attach files to settings instance")]
            public void Spec06()
            {
                using (var testable = new LoadTestable())
                {
                    testable.PrimaryExists = false;
                    testable.ConfigurationExists = false;

                    var actual = testable.Instance.Load(JSLintNetSettings.FileName, "Release");

                    I.Expect(actual.Files).ToContain(JSLintNetSettings.FileName);
                    I.Expect(actual.Files).ToContain("JSLintNet.Release.json");
                    I.Expect(actual.Files.Count).ToBe(2);
                }
            }

            private class LoadTestable : SettingsRepositoryTestableBase
            {
                public LoadTestable()
                {
                    this.PrimarySettings = new JSLintNetSettings();
                    this.ConfigurationSettings = new JSLintNetSettings();

                    this.BeforeInit += this.OnBeforeInit;
                }

                public bool PrimaryExists { get; set; }

                public JSLintNetSettings PrimarySettings { get; set; }

                public bool ConfigurationExists { get; set; }

                public JSLintNetSettings ConfigurationSettings { get; set; }

                public string ConfigurationName { get; set; }

                private void OnBeforeInit(object sender, EventArgs e)
                {
                    this.GetMock<IFileSystemWrapper>()
                        .Setup(x => x.FileExists(It.Is<string>(y => y.EndsWith(JSLintNetSettings.FileName))))
                        .Returns(() => this.PrimaryExists);

                    this.GetMock<IFileSystemWrapper>()
                        .Setup(x => x.ReadAllText(It.Is<string>(y => y.EndsWith(JSLintNetSettings.FileName)), Encoding.UTF8))
                        .Returns("PRIMARY SETTINGS");

                    this.GetMock<IJsonProvider>()
                        .Setup(x => x.DeserializeSettings("PRIMARY SETTINGS"))
                        .Returns(this.PrimarySettings);

                    this.GetMock<IFileSystemWrapper>()
                        .Setup(x => x.FileExists(It.Is<string>(y => !y.EndsWith(JSLintNetSettings.FileName))))
                        .Returns(() => this.ConfigurationExists);

                    this.GetMock<IFileSystemWrapper>()
                        .Setup(x => x.ReadAllText(It.Is<string>(y => !y.EndsWith(JSLintNetSettings.FileName)), Encoding.UTF8))
                        .Returns("CONFIGURATION SETTINGS");

                    this.GetMock<IJsonProvider>()
                        .Setup(x => x.DeserializeSettings("CONFIGURATION SETTINGS"))
                        .Returns(this.ConfigurationSettings);
                }
            }
        }

        private class SettingsRepositoryTestableBase : TestableBase<SettingsRepository>
        {
        }
    }
}
