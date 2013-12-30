namespace JSLintNet.VisualStudio.Specifications.EventControllers
{
    using System.IO;
    using EnvDTE;
    using JSLintNet.QualityTools;
    using JSLintNet.VisualStudio.EventControllers;
    using JSLintNet.VisualStudio.Specifications.Fakes;
    using Moq;
    using Xunit;

    public class DocumentEventControllerUnit
    {
        public class OnDocumentSaved : UnitBase
        {
            [Fact(DisplayName = "Should not run unlintable document")]
            public void Spec01()
            {
                using (var testable = new OnDocumentSavedTestable())
                {
                    testable.DocumentFullName = @"project path\file.txt";

                    testable.Instance.Initialize();
                    testable.DocumentEventsMock.Raise(x => x.DocumentSaved += null, testable.DocumentMock.Object);

                    testable.Verify<IVisualStudioJSLintProvider>(x => x.LintDocument(testable.DocumentMock.Object, testable.Settings), Times.Never());
                }
            }

            [Fact(DisplayName = "Should not run if run on save is false")]
            public void Spec02()
            {
                using (var testable = new OnDocumentSavedTestable())
                {
                    testable.DocumentFullName = @"project path\file.js";
                    testable.Settings.RunOnSave = false;

                    testable.Instance.Initialize();
                    testable.DocumentEventsMock.Raise(x => x.DocumentSaved += null, testable.DocumentMock.Object);

                    testable.Verify<IVisualStudioJSLintProvider>(x => x.LintDocument(testable.DocumentMock.Object, testable.Settings), Times.Never());
                }
            }

            [Fact(DisplayName = "Should not run if path is ignored")]
            public void Spec03()
            {
                using (var testable = new OnDocumentSavedTestable())
                {
                    testable.DocumentFullName = @"project path\sub path\file.js";
                    testable.Settings.RunOnSave = true;
                    testable.Settings.Ignore.Add(@"\sub path\");

                    testable.Instance.Initialize();
                    testable.DocumentEventsMock.Raise(x => x.DocumentSaved += null, testable.DocumentMock.Object);

                    testable.Verify<IVisualStudioJSLintProvider>(x => x.LintDocument(testable.DocumentMock.Object, testable.Settings), Times.Never());
                }
            }

            [Fact(DisplayName = "Should process lintable file when run on save is true")]
            public void Spec04()
            {
                using (var testable = new OnDocumentSavedTestable())
                {
                    testable.DocumentFullName = @"project path\sub path\file.js";
                    testable.Settings.RunOnSave = true;

                    testable.Instance.Initialize();
                    testable.DocumentEventsMock.Raise(x => x.DocumentSaved += null, testable.DocumentMock.Object);

                    testable.Verify<IVisualStudioJSLintProvider>(x => x.LintDocument(testable.DocumentMock.Object, testable.Settings));
                }
            }

            private class OnDocumentSavedTestable : DocumentEventControllerTestableBase
            {
                public OnDocumentSavedTestable()
                {
                    this.Settings = new JSLintNetSettings();
                    this.ProjectMock = new Mock<Project>();
                    this.ProjectItemMock = new Mock<ProjectItem>();
                    this.ProjectPropertiesFake = new PropertiesFake();

                    this.BeforeInit += this.OnBeforeInit;
                }

                public Mock<Project> ProjectMock { get; set; }

                public Mock<ProjectItem> ProjectItemMock { get; set; }

                public JSLintNetSettings Settings { get; set; }

                public PropertiesFake ProjectPropertiesFake { get; set; }

                private void OnBeforeInit(object sender, System.EventArgs e)
                {
                    this.GetMock<IVisualStudioJSLintProvider>()
                        .Setup(x => x.LoadSettings(It.IsAny<Project>()))
                        .Returns(this.Settings);

                    this.ProjectMock
                        .SetupGet(x => x.FullName)
                        .Returns(@"project path\project.csproj");

                    this.ProjectPropertiesFake.AddProperty("FullPath", @"project path\");

                    this.ProjectMock
                        .SetupGet(x => x.Properties)
                        .Returns(this.ProjectPropertiesFake);

                    this.ProjectItemMock
                        .SetupGet(x => x.ContainingProject)
                        .Returns(this.ProjectMock.Object);

                    this.ProjectItemMock
                        .Setup(x => x.get_FileNames(It.IsAny<short>()))
                        .Returns(() => this.DocumentFullName);

                    var properties = new PropertiesFake();
                    properties.AddProperty("IsLink", false);

                    this.ProjectItemMock
                        .SetupGet(x => x.Properties)
                        .Returns(properties);

                    this.DocumentMock
                        .SetupGet(x => x.ProjectItem)
                        .Returns(this.ProjectItemMock.Object);
                }
            }
        }

        private abstract class DocumentEventControllerTestableBase : EventControllerTestableBase<DocumentEventController>
        {
            public DocumentEventControllerTestableBase()
            {
                this.DocumentEventsMock = new Mock<DocumentEvents>();
                this.DocumentMock = new Mock<Document>();

                this.BeforeInit += this.OnBeforeInit;
            }

            public Mock<DocumentEvents> DocumentEventsMock { get; set; }

            public Mock<Document> DocumentMock { get; set; }

            public string DocumentFullName { get; set; }

            private void OnBeforeInit(object sender, System.EventArgs e)
            {
                this.EventsMock
                    .Setup(x => x.get_DocumentEvents(null))
                    .Returns(this.DocumentEventsMock.Object);

                this.DocumentMock
                    .SetupGet(x => x.Name)
                    .Returns(() => Path.GetFileName(this.DocumentFullName));

                this.DocumentMock
                    .SetupGet(x => x.FullName)
                    .Returns(() => this.DocumentFullName);
            }
        }
    }
}
