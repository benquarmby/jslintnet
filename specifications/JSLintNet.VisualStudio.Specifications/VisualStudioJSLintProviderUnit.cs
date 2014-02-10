namespace JSLintNet.VisualStudio.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using EnvDTE;
    using JSLintNet.Abstractions;
    using JSLintNet.Json;
    using JSLintNet.Properties;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using JSLintNet.QualityTools.Fakes;
    using JSLintNet.VisualStudio.Errors;
    using JSLintNet.VisualStudio.Specifications.Fakes;
    using Microsoft.VisualStudio.Shell.Interop;
    using Moq;
    using Xunit;

    public class VisualStudioJSLintProviderUnit
    {
        public class LoadSettings : UnitBase
        {
            [Fact(DisplayName = "Should get linked settings file from project if it exists")]
            public void Spec01()
            {
                using (var testable = new LoadSettingsTestable())
                {
                    var settingsPath = Path.Combine(@"some\path", JSLintNetSettings.FileName);
                    testable.ProjectItemsFake.AddProjectItem(settingsPath, true);
                    testable.PrimaryExists = true;

                    testable.Instance.LoadSettings(testable.ProjectMock.Object);

                    testable.Verify<IFileSystemWrapper>(x => x.ReadAllText(settingsPath, Encoding.UTF8));
                }
            }

            [Fact(DisplayName = "Should get settings from project root if it exists")]
            public void Spec02()
            {
                using (var testable = new LoadSettingsTestable())
                {
                    testable.PrimaryExists = true;

                    testable.Instance.LoadSettings(testable.ProjectMock.Object);

                    testable.Verify<IFileSystemWrapper>(x => x.ReadAllText(It.Is<string>(y => y.EndsWith(JSLintNetSettings.FileName)), Encoding.UTF8));
                }
            }

            [Fact(DisplayName = "Should deserialize settings file if it exists")]
            public void Spec03()
            {
                using (var testable = new LoadSettingsTestable())
                {
                    testable.PrimaryExists = true;

                    testable.Instance.LoadSettings(testable.ProjectMock.Object);

                    testable.Verify<IJsonProvider>(x => x.DeserializeSettings("PRIMARY SETTINGS"));
                }
            }

            [Fact(DisplayName = "Should merge settings with active configuration version if it exists")]
            public void Spec04()
            {
                using (var testable = new LoadSettingsTestable())
                {
                    testable.PrimaryExists = true;
                    testable.PrimarySettings.CancelBuild = false;

                    testable.ConfigurationExists = true;
                    testable.ConfigurationName = "Release";
                    testable.ConfigurationSettings.CancelBuild = true;

                    testable.Instance.LoadSettings(testable.ProjectMock.Object);

                    I.Expect(testable.PrimarySettings.CancelBuild).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should merge settings with linked configuration version if it exists")]
            public void Spec05()
            {
                using (var testable = new LoadSettingsTestable())
                {
                    testable.PrimaryExists = true;
                    testable.PrimarySettings.ErrorLimit = 10;
                    testable.PrimarySettings.FileLimit = 10;

                    var settingsPath = Path.Combine(@"some\path", "JSLintNet.Debug.json");
                    testable.ProjectItemsFake.AddProjectItem(settingsPath, true);

                    testable.ConfigurationExists = true;
                    testable.ConfigurationName = "Debug";
                    testable.ConfigurationSettings.FileLimit = 100;

                    testable.Instance.LoadSettings(testable.ProjectMock.Object);

                    testable.Verify<IFileSystemWrapper>(x => x.ReadAllText(settingsPath, Encoding.UTF8));
                    I.Expect(testable.PrimarySettings.ErrorLimit).ToBe(10);
                    I.Expect(testable.PrimarySettings.FileLimit).ToBe(100);
                }
            }

            private class LoadSettingsTestable : VisualStudioJSLintProviderTestableBase
            {
                public LoadSettingsTestable()
                {
                    this.ProjectItemsFake = new ProjectItemsFake(this.ProjectMock.Object);
                    this.PrimarySettings = new JSLintNetSettings();
                    this.ConfigurationSettings = new JSLintNetSettings();

                    this.BeforeInit += this.OnBeforeInit;
                }

                public ProjectItemsFake ProjectItemsFake { get; set; }

                public string ConfigurationName { get; set; }

                public JSLintNetSettings PrimarySettings { get; set; }

                public JSLintNetSettings ConfigurationSettings { get; set; }

                public bool PrimaryExists { get; set; }

                public bool ConfigurationExists { get; set; }

                private void OnBeforeInit(object sender, EventArgs e)
                {
                    this.ProjectMock
                        .SetupGet(x => x.ProjectItems)
                        .Returns(this.ProjectItemsFake);

                    var activeMock = new Mock<Configuration>();
                    activeMock
                        .SetupGet(x => x.ConfigurationName)
                        .Returns(() => this.ConfigurationName);

                    var managerMock = new Mock<ConfigurationManager>();
                    managerMock
                        .SetupGet(x => x.ActiveConfiguration)
                        .Returns(activeMock.Object);

                    this.ProjectMock
                        .SetupGet(x => x.ConfigurationManager)
                        .Returns(managerMock.Object);

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
                        .Setup(x => x.FileExists(It.Is<string>(y => y.EndsWith("JSLintNet." + this.ConfigurationName + ".json"))))
                        .Returns(() => this.ConfigurationExists);

                    this.GetMock<IFileSystemWrapper>()
                        .Setup(x => x.ReadAllText(It.Is<string>(y => y.EndsWith("JSLintNet." + this.ConfigurationName + ".json")), Encoding.UTF8))
                        .Returns("CONFIGURATION SETTINGS");

                    this.GetMock<IJsonProvider>()
                        .Setup(x => x.DeserializeSettings("CONFIGURATION SETTINGS"))
                        .Returns(this.ConfigurationSettings);
                }
            }
        }

        public class LintProjectItems : UnitBase
        {
            [Fact(DisplayName = "Should stop processing when error limit reached")]
            public void Spec01()
            {
                using (var testable = new LintProjectItemsTestable())
                {
                    testable.Settings.ErrorLimit = 10;

                    for (int i = 1; i <= 10; i++)
                    {
                        testable.AddFile("file" + i + ".js", 2);
                    }

                    testable.Instance.LintProjectItems(testable.ProjectItems, testable.Settings);

                    testable.Verify<IJSLintErrorListProvider>(x => x.AddCustomError(Resources.ErrorLimitReachedFormat, 10));
                    testable.JSLintContextMock.Verify(x => x.Lint("file5.js contents", null));
                    testable.JSLintContextMock.Verify(x => x.Lint("file6.js contents", null), Times.Never());
                }
            }

            [Fact(DisplayName = "Should stop processing when file limit reached")]
            public void Spec02()
            {
                using (var testable = new LintProjectItemsTestable())
                {
                    testable.Settings.FileLimit = 5;

                    for (int i = 1; i <= 10; i++)
                    {
                        testable.AddFile("file" + i + ".js", 0);
                    }

                    testable.Instance.LintProjectItems(testable.ProjectItems, testable.Settings);

                    testable.Verify<IJSLintErrorListProvider>(x => x.AddCustomError(Resources.FileLimitReachedFormat, 5));
                    testable.JSLintContextMock.Verify(x => x.Lint("file5.js contents", null));
                    testable.JSLintContextMock.Verify(x => x.Lint("file6.js contents", null), Times.Never());
                }
            }

            [Fact(DisplayName = "Should stop processing when exception limit reached")]
            public void Spec03()
            {
                using (var testable = new LintProjectItemsTestable())
                {
                    testable.JSLintContextMock
                        .Setup(x => x.Lint(It.IsAny<string>(), It.IsAny<JSLintOptions>()))
                        .Throws<Exception>();

                    var document = new Mock<Document>();

                    document
                        .Setup(x => x.Object(It.IsAny<string>()))
                        .Throws<Exception>();

                    for (int i = 1; i <= 51; i++)
                    {
                        var mockItem = testable.AddFile("file" + i + ".js", 0, true);

                        mockItem
                            .SetupGet(x => x.Document)
                            .Returns(document.Object);
                    }

                    testable.Instance.LintProjectItems(testable.ProjectItems, testable.Settings);

                    testable.Verify<IJSLintErrorListProvider>(x => x.AddCustomError(Resources.ExceptionLimitReachedFormat, 50));

                    Mock.Get(testable.ProjectItems[49]).Verify(x => x.Document);
                    Mock.Get(testable.ProjectItems[50]).Verify(x => x.Document, Times.Never());
                }
            }

            private class LintProjectItemsTestable : VisualStudioJSLintProviderTestableBase
            {
                public LintProjectItemsTestable()
                {
                    this.JSLintContextMock = new Mock<IJSLintContext>();
                    this.Settings = new JSLintNetSettings();
                    this.ProjectItems = new List<ProjectItem>();

                    this.BeforeInit += this.OnBeforeInit;
                }

                public Mock<IJSLintContext> JSLintContextMock { get; set; }

                public JSLintNetSettings Settings { get; set; }

                public IList<ProjectItem> ProjectItems { get; set; }

                public Mock<ProjectItem> AddFile(string fileName, int errorCount = 0, bool noMocks = false)
                {
                    var itemMock = new Mock<ProjectItem>();

                    itemMock
                        .SetupGet(x => x.ContainingProject)
                        .Returns(this.ProjectMock.Object);

                    itemMock
                        .Setup(x => x.get_FileNames(0))
                        .Returns(fileName);

                    if (!noMocks)
                    {
                        var source = fileName + " contents";

                        this.GetMock<IFileSystemWrapper>()
                            .Setup(x => x.ReadAllText(fileName, Encoding.UTF8))
                            .Returns(source);

                        this.JSLintContextMock
                            .Setup(x => x.Lint(source, null))
                            .Returns(new JSLintDataFake(fileName, errorCount));
                    }

                    this.ProjectItems.Add(itemMock.Object);

                    return itemMock;
                }

                private void OnBeforeInit(object sender, EventArgs e)
                {
                    this.GetMock<IJSLintFactory>()
                        .Setup(x => x.CreateContext())
                        .Returns(this.JSLintContextMock.Object);

                    var solutionServiceMock = new Mock<IVsSolution>();
                    var statusBarMock = new Mock<IVsStatusbar>();

                    this.GetMock<IServiceProvider>()
                        .Setup(x => x.GetService(typeof(SVsSolution)))
                        .Returns(solutionServiceMock.Object);

                    this.GetMock<IServiceProvider>()
                        .Setup(x => x.GetService(typeof(SVsStatusbar)))
                        .Returns(statusBarMock.Object);
                }
            }
        }

        private abstract class VisualStudioJSLintProviderTestableBase : TestableBase<VisualStudioJSLintProvider>
        {
            public VisualStudioJSLintProviderTestableBase()
            {
                this.ProjectFullPath = @"E:\\solution\project\";
                this.ProjectUniqueName = @"project.csproj";
                this.ProjectMock = new Mock<Project>();
                this.ProjectPropertiesFake = new PropertiesFake();

                this.BeforeInit += this.OnBeforeInit;
            }

            public string ProjectFullPath { get; set; }

            public string ProjectUniqueName { get; set; }

            public Mock<Project> ProjectMock { get; set; }

            public PropertiesFake ProjectPropertiesFake { get; set; }

            private void OnBeforeInit(object sender, EventArgs e)
            {
                this.ProjectMock
                    .SetupGet(x => x.FullName)
                    .Returns(() => Path.Combine(this.ProjectFullPath, this.ProjectUniqueName));

                this.ProjectMock
                    .SetupGet(x => x.UniqueName)
                    .Returns(() => this.ProjectUniqueName);

                this.ProjectMock
                    .SetupGet(x => x.Properties)
                    .Returns(this.ProjectPropertiesFake);

                this.ProjectPropertiesFake.AddProperty("FullPath", () => this.ProjectFullPath);
            }
        }
    }
}
