namespace JSLintNet.VisualStudio.Specifications
{
    using System.IO;
    using System.Text;
    using EnvDTE;
    using EnvDTE80;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using JSLintNet.Settings;
    using JSLintNet.VisualStudio.Errors;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VSSDK.Tools.VsIdeTesting;

    /// <summary>
    /// Integration specs for the Visual Studio JSLint Provider.
    /// </summary>
    /// <remarks>
    /// These specs cannot be debugged normally. To debug, add a manual breakpoint after the environment and service provider objects have been resolved using:
    /// System.Diagnostics.Debugger.Break();
    /// </remarks>
    public class VisualStudioJSLintProviderIntegration
    {
        private const string VSHost = "VS IDE";

        private const string VSHive = VsIdeTestHostContants.TestPropertyName.RegistryHiveName;

        private const string VS11Exp = "11.0Exp";

        [TestClass]
        public class GetSettings : IntegrationBase
        {
            public TestContext TestContext { get; set; }

            [TestMethod]
            [HostType(VSHost)]
            [TestProperty(VSHive, VS11Exp)]
            public void Should_return_settings_from_project_file_if_valid_JSON()
            {
                using (var testable = new GetSettingsTestable())
                {
                    testable.WriteSettings(@"{ ""output"": ""Warning"" }");

                    try
                    {
                        var actual = testable.Instance.LoadSettings(testable.Project);

                        I.Expect(actual).Not.ToBeNull();
                        I.Expect(actual.Output).ToBe(Output.Warning);
                    }
                    finally
                    {
                        testable.DeleteSettings();
                    }
                }
            }

            [TestMethod]
            [HostType(VSHost)]
            [TestProperty(VSHive, VS11Exp)]
            public void Should_return_default_instance_from_project_file_if_invalid_JSON()
            {
                using (var testable = new GetSettingsTestable())
                {
                    testable.WriteSettings(@"<NOT>JSON</NOT>");

                    try
                    {
                        var actual = testable.Instance.LoadSettings(testable.Project);

                        I.Expect(actual).Not.ToBeNull();
                        I.Expect(actual.RunOnBuild).ToBeNullOrFalse();
                        I.Expect(actual.RunOnSave).ToBeNullOrFalse();
                        I.Expect(actual.CancelBuild).ToBeNullOrFalse();
                        I.Expect(actual.Options).ToBeNull();
                    }
                    finally
                    {
                        testable.DeleteSettings();
                    }
                }
            }

            [TestMethod]
            [HostType(VSHost)]
            [TestProperty(VSHive, VS11Exp)]
            public void Should_return_default_instance_if_file_does_not_exist()
            {
                using (var testable = new GetSettingsTestable())
                {
                    testable.DeleteSettings();

                    var actual = testable.Instance.LoadSettings(testable.Project);

                    I.Expect(actual).Not.ToBeNull();
                    I.Expect(actual.RunOnBuild).ToBeNullOrFalse();
                    I.Expect(actual.RunOnSave).ToBeNullOrFalse();
                    I.Expect(actual.CancelBuild).ToBeNullOrFalse();
                    I.Expect(actual.Options).ToBeNull();
                }
            }

            private class GetSettingsTestable : VisualStudioJSLintProviderTestableBase
            {
                public void WriteSettings(string settings)
                {
                    File.WriteAllText(this.SettingsPath, settings, Encoding.UTF8);
                }

                public void DeleteSettings()
                {
                    if (File.Exists(this.SettingsPath))
                    {
                        File.Delete(this.SettingsPath);
                    }
                }
            }
        }

        private abstract class VisualStudioJSLintProviderTestableBase : TestFixtureBase<VisualStudioJSLintProvider>
        {
            private const string ArtifactSolution = @"..\..\..\specifications\artifacts\Artifacts.sln";

            private const string AspNetProject = @"Artifacts.AspNet\Artifacts.AspNet.csproj";

            public VisualStudioJSLintProviderTestableBase()
            {
                this.Environment = VsIdeTestHostContext.Dte as DTE2;

                var solutionPath = Path.Combine(System.Environment.CurrentDirectory, ArtifactSolution);
                this.Environment.Solution.Open(solutionPath);

                var project = this.Project = this.Environment.Locate().ProjectByUniqueName(AspNetProject);
                this.SettingsPath = Path.Combine(Path.GetDirectoryName(project.FullName), JSLintNetSettings.FileName);
            }

            public DTE2 Environment { get; set; }

            public Project Project { get; set; }

            public string SettingsPath { get; set; }

            protected override VisualStudioJSLintProvider Resolve()
            {
                var serviceProvider = VsIdeTestHostContext.ServiceProvider;

                return new VisualStudioJSLintProvider(serviceProvider, new JSLintErrorListProvider(serviceProvider));
            }
        }
    }
}
