namespace JSLintNet.Specifications.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using JSLintNet;
    using JSLintNet.Abstractions;
    using JSLintNet.Properties;
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

            [Fact(DisplayName = "Should save report as UTF8 if it exists")]
            public void Spec15()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file.js", 0);

                    testable.Instance.ReportFile = "REPORTFILE";
                    testable.Instance.Execute();

                    testable.Verify<IFileSystemWrapper>(x => x.WriteAllText(@"D:\solution\source\project\REPORTFILE", It.IsAny<string>(), Encoding.UTF8));
                }
            }

            [Fact(DisplayName = "Should stop when default error limit reached")]
            public void Spec16()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.SetupJSLintFile("file.js", JSLintNetSettings.DefaultErrorLimit);

                    testable.Instance.Execute();

                    I.Expect(testable.BuildEngine.ErrorEvents).ToContain(x => x.Message == string.Format(Resources.ErrorLimitReachedFormat, JSLintNetSettings.DefaultErrorLimit));
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

                    testable.GetMock<IJSLintContext>()
                        .Setup(x => x.Lint(It.IsAny<string>(), It.IsAny<JSLintOptions>(), It.IsAny<IList<string>>()))
                        .Throws<Exception>();

                    testable.Instance.Execute();

                    I.Expect(testable.BuildEngine.ErrorEvents).ToContain(x => x.Message == string.Format(Resources.ExceptionLimitReachedFormat, JSLintNetSettings.ExceptionLimit));
                }
            }

            [Fact(DisplayName = "Should stop when default file limit reached")]
            public void Spec18()
            {
                using (var testable = new ExecuteTestable())
                {
                    for (int i = 0; i < JSLintNetSettings.DefaultFileLimit; i++)
                    {
                        testable.SetupJSLintFile("file" + i + ".js", 0);
                    }

                    testable.Instance.Execute();

                    I.Expect(testable.BuildEngine.ErrorEvents).ToContain(x => x.Message == string.Format(Resources.FileLimitReachedFormat, JSLintNetSettings.DefaultFileLimit));
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

                    I.Expect(testable.BuildEngine.ErrorEvents).ToContain(x => x.Message == string.Format(Resources.ErrorLimitReachedFormat, 11));
                }
            }

            [Fact(DisplayName = "Should stop when custom file limit reached")]
            public void Spec20()
            {
                using (var testable = new ExecuteTestable())
                {
                    testable.Settings.FileLimit = 10;
                    testable.SettingsExist = true;

                    for (int i = 0; i < 10; i++)
                    {
                        testable.SetupJSLintFile("file" + i + ".js", 0);
                    }

                    testable.Instance.Execute();

                    I.Expect(testable.BuildEngine.ErrorEvents).ToContain(x => x.Message == string.Format(Resources.FileLimitReachedFormat, 10));
                }
            }

            private class ExecuteTestable : JSLintTaskTestableBase
            {
                public ExecuteTestable()
                {
                    this.SourceDirectory = @"D:\solution\source\project";
                    this.SourceFiles = new List<string>();
                    this.Settings = new JSLintNetSettings();
                    this.BuildEngine = new BuildEngineStub();
                }

                public string SourceDirectory { get; set; }

                public List<string> SourceFiles { get; set; }

                public bool SettingsExist { get; set; }

                public JSLintNetSettings Settings { get; set; }

                public BuildEngineStub BuildEngine { get; set; }

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

                        this.GetMock<IJSLintContext>()
                            .Setup(x => x.Lint(contents, It.IsAny<JSLintOptions>(), It.IsAny<IList<string>>()))
                            .Returns(data);
                    }
                }

                protected override void BeforeResolve()
                {
                    base.BeforeResolve();

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

                    this.Instance.BuildEngine = this.BuildEngine;
                    this.Instance.SourceDirectory = this.SourceDirectory;
                }
            }
        }

        private abstract class JSLintTaskTestableBase : MockTestFixture<JSLintTask>
        {
        }
    }
}
