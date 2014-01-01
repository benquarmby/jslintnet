namespace JSLintNet.Console.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using JSLintNet.Abstractions;
    using JSLintNet.Json;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Fakes;
    using JSLintNet.UI.ViewModels;
    using JSLintNet.UI.Views;
    using Moq;
    using Xunit;
    using CoreResources = JSLintNet.Properties.Resources;

    public class ConsoleJSLintProviderUnit
    {
        public class Lint : UnitBase
        {
            [Fact(DisplayName = "Should stop processing when error limit reached")]
            public void Spec01()
            {
                using (var testable = new LintTestable())
                {
                    testable.SetupFile("some1.js", 2);
                    testable.SetupFile("some2.js", 2);

                    testable.Options.Settings.ErrorLimit = 10;
                    testable.ErrorCount = 11;

                    testable.Instance.Lint(testable.Options);

                    testable.ConsoleWriterMock.Verify(x => x.WriteErrorLine(CoreResources.ErrorLimitReachedFormat, 11));
                    testable.FileSystemWrapperMock.Verify(x => x.ReadAllText(testable.Options.SourceFiles[0], Encoding.UTF8));
                    testable.FileSystemWrapperMock.Verify(x => x.ReadAllText(testable.Options.SourceFiles[1], Encoding.UTF8), Times.Never());
                }
            }

            [Fact(DisplayName = "Should stop processing when file limit reached")]
            public void Spec02()
            {
                using (var testable = new LintTestable())
                {
                    testable.SetupFile("some1.js", 2);
                    testable.SetupFile("some2.js", 2);

                    testable.Options.Settings.FileLimit = 10;
                    testable.ProcessedFileCount = 11;

                    testable.Instance.Lint(testable.Options);

                    testable.ConsoleWriterMock.Verify(x => x.WriteErrorLine(CoreResources.FileLimitReachedFormat, 11));
                    testable.FileSystemWrapperMock.Verify(x => x.ReadAllText(testable.Options.SourceFiles[0], Encoding.UTF8));
                    testable.FileSystemWrapperMock.Verify(x => x.ReadAllText(testable.Options.SourceFiles[1], Encoding.UTF8), Times.Never());
                }
            }

            [Fact(DisplayName = "Should stop processing when exception limit reached")]
            public void Spec03()
            {
                using (var testable = new LintTestable())
                {
                    for (int i = 0; i <= JSLintNetSettings.ExceptionLimit; i++)
                    {
                        var filePath = Path.Combine(testable.Options.SourceDirectory, "file" + i + ".js");
                        testable.Options.SourceFiles.Add(filePath);
                    }

                    testable.JSLintContextMock
                        .Setup(x => x.Lint(It.IsAny<string>(), It.IsAny<JSLintOptions>()))
                        .Throws<Exception>();

                    testable.Instance.Lint(testable.Options);

                    testable.ConsoleWriterMock.Verify(x => x.WriteErrorLine(CoreResources.ExceptionLimitReachedFormat, JSLintNetSettings.ExceptionLimit));
                    testable.FileSystemWrapperMock.Verify(x => x.ReadAllText(testable.Options.SourceFiles[49], Encoding.UTF8));
                    testable.FileSystemWrapperMock.Verify(x => x.ReadAllText(testable.Options.SourceFiles[50], Encoding.UTF8), Times.Never());
                }
            }

            private class LintTestable : ConsoleJSLintProviderTestableBase
            {
                public LintTestable()
                {
                    this.JSLintContextMock = new Mock<IJSLintContext>();
                    this.JSLintReportBuilderMock = new Mock<IJSLintReportBuilder>();

                    this.Options = new ConsoleOptions()
                    {
                        SourceDirectory = "Source Directory",
                        Settings = new JSLintNetSettings(),
                        SourceFiles = new List<string>()
                    };

                    this.BeforeInit += this.OnBeforeInit;
                }

                public Mock<IJSLintContext> JSLintContextMock { get; set; }

                public Mock<IJSLintReportBuilder> JSLintReportBuilderMock { get; set; }

                public ConsoleOptions Options { get; set; }

                public int ErrorCount { get; set; }

                public int ProcessedFileCount { get; set; }

                public JSLintDataFake SetupFile(string fileName, int errorCount)
                {
                    var filePath = Path.Combine(this.Options.SourceDirectory, fileName);
                    var fake = new JSLintDataFake(fileName, errorCount);
                    var source = fileName + " contents";

                    this.FileSystemWrapperMock
                        .Setup(x => x.ReadAllText(filePath, Encoding.UTF8))
                        .Returns(source);

                    this.JSLintContextMock
                        .Setup(x => x.Lint(source, null))
                        .Returns(fake);

                    this.Options.SourceFiles.Add(filePath);

                    return fake;
                }

                private void OnBeforeInit(object sender, EventArgs e)
                {
                    this.JSLintFactoryMock
                        .Setup(x => x.CreateContext())
                        .Returns(this.JSLintContextMock.Object);

                    this.JSLintFactoryMock
                        .Setup(x => x.CreateReportBuilder())
                        .Returns(this.JSLintReportBuilderMock.Object);

                    this.JSLintReportBuilderMock
                        .SetupGet(x => x.ErrorCount)
                        .Returns(() => this.ErrorCount);

                    this.JSLintReportBuilderMock
                        .SetupGet(x => x.ProcessedFileCount)
                        .Returns(() => this.ProcessedFileCount);
                }
            }
        }

        public class EditSettings : UnitBase
        {
            [Fact(DisplayName = "Should always show editor window")]
            public void Spec01()
            {
                using (var testable = new EditSettingsTestable())
                {
                    testable.Instance.EditSettings(testable.Options);

                    testable.ViewMock.Verify(x => x.ShowDialog());
                }
            }

            [Fact(DisplayName = "Should save results when dialog is OK")]
            public void Spec02()
            {
                using (var testable = new EditSettingsTestable())
                {
                    testable.ViewMock
                        .Setup(x => x.ShowDialog())
                        .Returns(true);

                    testable.Instance.EditSettings(testable.Options);

                    testable.FileSystemWrapperMock.Verify(x => x.WriteAllText(testable.Options.SettingsFile, It.IsAny<string>(), It.IsAny<Encoding>()));
                }
            }

            [Fact(DisplayName = "Should not save results when dialog is canceled")]
            public void Spec03()
            {
                using (var testable = new EditSettingsTestable())
                {
                    testable.ViewMock
                        .Setup(x => x.ShowDialog())
                        .Returns(false);

                    testable.Instance.EditSettings(testable.Options);

                    testable.FileSystemWrapperMock.Verify(x => x.WriteAllText(testable.Options.SettingsFile, It.IsAny<string>(), It.IsAny<Encoding>()), Times.Never());
                }
            }

            private class EditSettingsTestable : ConsoleJSLintProviderTestableBase
            {
                public EditSettingsTestable()
                {
                    this.ViewMock = new Mock<IView>();

                    this.Options = new ConsoleOptions()
                    {
                        SettingsEditor = true,
                        SettingsFile = @"D:\path\to\file.json",
                        Settings = new JSLintNetSettings()
                    };

                    this.BeforeInit += this.OnBeforeInit;
                }

                public ConsoleOptions Options { get; set; }

                public Mock<IView> ViewMock { get; set; }

                private void OnBeforeInit(object sender, EventArgs e)
                {
                    this.GetMock<IViewFactory>()
                        .Setup(x => x.CreateSettings(It.IsAny<SettingsViewModel>()))
                        .Returns(this.ViewMock.Object);
                }
            }
        }

        private abstract class ConsoleJSLintProviderTestableBase : TestableBase<ConsoleJSLintProvider>
        {
            public ConsoleJSLintProviderTestableBase()
            {
                this.JSLintFactoryMock = this.GetMock<IJSLintFactory>();
                this.FileSystemWrapperMock = this.GetMock<IFileSystemWrapper>();
                this.JsonProviderMock = this.GetMock<IJsonProvider>();
                this.ConsoleWriterMock = this.GetMock<IConsoleWriter>();

                this.BeforeInit += this.OnBeforeInit;
            }

            public Mock<IJSLintFactory> JSLintFactoryMock { get; set; }

            public Mock<IFileSystemWrapper> FileSystemWrapperMock { get; set; }

            public Mock<IJsonProvider> JsonProviderMock { get; set; }

            public Mock<IConsoleWriter> ConsoleWriterMock { get; set; }

            private void OnBeforeInit(object sender, EventArgs e)
            {
                this.ConsoleWriterMock
                    .Setup(x => x.WriteLine())
                    .Returns(this.ConsoleWriterMock.Object);

                this.ConsoleWriterMock
                    .Setup(x => x.WriteLine(It.IsAny<string>(), It.IsAny<object[]>()))
                    .Returns(this.ConsoleWriterMock.Object);

                this.ConsoleWriterMock
                    .Setup(x => x.WriteErrorLine())
                    .Returns(this.ConsoleWriterMock.Object);

                this.ConsoleWriterMock
                    .Setup(x => x.WriteErrorLine(It.IsAny<string>(), It.IsAny<object[]>()))
                    .Returns(this.ConsoleWriterMock.Object);
            }
        }
    }
}
