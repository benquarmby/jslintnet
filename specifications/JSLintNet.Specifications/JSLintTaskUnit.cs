namespace JSLintNet.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using JSLintNet;
    using JSLintNet.Abstractions;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using JSLintNet.QualityTools.Fakes;
    using JSLintNet.Settings;
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

                    testable.JSLintContextMock.Verify(x => x.Lint("file1.js contents", It.IsAny<JSLintOptions>(), It.IsAny<IList<string>>()));
                    testable.JSLintContextMock.Verify(x => x.Lint("jsfile2.js contents", It.IsAny<JSLintOptions>(), It.IsAny<IList<string>>()));
                    testable.JSLintContextMock.Verify(x => x.Lint(It.IsAny<string>(), It.IsAny<JSLintOptions>(), It.IsAny<IList<string>>()), Times.Exactly(2));
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

            [Fact(DisplayName = "Should stop when default error limit reached")]
            public void Spec16()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file.js", 0);
                    testable.ErrorCount = JSLintNetSettings.DefaultErrorLimit;

                    testable.Instance.Execute();
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
                        .Setup(x => x.Lint(It.IsAny<string>(), It.IsAny<JSLintOptions>(), It.IsAny<IList<string>>()))
                        .Throws<Exception>();

                    testable.Instance.Execute();
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
                }
            }

            private class ExecuteTestable : JSLintTaskTestableBase
            {
                public ExecuteTestable()
                {
                    this.SourceDirectory = @"D:\solution\source\project";
                    this.JSLintContextMock = new Mock<IJSLintContext>();
                    this.SourceFiles = new List<string>();
                    this.Settings = new JSLintNetSettings();
                }

                public Mock<IJSLintContext> JSLintContextMock { get; set; }

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
                            .Setup(x => x.Lint(contents, It.IsAny<JSLintOptions>(), It.IsAny<IList<string>>()))
                            .Returns(data);

                        this.ProcessedFileCount += 1;

                        if (errorCount > 0)
                        {
                            this.ErrorFileCount += 1;
                            this.ErrorCount += errorCount;
                        }
                    }
                }

                protected override void BeforeResolve()
                {
                    base.BeforeResolve();

                    this.GetMock<IJSLintFactory>()
                        .Setup(x => x.CreateContext())
                        .Returns(this.JSLintContextMock.Object);

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

                protected override void AfterResolve()
                {
                    base.AfterResolve();

                    this.Instance.SourceDirectory = this.SourceDirectory;
                }
            }
        }

        private abstract class JSLintTaskTestableBase : MockTestFixture<JSLintTask>
        {
        }
    }
}
