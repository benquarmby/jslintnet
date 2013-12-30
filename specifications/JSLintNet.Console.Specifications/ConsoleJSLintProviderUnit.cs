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

                    this.JSLintReportBuilderMock
                        .SetupGet(x => x.ErrorCount)
                        .Returns(() => this.ErrorCount);

                    this.JSLintReportBuilderMock
                        .SetupGet(x => x.ProcessedFileCount)
                        .Returns(() => this.ProcessedFileCount);
                }
            }
        }

        private abstract class ConsoleJSLintProviderTestableBase : TestableBase<ConsoleJSLintProvider>
        {
            public ConsoleJSLintProviderTestableBase()
            {
                this.JSLintFactoryMock = this.AutoMocker.Mock<IJSLintFactory>();
                this.FileSystemWrapperMock = this.AutoMocker.Mock<IFileSystemWrapper>();
                this.JsonProviderMock = this.AutoMocker.Mock<IJsonProvider>();
                this.ConsoleWriterMock = this.AutoMocker.Mock<IConsoleWriter>();
            }

            public Mock<IJSLintFactory> JSLintFactoryMock { get; set; }

            public Mock<IFileSystemWrapper> FileSystemWrapperMock { get; set; }

            public Mock<IJsonProvider> JsonProviderMock { get; set; }

            public Mock<IConsoleWriter> ConsoleWriterMock { get; set; }
        }
    }
}
