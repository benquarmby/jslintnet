namespace JSLintNet.VisualStudio.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using EnvDTE;
    using JSLintNet.Abstractions;
    using JSLintNet.Properties;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using JSLintNet.QualityTools.Fakes;
    using JSLintNet.Settings;
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

                    testable.Instance.LoadSettings(testable.ProjectMock.Object);

                    testable.Verify<ISettingsRepository>(x => x.Load(settingsPath, testable.ConfigurationName));
                }
            }

            [Fact(DisplayName = "Should get settings from project root if it exists")]
            public void Spec02()
            {
                using (var testable = new LoadSettingsTestable())
                {
                    testable.Instance.LoadSettings(testable.ProjectMock.Object);

                    testable.Verify<ISettingsRepository>(x => x.Load(It.Is<string>(y => y.EndsWith(JSLintNetSettings.FileName)), testable.ConfigurationName));
                }
            }

            [Fact(DisplayName = "Should check if settings exist in cache")]
            public void Spec03()
            {
                using (var testable = new LoadSettingsTestable())
                {
                    testable.Instance.LoadSettings(testable.ProjectMock.Object);

                    testable.Verify<ICacheProvider>(x => x.Contains(It.IsAny<string>()));
                }
            }

            [Fact(DisplayName = "Should get from cache if it contains settings")]
            public void Spec04()
            {
                using (var testable = new LoadSettingsTestable())
                {
                    testable.CacheContains = true;
                    testable.Instance.LoadSettings(testable.ProjectMock.Object);

                    testable.Verify<ICacheProvider>(x => x.Get<JSLintNetSettings>(It.IsAny<string>()));
                }
            }

            [Fact(DisplayName = "Should set to cache if it does not contain settings")]
            public void Spec05()
            {
                using (var testable = new LoadSettingsTestable())
                {
                    testable.CacheContains = false;
                    testable.Instance.LoadSettings(testable.ProjectMock.Object);

                    testable.Verify<ICacheProvider>(x => x.Set(It.IsAny<string>(), testable.Settings, It.IsAny<int>(), It.IsAny<string[]>()));
                    testable.Verify<ICacheProvider>(x => x.Get<JSLintNetSettings>(It.IsAny<string>()), Times.Never());
                }
            }

            [Fact(DisplayName = "Should not throw when there is no project FullName or FullPath")]
            public void Spec06()
            {
                using (var testable = new LoadSettingsTestable())
                {
                    testable.Initialize();

                    testable.ProjectMock
                        .SetupGet(x => x.FullName)
                        .Returns((string)null);

                    testable.ProjectPropertiesFake.Items.Remove("FullPath");

                    I.Expect(() =>
                    {
                        testable.Instance.LoadSettings(testable.ProjectMock.Object);
                    }).Not.ToThrow();
                }
            }

            [Fact(DisplayName = "Should not pass configuration name to repository when merge is false")]
            public void Spec07()
            {
                using (var testable = new LoadSettingsTestable())
                {
                    testable.ConfigurationName = "Release";

                    testable.Instance.LoadSettings(testable.ProjectMock.Object, false);

                    testable.Verify<ISettingsRepository>(x => x.Load(It.IsAny<string>(), null));
                }
            }

            private class LoadSettingsTestable : VisualStudioJSLintProviderTestableBase
            {
                public LoadSettingsTestable()
                {
                    this.ProjectItemsFake = new ProjectItemsFake(this.ProjectMock.Object);
                    this.Settings = new JSLintNetSettings();
                }

                public ProjectItemsFake ProjectItemsFake { get; set; }

                public string ConfigurationName { get; set; }

                public JSLintNetSettings Settings { get; set; }

                public bool CacheContains { get; set; }

                protected override void BeforeResolve()
                {
                    base.BeforeResolve();

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

                    this.GetMock<ISettingsRepository>()
                        .Setup(x => x.Load(It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(this.Settings);

                    this.GetMock<ICacheProvider>()
                        .Setup(x => x.Contains(It.IsAny<string>()))
                        .Returns(() => this.CacheContains);

                    this.GetMock<ICacheProvider>()
                        .Setup(x => x.Get<JSLintNetSettings>(It.IsAny<string>()))
                        .Returns(this.Settings);
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
                    testable.JSLintContextMock.Verify(x => x.Lint("file5.js contents", null, It.IsAny<IList<string>>()));
                    testable.JSLintContextMock.Verify(x => x.Lint("file6.js contents", null, It.IsAny<IList<string>>()), Times.Never());
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
                    testable.JSLintContextMock.Verify(x => x.Lint("file5.js contents", null, It.IsAny<IList<string>>()));
                    testable.JSLintContextMock.Verify(x => x.Lint("file6.js contents", null, It.IsAny<IList<string>>()), Times.Never());
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
                            .Setup(x => x.Lint(source, null, It.IsAny<IList<string>>()))
                            .Returns(new JSLintDataFake(fileName, errorCount));
                    }

                    this.ProjectItems.Add(itemMock.Object);

                    return itemMock;
                }

                protected override void BeforeResolve()
                {
                    base.BeforeResolve();

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

        private abstract class VisualStudioJSLintProviderTestableBase : TestFixture<VisualStudioJSLintProvider>
        {
            public VisualStudioJSLintProviderTestableBase()
            {
                this.ProjectFullPath = @"E:\\solution\project\";
                this.ProjectUniqueName = @"project.csproj";
                this.ProjectMock = new Mock<Project>();
                this.ProjectPropertiesFake = new PropertiesFake();
            }

            public string ProjectFullPath { get; set; }

            public string ProjectUniqueName { get; set; }

            public Mock<Project> ProjectMock { get; set; }

            public PropertiesFake ProjectPropertiesFake { get; set; }

            protected override void BeforeResolve()
            {
                base.BeforeResolve();

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
