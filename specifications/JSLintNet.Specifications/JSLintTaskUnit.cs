namespace JSLintNet.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using JSLintNet;
    using JSLintNet.Abstractions;
    using JSLintNet.Models;
    using JSLintNet.Properties;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using JSLintNet.QualityTools.Fakes;
    using JSLintNet.Settings;
    using Microsoft.Build.Framework;
    using Moq;
    using Xunit;

    public class JSLintTaskUnit
    {
        public class Execute : UnitBase
        {
            [Fact(DisplayName = "Should return true when no files found")]
            public void Spec01()
            {
                using (var testable = new ExecuteTestable())
                {
                    var actual = testable.Instance.Execute();

                    I.Expect(actual).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should return true when no files contain errors")]
            public void Spec02()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file.js", 0);

                    var actual = testable.Instance.Execute();

                    I.Expect(actual).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should return false when one error found")]
            public void Spec03()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file.js", 1);

                    var actual = testable.Instance.Execute();

                    I.Expect(actual).ToBeFalse();
                }
            }

            [Fact(DisplayName = "Should return false when many errors found")]
            public void Spec04()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file1.js", 5);
                    testable.SetupJSLintFile("file2.js", 0);
                    testable.SetupJSLintFile("file3.js", 3);

                    var actual = testable.Instance.Execute();

                    I.Expect(actual).ToBeFalse();
                }
            }

            [Fact(DisplayName = "Should return true when errors found but treating errors as warnings")]
            public void Spec05()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file.js", 2);
                    testable.Settings.Output = Output.Warning;
                    testable.SettingsExist = true;

                    var actual = testable.Instance.Execute();

                    I.Expect(actual).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should lint the content of each file provided")]
            public void Spec06()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file1.js", 0);
                    testable.SetupJSLintFile("jsfile2.js", 0);

                    testable.Instance.Execute();

                    testable.JSLintContextMock.Verify(x => x.Lint("file1.js contents", It.IsAny<JSLintOptions>()));
                    testable.JSLintContextMock.Verify(x => x.Lint("jsfile2.js contents", It.IsAny<JSLintOptions>()));
                    testable.JSLintContextMock.Verify(x => x.Lint(It.IsAny<string>(), It.IsAny<JSLintOptions>()), Times.Exactly(2));
                }
            }

            [Fact(DisplayName = "Should log each jslint error as a task error")]
            public void Spec07()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file1.js", 2);
                    testable.SetupJSLintFile("jsfile2.js", 1);

                    testable.Instance.Execute();

                    testable.LoggingHelperMock.Verify(x => x.LogError(It.IsAny<string>(), null, null, It.Is<string>(y => y.EndsWith("file1.js")), 1, 1, 0, 0, "JSLint : file1.js message 1"));
                    testable.LoggingHelperMock.Verify(x => x.LogError(It.IsAny<string>(), null, null, It.Is<string>(y => y.EndsWith("file1.js")), 2, 2, 0, 0, "JSLint : file1.js message 2"));
                    testable.LoggingHelperMock.Verify(x => x.LogError(It.IsAny<string>(), null, null, It.Is<string>(y => y.EndsWith("jsfile2.js")), 1, 1, 0, 0, "JSLint : jsfile2.js message 1"));
                    testable.LoggingHelperMock.Verify(x => x.LogWarning(It.IsAny<string>(), null, null, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), 0, 0, It.IsAny<string>()), Times.Never());
                }
            }

            [Fact(DisplayName = "Should log each jslint error as a task warning when treating errors as warnings")]
            public void Spec08()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file1.js", 2);
                    testable.SetupJSLintFile("jsfile2.js", 1);
                    testable.Settings.Output = Output.Warning;
                    testable.SettingsExist = true;

                    testable.Instance.Execute();

                    testable.LoggingHelperMock.Verify(x => x.LogWarning(It.IsAny<string>(), null, null, It.Is<string>(y => y.EndsWith("file1.js")), 1, 1, 0, 0, "JSLint : file1.js message 1"));
                    testable.LoggingHelperMock.Verify(x => x.LogWarning(It.IsAny<string>(), null, null, It.Is<string>(y => y.EndsWith("file1.js")), 2, 2, 0, 0, "JSLint : file1.js message 2"));
                    testable.LoggingHelperMock.Verify(x => x.LogWarning(It.IsAny<string>(), null, null, It.Is<string>(y => y.EndsWith("jsfile2.js")), 1, 1, 0, 0, "JSLint : jsfile2.js message 1"));
                    testable.LoggingHelperMock.Verify(x => x.LogError(It.IsAny<string>(), null, null, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), 0, 0, It.IsAny<string>()), Times.Never());
                }
            }

            [Fact(DisplayName = "Should set error count to total number of error")]
            public void Spec09()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file1.js", 2);
                    testable.SetupJSLintFile("file2.js", 1);
                    testable.SetupJSLintFile("file3.js", 5);
                    testable.SetupJSLintFile("file4.js", 2);

                    testable.Instance.Execute();

                    I.Expect(testable.Instance.ErrorCount).ToBe(10);
                }
            }

            [Fact(DisplayName = "Should set error file count to total number of files with errors")]
            public void Spec10()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file1.js", 2);
                    testable.SetupJSLintFile("file2.js", 0);
                    testable.SetupJSLintFile("file3.js", 3);

                    testable.Instance.Execute();

                    I.Expect(testable.Instance.ErrorFileCount).ToBe(2);
                }
            }

            [Fact(DisplayName = "Should set process file count to total count of files processed")]
            public void Spec11()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file1.js", 3);
                    testable.SetupJSLintFile("file2.js", 0);
                    testable.SetupJSLintFile("file3.js", 0);
                    testable.SetupJSLintFile("file4.js", 1);

                    testable.Instance.Execute();

                    I.Expect(testable.Instance.ProcessedFileCount).ToBe(4);
                }
            }

            [Fact(DisplayName = "Should not try to create a reporter if no source files exist")]
            public void Spec12()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.Instance.Execute();

                    testable.Verify<IJSLintFactory>(x => x.CreateReportBuilder(), Times.Never());
                }
            }

            [Fact(DisplayName = "Should try to create a report builder")]
            public void Spec13()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file.js", 0);

                    testable.Instance.Execute();

                    testable.Verify<IJSLintFactory>(x => x.CreateReportBuilder());
                }
            }

            [Fact(DisplayName = "Should add all files with data to report builder if it exists")]
            public void Spec14()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file1.js", 3);
                    testable.SetupJSLintFile("file2.js", 5);

                    testable.Instance.Execute();

                    testable.JSLintReportBuilderMock.Verify(x => x.AddFile(It.Is<string>(y => y.EndsWith("file1.js")), It.IsAny<IJSLintData>()));
                    testable.JSLintReportBuilderMock.Verify(x => x.AddFile(It.Is<string>(y => y.EndsWith("file2.js")), It.IsAny<IJSLintData>()));
                }
            }

            [Fact(DisplayName = "Should save report as UTF8 if it exists")]
            public void Spec15()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file.js", 0);

                    testable.Instance.ReportFile = "REPORTFILE";
                    testable.Instance.Execute();

                    testable.JSLintReportBuilderMock.Verify(x => x.ToString());
                    testable.Verify<IFileSystemWrapper>(x => x.WriteAllText(@"D:\solution\source\project\REPORTFILE", "REPORTRESULT", Encoding.UTF8));
                }
            }

            [Fact(DisplayName = "Should stop when default error limit reached")]
            public void Spec16()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file.js", 0);
                    testable.ErrorCount = JSLintNetSettings.DefaultErrorLimit;

                    testable.Instance.Execute();

                    testable.LoggingHelperMock.Verify(x => x.LogError(Resources.ErrorLimitReachedFormat, JSLintNetSettings.DefaultErrorLimit));
                }
            }

            [Fact(DisplayName = "Should stop when hard exception limit reached")]
            public void Spec17()
            {
                using (var testable = new ExecuteTestable())
                {
                    for (int i = 0; i < JSLintNetSettings.ExceptionLimit; i++)
                    {
                        var filePath = Path.Combine(testable.SourceDirectory, "file" + i + ".js");
                        testable.SourceFiles.Add(filePath);
                    }

                    testable.JSLintContextMock
                        .Setup(x => x.Lint(It.IsAny<string>(), It.IsAny<JSLintOptions>()))
                        .Throws<Exception>();

                    testable.Instance.Execute();

                    testable.LoggingHelperMock.Verify(x => x.LogError(Resources.ExceptionLimitReachedFormat, JSLintNetSettings.ExceptionLimit));
                }
            }

            [Fact(DisplayName = "Should stop when default file limit reached")]
            public void Spec18()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file.js", 0);
                    testable.ProcessedFileCount = JSLintNetSettings.DefaultFileLimit;

                    testable.Instance.Execute();

                    testable.LoggingHelperMock.Verify(x => x.LogError(Resources.FileLimitReachedFormat, JSLintNetSettings.DefaultFileLimit));
                }
            }

            [Fact(DisplayName = "Should stop when custom error limit reached")]
            public void Spec19()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.Settings.ErrorLimit = 10;
                    testable.SettingsExist = true;
                    testable.SetupJSLintFile("file.js", 11);

                    testable.Instance.Execute();

                    testable.LoggingHelperMock.Verify(x => x.LogError(Resources.ErrorLimitReachedFormat, 11));
                }
            }

            [Fact(DisplayName = "Should stop when custom file limit reached")]
            public void Spec20()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.Settings.FileLimit = 10;
                    testable.SettingsExist = true;

                    testable.SetupJSLintFile("file.js", 0);
                    testable.ProcessedFileCount = 11;

                    testable.Instance.Execute();

                    testable.LoggingHelperMock.Verify(x => x.LogError(Resources.FileLimitReachedFormat, 11));
                }
            }

            private class ExecuteTestable : JSLintTaskTestableBase
            {
                public ExecuteTestable()
                {
                    this.SourceDirectory = @"D:\solution\source\project";
                    this.JSLintContextMock = new Mock<IJSLintContext>();
                    this.JSLintReportBuilderMock = new Mock<IJSLintReportBuilder>();
                    this.SourceFiles = new List<string>();
                    this.Settings = new JSLintNetSettings();

                    this.BeforeInit += this.OnBeforeInit;

                    this.AfterInit += this.OnAfterInit;
                }

                public Mock<IJSLintContext> JSLintContextMock { get; set; }

                public Mock<IJSLintReportBuilder> JSLintReportBuilderMock { get; set; }

                public int ProcessedFileCount { get; set; }

                public int ErrorFileCount { get; set; }

                public int ErrorCount { get; set; }

                public string SourceDirectory { get; set; }

                public List<string> SourceFiles { get; set; }

                public bool SettingsExist { get; set; }

                public JSLintNetSettings Settings { get; set; }

                public void SetupJSLintFile(string fileName, int errorCount = 0)
                {
                    using (var testable = new ExecuteTestable())
                    {
                        var filePath = Path.Combine(this.SourceDirectory, fileName);
                        this.SourceFiles.Add(filePath);

                        var contents = fileName + " contents";
                        var data = new JSLintDataFake(fileName, errorCount);

                        this.GetMock<IFileSystemWrapper>()
                            .Setup(x => x.ReadAllText(filePath, It.IsAny<Encoding>()))
                            .Returns(contents);

                        this.JSLintContextMock
                            .Setup(x => x.Lint(contents, It.IsAny<JSLintOptions>()))
                            .Returns(data);

                        this.ProcessedFileCount += 1;

                        if (errorCount > 0)
                        {
                            this.ErrorFileCount += 1;
                            this.ErrorCount += errorCount;
                        }
                    }
                }

                private void OnBeforeInit(object sender, EventArgs e)
                {
                    this.GetMock<IJSLintFactory>()
                        .Setup(x => x.CreateContext())
                        .Returns(this.JSLintContextMock.Object);

                    this.GetMock<IJSLintFactory>()
                        .Setup(x => x.CreateReportBuilder())
                        .Returns(this.JSLintReportBuilderMock.Object);

                    this.JSLintReportBuilderMock
                        .Setup(x => x.ToString())
                        .Returns("REPORTRESULT");

                    this.JSLintReportBuilderMock
                        .SetupGet(x => x.ErrorCount)
                        .Returns(() => this.ErrorCount);

                    this.JSLintReportBuilderMock
                        .SetupGet(x => x.ErrorFileCount)
                        .Returns(() => this.ErrorFileCount);

                    this.JSLintReportBuilderMock
                        .SetupGet(x => x.ProcessedFileCount)
                        .Returns(() => this.ProcessedFileCount);

                    this.GetMock<IFileSystemWrapper>()
                        .Setup(x => x.GetFiles(this.SourceDirectory, It.IsAny<string>(), SearchOption.AllDirectories))
                        .Returns(() => this.SourceFiles.ToArray());

                    var settingsPath = Path.Combine(this.SourceDirectory, JSLintNetSettings.FileName);
                    this.GetMock<IFileSystemWrapper>()
                        .Setup(x => x.FileExists(settingsPath))
                        .Returns(() => this.SettingsExist);

                    this.GetMock<IFileSystemWrapper>()
                        .Setup(x => x.ReadAllText(settingsPath, Encoding.UTF8))
                        .Returns("SETTINGS");

                    this.GetMock<ISettingsRepository>()
                        .Setup(x => x.Load(It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(this.Settings);
                }

                private void OnAfterInit(object sender, EventArgs e)
                {
                    this.Instance.SourceDirectory = this.SourceDirectory;
                }
            }
        }

        private abstract class JSLintTaskTestableBase : TestableBase<JSLintTask>
        {
            public JSLintTaskTestableBase()
            {
                this.LoggingHelperMock = new Mock<ITaskLoggingHelper>();

                this.BeforeInit += this.OnBeforeInit;
            }

            public Mock<ITaskLoggingHelper> LoggingHelperMock { get; set; }

            private void OnBeforeInit(object sender, EventArgs e)
            {
                this.GetMock<IAbstractionFactory>()
                    .Setup(x => x.CreateTaskLoggingHelper(It.IsAny<ITask>()))
                    .Returns(this.LoggingHelperMock.Object);
            }
        }
    }
}
