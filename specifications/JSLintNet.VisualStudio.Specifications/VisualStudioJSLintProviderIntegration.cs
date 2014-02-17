namespace JSLintNet.VisualStudio.Specifications
{
    using System;
    using System.IO;
    using System.Text;
    using EnvDTE;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using JSLintNet.Settings;
    using JSLintNet.VisualStudio.Errors;
    using JSLintNet.VisualStudio.Specifications.Helpers;
    using Microsoft.VisualStudio.Shell;
    using Xunit;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    /// <summary>
    /// Integration specs for the Visual Studio JSLint Provider.
    /// </summary>
    /// <remarks>
    /// These specs cannot be debugged normally. To debug, add a manual breakpoint after the environment and service provider objects have been resolved using:
    /// System.Diagnostics.Debugger.Break();
    /// </remarks>
    public class VisualStudioJSLintProviderIntegration
    {
        public class GetSettings : IntegrationBase
        {
            [Fact(DisplayName = "Should return settings from project file if valid json")]
            public void Spec01()
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

            [Fact(DisplayName = "Should return default instance from project file if invalid json")]
            public void Spec02()
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

            [Fact(DisplayName = "Should return default instance if file does not exist")]
            public void Spec03()
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

        private abstract class VisualStudioJSLintProviderTestableBase : IDisposable
        {
            public VisualStudioJSLintProviderTestableBase()
            {
                this.Environment = VisualStudioHelper.GetCurrentEnvironment();

                var serviceProvider = new ServiceProvider(this.Environment as IOleServiceProvider);
                var assemblyName = typeof(VisualStudioJSLintProviderIntegration).Assembly.GetName().Name;
                var projectFile = string.Format(@"Specifications\{0}\{0}.csproj", assemblyName);

                this.Project = this.Environment.Solution.Projects.FindByUniqueName(projectFile);
                this.SettingsPath = Path.Combine(Path.GetDirectoryName(this.Project.FullName), "JSLintNet.json");

                this.Instance = new VisualStudioJSLintProvider(serviceProvider, new JSLintErrorListProvider(serviceProvider));
            }

            public DTE Environment { get; set; }

            public Project Project { get; set; }

            public string SettingsPath { get; private set; }

            public VisualStudioJSLintProvider Instance { get; set; }

            public void Dispose()
            {
                this.Dispose(true);
            }

            protected void Dispose(bool disposing)
            {
            }
        }
    }
}
