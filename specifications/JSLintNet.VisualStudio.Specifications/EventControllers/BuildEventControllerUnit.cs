namespace JSLintNet.VisualStudio.Specifications.EventControllers
{
    using System.Collections.Generic;
    using System.IO;
    using EnvDTE;
    using IExpect;
    using JSLintNet.QualityTools;
    using JSLintNet.Settings;
    using JSLintNet.VisualStudio.Errors;
    using JSLintNet.VisualStudio.EventControllers;
    using JSLintNet.VisualStudio.Properties;
    using JSLintNet.VisualStudio.Specifications.Fakes;
    using Moq;
    using Xunit;

    public class BuildEventControllerUnit
    {
        public class OnBuildProjectConfigBegin : UnitBase
        {
            [Fact(DisplayName = "Should not run for clean build")]
            public void Spec01()
            {
                using (var testable = new OnBuildProjectConfigBeginTestable())
                {
                    testable.Instance.Initialize();
                    testable.BuildEventsMock.Raise(x => x.OnBuildBegin += null, default(vsBuildScope), vsBuildAction.vsBuildActionClean);
                    testable.BuildEventsMock.Raise(x => x.OnBuildProjConfigBegin += null, "Some Name", string.Empty, string.Empty, string.Empty);

                    testable.EnvironmentMock.VerifyGet(x => x.Solution, Times.Never());
                }
            }

            [Fact(DisplayName = "Should not run when project is not found")]
            public void Spec02()
            {
                using (var testable = new OnBuildProjectConfigBeginTestable())
                {
                    testable.Instance.Initialize();
                    testable.BuildEventsMock.Raise(x => x.OnBuildProjConfigBegin += null, "Not Found", string.Empty, string.Empty, string.Empty);

                    testable.EnvironmentMock.VerifyGet(x => x.Solution);
                    testable.Verify<IVisualStudioJSLintProvider>(x => x.LoadSettings(It.IsAny<Project>()), Times.Never());
                }
            }

            [Fact(DisplayName = "Should not run when run on build is false")]
            public void Spec03()
            {
                using (var testable = new OnBuildProjectConfigBeginTestable())
                {
                    testable.Settings.RunOnBuild = false;

                    testable.Instance.Initialize();
                    testable.BuildEventsMock.Raise(x => x.OnBuildProjConfigBegin += null, testable.ProjectMock.Object.UniqueName, string.Empty, string.Empty, string.Empty);

                    testable.Verify<IVisualStudioJSLintProvider>(x => x.LoadSettings(testable.ProjectMock.Object));
                    testable.ProjectMock.VerifyGet(x => x.ProjectItems, Times.Never());
                }
            }

            [Fact(DisplayName = "Should find and process all lintables when run on build is true")]
            public void Spec04()
            {
                using (var testable = new OnBuildProjectConfigBeginTestable())
                {
                    testable.Settings.RunOnBuild = true;

                    testable.Instance.Initialize();
                    testable.BuildEventsMock.Raise(x => x.OnBuildProjConfigBegin += null, testable.ProjectMock.Object.UniqueName, string.Empty, string.Empty, string.Empty);

                    I.Expect(testable.LastItemsLinted).Not.ToBeNull();
                    I.Expect(testable.LastItemsLinted.Count).ToBe(2);
                    testable.EnvironmentMock.Verify(x => x.ExecuteCommand("Build.Cancel", string.Empty), Times.Never());
                }
            }

            [Fact(DisplayName = "Should cancel build when errors found and cancel build is true")]
            public void Spec05()
            {
                using (var testable = new OnBuildProjectConfigBeginTestable())
                {
                    testable.Settings.RunOnBuild = true;
                    testable.Settings.CancelBuild = true;
                    testable.ErrorsFound = 222;

                    testable.Instance.Initialize();
                    testable.BuildEventsMock.Raise(x => x.OnBuildProjConfigBegin += null, testable.ProjectMock.Object.UniqueName, string.Empty, string.Empty, string.Empty);

                    testable.EnvironmentMock.Verify(x => x.ExecuteCommand("Build.Cancel", string.Empty));
                    testable.Verify<IJSLintErrorListProvider>(x => x.AddCustomError(Resources.BuildCanceled));
                }
            }

            private class OnBuildProjectConfigBeginTestable : BuildEventControllerTestableBase
            {
                public OnBuildProjectConfigBeginTestable()
                {
                    this.ProjectFullName = @"Z:\\project path\project.csproj";

                    this.SolutionMock = new Mock<Solution>();
                    this.ProjectsFake = new ProjectsFake();
                    this.ProjectMock = new Mock<Project>();
                    this.ProjectPropertiesFake = new PropertiesFake();
                    this.ProjectItemsFake = new ProjectItemsFake();
                    this.Settings = new JSLintNetSettings();
                }

                public string ProjectFullName { get; set; }

                public Mock<Solution> SolutionMock { get; set; }

                public ProjectsFake ProjectsFake { get; set; }

                public Mock<Project> ProjectMock { get; set; }

                public PropertiesFake ProjectPropertiesFake { get; set; }

                public ProjectItemsFake ProjectItemsFake { get; set; }

                public IList<ProjectItem> LastItemsLinted { get; set; }

                public int ErrorsFound { get; set; }

                internal JSLintNetSettings Settings { get; set; }

                protected override void BeforeResolve()
                {
                    base.BeforeResolve();

                    this.EnvironmentMock
                        .SetupGet(x => x.Solution)
                        .Returns(this.SolutionMock.Object);

                    this.SolutionMock
                        .SetupGet(x => x.Projects)
                        .Returns(this.ProjectsFake);

                    this.ProjectsFake.AddKeyedItem(this.ProjectMock.Object);

                    this.ProjectMock
                        .SetupGet(x => x.FullName)
                        .Returns(() => this.ProjectFullName);

                    this.ProjectMock
                        .SetupGet(x => x.UniqueName)
                        .Returns("Unique Name");

                    this.ProjectMock
                        .SetupGet(x => x.ProjectItems)
                        .Returns(this.ProjectItemsFake);

                    this.ProjectMock
                        .SetupGet(x => x.Properties)
                        .Returns(this.ProjectPropertiesFake);

                    this.ProjectPropertiesFake.AddProperty("FullPath", Path.GetDirectoryName(this.ProjectFullName));

                    this.ProjectItemsFake.ContainingProject = this.ProjectMock.Object;
                    this.ProjectItemsFake.AddProjectItem(@"project path\test1.js");
                    this.ProjectItemsFake.AddProjectItem(@"project path\test2.txt");
                    this.ProjectItemsFake.AddProjectItem(@"project path\test3.ts");
                    this.ProjectItemsFake.AddProjectItem(@"project path\test4.json");

                    this.GetMock<IVisualStudioJSLintProvider>()
                        .Setup(x => x.LintProjectItems(It.IsAny<IList<ProjectItem>>(), this.Settings))
                        .Callback((IList<ProjectItem> x, JSLintNetSettings y) => this.LastItemsLinted = x)
                        .Returns(() => this.ErrorsFound);

                    this.GetMock<IVisualStudioJSLintProvider>()
                        .Setup(x => x.LoadSettings(this.ProjectMock.Object))
                        .Returns(this.Settings);
                }
            }
        }

        private abstract class BuildEventControllerTestableBase : EventControllerTestableBase<BuildEventController>
        {
            public BuildEventControllerTestableBase()
            {
                this.BuildEventsMock = new Mock<BuildEvents>();
            }

            public Mock<BuildEvents> BuildEventsMock { get; set; }

            protected override void BeforeResolve()
            {
                base.BeforeResolve();

                this.EventsMock
                    .SetupGet(x => x.BuildEvents)
                    .Returns(this.BuildEventsMock.Object);
            }
        }
    }
}
