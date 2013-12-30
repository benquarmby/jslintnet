namespace JSLintNet.Specifications
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using JSLintNet.QualityTools.Helpers;
    using Xunit;

    public class JSLintTaskIntegration : IntegrationBase
    {
        [Fact(DisplayName = "Should fail with correct counts using errors project")]
        public void Spec01()
        {
            var actual = JSLintTaskHelper.ExecuteMSBuildProject("Errors");

            I.Expect(actual.Success).ToBeFalse();
            I.Expect(actual.ErrorCount).ToBe(12);
            I.Expect(actual.ErrorFileCount).ToBe(3);
            I.Expect(actual.ProcessedFileCount).ToBe(5);
        }

        [Fact(DisplayName = "Should fail with correct counts using source files project")]
        public void Spec02()
        {
            var actual = JSLintTaskHelper.ExecuteMSBuildProject("SourceFiles");

            I.Expect(actual.Success).ToBeFalse();
            I.Expect(actual.ErrorCount).ToBe(12);
            I.Expect(actual.ErrorFileCount).ToBe(3);
            I.Expect(actual.ProcessedFileCount).ToBe(3);
        }

        [Fact(DisplayName = "Should succeed with correct counts using warnings project")]
        public void Spec03()
        {
            var actual = JSLintTaskHelper.ExecuteMSBuildProject("Warnings");

            I.Expect(actual.Success).ToBeTrue();
            I.Expect(actual.ErrorCount).ToBe(12);
            I.Expect(actual.ErrorFileCount).ToBe(3);
            I.Expect(actual.ProcessedFileCount).ToBe(5);
        }

        [Fact(DisplayName = "Should fail when source directory property omitted")]
        public void Spec04()
        {
            var actual = JSLintTaskHelper.ExecuteMSBuildProject("NoSourceDirectory");

            I.Expect(actual.Success).ToBeFalse();
            I.Expect(actual.Output).ToContain("task was not given a value for the required parameter \"SourceDirectory\"");
        }

        [Fact(DisplayName = "Should save html report with report file property")]
        public void Spec05()
        {
            var actual = JSLintTaskHelper.ExecuteMSBuildProject("HtmlReport");
            var reportPath = Path.Combine(JSLintTaskHelper.ProjectRoot, @"Scripts\JSLintReport.html");

            I.Expect(File.Exists(reportPath)).ToBeTrue();

            File.Delete(reportPath);
        }

        [Fact(DisplayName = "Should override output from settings with explicit property")]
        public void Spec06()
        {
            var actual = JSLintTaskHelper.ExecuteMSBuildProject("OutputOverride");

            I.Expect(actual.Success).ToBeFalse();
            I.Expect(actual.ErrorCount).ToBe(12);
        }

        private static class JSLintTaskHelper
        {
            public static readonly string MSBuildExecutable = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework\v4.0.30319\MSBuild.exe");

            public static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\Resources"));

            public static readonly Regex ErrorCountPattern = new Regex(@"JSLINTERRORCOUNT=(?<Count>[\d]+)=JSLINTERRORCOUNT", RegexOptions.Compiled);

            public static readonly Regex ErrorFileCountPattern = new Regex(@"JSLINTERRORFILECOUNT=(?<Count>[\d]+)=JSLINTERRORFILECOUNT", RegexOptions.Compiled);

            public static readonly Regex ProcessedFileCountPattern = new Regex(@"JSLINTPROCESSEDFILECOUNT=(?<Count>[\d]+)=JSLINTPROCESSEDFILECOUNT", RegexOptions.Compiled);

            public static int ParseCount(Regex pattern, string input)
            {
                var match = pattern.Match(input);

                if (match.Success && match.Groups["Count"].Success)
                {
                    int count;

                    if (int.TryParse(match.Groups["Count"].Value, out count))
                    {
                        return count;
                    }
                }

                return -1;
            }

            public static JSLintTaskResult ExecuteMSBuildProject(string projectName)
            {
                var result = ProcessHelper.Execute(MSBuildExecutable, projectName + ".proj", ProjectRoot);
                var exitCode = result.ExitCode;
                var output = result.Output;

                return new JSLintTaskResult()
                {
                    ExitCode = result.ExitCode,
                    Output = result.Output,
                    Error = result.Error,
                    ErrorCount = ParseCount(ErrorCountPattern, output),
                    ErrorFileCount = ParseCount(ErrorFileCountPattern, output),
                    ProcessedFileCount = ParseCount(ProcessedFileCountPattern, output),
                    Success = exitCode == 0 && output.Contains("Build succeeded.")
                };
            }

            public class JSLintTaskResult : ProcessResult
            {
                public int ErrorCount { get; set; }

                public int ErrorFileCount { get; set; }

                public int ProcessedFileCount { get; set; }

                public bool Success { get; set; }
            }
        }
    }
}
