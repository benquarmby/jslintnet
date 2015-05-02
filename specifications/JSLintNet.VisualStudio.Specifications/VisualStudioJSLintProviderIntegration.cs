namespace JSLintNet.VisualStudio.Specifications
{
    using System;
    using EnvDTE80;
    using JSLintNet.QualityTools;
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
            /// <summary>
            /// Should return settings from project file if valid JSON.
            /// </summary>
            [TestMethod]
            [HostType(VSHost)]
            [TestProperty(VSHive, VS11Exp)]
            public void Should_return_settings_from_project_file_if_valid_JSON()
            {
                System.Diagnostics.Debugger.Break();

                using (var testable = new GetSettingsTestable())
                {
                    testable.Environment.Solution.Open("");

                    testable.WriteSettings(@"{ ""output"": ""Warning"" }");

                    try
                    {
                        ////var actual = testable.Instance.LoadSettings(testable.Project);

                        ////I.Expect(actual).Not.ToBeNull();
                        ////I.Expect(actual.Output).ToBe(Output.Warning);
                    }
                    finally
                    {
                        testable.DeleteSettings();
                    }
                }
            }

            [TestMethod]
            [HostType("VS IDE")]
            [TestProperty(VsIdeTestHostContants.TestPropertyName.RegistryHiveName, "11.0Exp")]
            public void Spec02()
            {
                using (var testable = new GetSettingsTestable())
                {
                    testable.WriteSettings(@"<NOT>JSON</NOT>");

                    ////try
                    ////{
                    ////    var actual = testable.Instance.LoadSettings(testable.Project);

                    ////    I.Expect(actual).Not.ToBeNull();
                    ////    I.Expect(actual.RunOnBuild).ToBeNullOrFalse();
                    ////    I.Expect(actual.RunOnSave).ToBeNullOrFalse();
                    ////    I.Expect(actual.CancelBuild).ToBeNullOrFalse();
                    ////    I.Expect(actual.Options).ToBeNull();
                    ////}
                    ////finally
                    ////{
                    ////    testable.DeleteSettings();
                    ////}
                }
            }

            [TestMethod]
            [HostType("VS IDE")]
            [TestProperty(VsIdeTestHostContants.TestPropertyName.RegistryHiveName, "11.0Exp")]
            public void Spec03()
            {
                using (var testable = new GetSettingsTestable())
                {
                    testable.DeleteSettings();

                    ////var actual = testable.Instance.LoadSettings(testable.Project);

                    ////I.Expect(actual).Not.ToBeNull();
                    ////I.Expect(actual.RunOnBuild).ToBeNullOrFalse();
                    ////I.Expect(actual.RunOnSave).ToBeNullOrFalse();
                    ////I.Expect(actual.CancelBuild).ToBeNullOrFalse();
                    ////I.Expect(actual.Options).ToBeNull();
                }
            }

            private class GetSettingsTestable : VisualStudioJSLintProviderTestableBase
            {
                public void WriteSettings(string settings)
                {
                    ////File.WriteAllText(this.SettingsPath, settings, Encoding.UTF8);
                }

                public void DeleteSettings()
                {
                    ////if (File.Exists(this.SettingsPath))
                    ////{
                    ////    File.Delete(this.SettingsPath);
                    ////}
                }
            }
        }

        private abstract class VisualStudioJSLintProviderTestableBase : IDisposable
        {
            public VisualStudioJSLintProviderTestableBase()
            {
                var serviceProvider = VsIdeTestHostContext.ServiceProvider;

                this.Environment = VsIdeTestHostContext.Dte as DTE2;
                this.Instance = new VisualStudioJSLintProvider(serviceProvider, new JSLintErrorListProvider(serviceProvider));
            }

            public VisualStudioJSLintProvider Instance { get; set; }

            public DTE2 Environment { get; set; }

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
