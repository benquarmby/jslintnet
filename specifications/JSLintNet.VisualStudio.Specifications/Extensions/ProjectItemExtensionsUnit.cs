namespace JSLintNet.VisualStudio.Specifications.Extensions
{
    using EnvDTE;
    using JSLintNet.QualityTools.Expectations;
    using Moq;
    using Xunit;

    public class ProjectItemExtensionsUnit
    {
        public class IsFolder
        {
            [Fact(DisplayName = "Should return true given lower project kind")]
            public void Spec01()
            {
                var projectItem = Mock.Of<ProjectItem>(x => x.Kind == "{6bb5f8ef-4483-11d3-8bcf-00c04f8ec28c}");

                var actual = projectItem.Is().Folder;

                I.Expect(actual).ToBeTrue();
            }

            [Fact(DisplayName = "Should return true given upper project kind")]
            public void Spec02()
            {
                var projectItem = Mock.Of<ProjectItem>(x => x.Kind == "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}");

                var actual = projectItem.Is().Folder;

                I.Expect(actual).ToBeTrue();
            }

            [Fact(DisplayName = "Should return false given other project kind")]
            public void Spec03()
            {
                var projectItem = Mock.Of<ProjectItem>(x => x.Kind == "{not-a-guid}");

                var actual = projectItem.Is().Folder;

                I.Expect(actual).ToBeFalse();
            }
        }

        public class IsInSolutionFolder
        {
            [Fact(DisplayName = "Should return true given lower project kind")]
            public void Spec01()
            {
                var project = Mock.Of<Project>(x => x.Kind == "{66a26720-8fb5-11d2-aa7e-00c04f688dde}");
                var projectItem = Mock.Of<ProjectItem>(x => x.ContainingProject == project);

                var actual = projectItem.Is().InSolutionFolder;

                I.Expect(actual).ToBeTrue();
            }

            [Fact(DisplayName = "Should return true given upper project kind")]
            public void Spec02()
            {
                var project = Mock.Of<Project>(x => x.Kind == "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}");
                var projectItem = Mock.Of<ProjectItem>(x => x.ContainingProject == project);

                var actual = projectItem.Is().InSolutionFolder;

                I.Expect(actual).ToBeTrue();
            }

            [Fact(DisplayName = "Should return false given other project kind")]
            public void Spec03()
            {
                var project = Mock.Of<Project>(x => x.Kind == "{not-a-guid}");
                var projectItem = Mock.Of<ProjectItem>(x => x.ContainingProject == project);

                var actual = projectItem.Is().InSolutionFolder;

                I.Expect(actual).ToBeFalse();
            }
        }
    }
}
