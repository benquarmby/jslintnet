﻿namespace JSLintNet.Specifications.Tasks
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using IExpect;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Helpers;
    using JSLintNet.Settings;
    using Microsoft.Win32;
    using Xunit;

    public class JSLintTaskIntegration : IntegrationBase
    {
        [Fact(DisplayName = "Should fail with correct counts using errors project")]
        public void Errors()
        {
            var actual = JSLintTaskHelper.ExecuteMSBuildProject("Errors");

            I.Expect(actual.Success).ToBeFalse();
            I.Expect(actual.ErrorCount).ToBe(9);
            I.Expect(actual.ErrorFileCount).ToBe(3);
            I.Expect(actual.ProcessedFileCount).ToBe(5);
        }

        [Fact(DisplayName = "Should fail with correct counts using source files project")]
        public void SourceFiles()
        {
            var actual = JSLintTaskHelper.ExecuteMSBuildProject("SourceFiles");

            I.Expect(actual.Success).ToBeFalse();
            I.Expect(actual.ErrorCount).ToBe(9);
            I.Expect(actual.ErrorFileCount).ToBe(3);
            I.Expect(actual.ProcessedFileCount).ToBe(3);
        }

        [Fact(DisplayName = "Should succeed with correct counts using warnings project")]
        public void Warnings()
        {
            var actual = JSLintTaskHelper.ExecuteMSBuildProject("Warnings");

            I.Expect(actual.Success).ToBeTrue();
            I.Expect(actual.ErrorCount).ToBe(9);
            I.Expect(actual.ErrorFileCount).ToBe(3);
            I.Expect(actual.ProcessedFileCount).ToBe(5);
        }

        [Fact(DisplayName = "Should fail when source directory property omitted")]
        public void NoSourceDirectory()
        {
            var actual = JSLintTaskHelper.ExecuteMSBuildProject("NoSourceDirectory");

            I.Expect(actual.Success).ToBeFalse();
            I.Expect(actual.Output).ToContain("task was not given a value for the required parameter \"SourceDirectory\"");
        }

        [Fact(DisplayName = "Should save html report with report file property")]
        public void HtmlReport()
        {
            var actual = JSLintTaskHelper.ExecuteMSBuildProject("HtmlReport");
            var reportPath = Path.Combine(JSLintTaskHelper.ProjectRoot, @"Scripts\JSLintReport.html");

            I.Expect(File.Exists(reportPath)).ToBeTrue();

            File.Delete(reportPath);
        }

        [Fact(DisplayName = "Should merge settings from configuration version")]
        public void ConfigurationMerge()
        {
            var actual = JSLintTaskHelper.ExecuteMSBuildProject("Configuration", "/p:Configuration=Release");

            I.Expect(actual.Success).ToBeFalse();
            I.Expect(actual.ErrorCount).ToBe(9);
        }

        [Fact(DisplayName = "Should load settings from linked JSLintNet.json file")]
        public void LinkedSettings()
        {
            var actual = JSLintTaskHelper.ExecuteMSBuildProject("LinkedSettings");

            I.Expect(actual.Success).ToBeTrue();
            I.Expect(actual.OutputType).ToBe(Output.Warning);
            I.Expect(actual.ProcessedFileCount).ToBe(6);
        }

        private static class JSLintTaskHelper
        {
            public static readonly string MSBuildExecutable;

            public static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Resources"));

            public static readonly Regex ErrorCountPattern = new Regex(@"JSLINTERRORCOUNT=(?<Count>[\d]+)=JSLINTERRORCOUNT", RegexOptions.Compiled);

            public static readonly Regex ErrorFileCountPattern = new Regex(@"JSLINTERRORFILECOUNT=(?<Count>[\d]+)=JSLINTERRORFILECOUNT", RegexOptions.Compiled);

            public static readonly Regex ProcessedFileCountPattern = new Regex(@"JSLINTPROCESSEDFILECOUNT=(?<Count>[\d]+)=JSLINTPROCESSEDFILECOUNT", RegexOptions.Compiled);

            public static readonly Regex OutputPattern = new Regex(AssemblyInfo.Product + " (error|warning|message) :", RegexOptions.Compiled);

            static JSLintTaskHelper()
            {
                var msBuildDirectory = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0", "MSBuildToolsPath", null) as string;
                if (string.IsNullOrEmpty(msBuildDirectory))
                {
                    throw new Exception("Could not locate MSBuild tools path in the registry.");
                }

                MSBuildExecutable = Path.Combine(msBuildDirectory, "MSBuild.exe");
            }

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
                return ExecuteMSBuildProject(projectName, null);
            }

            public static JSLintTaskResult ExecuteMSBuildProject(string projectName, string args)
            {
                var arguments = projectName + ".proj";

                if (!string.IsNullOrEmpty(args))
                {
                    arguments += " " + args;
                }

                var result = ProcessHelper.Execute(MSBuildExecutable, arguments, ProjectRoot);
                var exitCode = result.ExitCode;
                var output = result.Output;

                return new JSLintTaskResult(result)
                {
                    ErrorCount = ParseCount(ErrorCountPattern, output),
                    ErrorFileCount = ParseCount(ErrorFileCountPattern, output),
                    ProcessedFileCount = ParseCount(ProcessedFileCountPattern, output),
                    Success = exitCode == 0 && output.Contains("Build succeeded."),
                    OutputType = DetectOutputType(output)
                };
            }

            private static Output? DetectOutputType(string output)
            {
                var match = OutputPattern.Match(output);

                if (match.Success && match.Groups.Count == 2)
                {
                    var rawType = match.Groups[1].Value;
                    Output outputType;

                    if (Enum.TryParse<Output>(rawType, true, out outputType))
                    {
                        return outputType;
                    }
                }

                return null;
            }

            public class JSLintTaskResult : ProcessResult
            {
                public JSLintTaskResult()
                {
                }

                public JSLintTaskResult(ProcessResult result)
                {
                    this.ExitCode = result.ExitCode;
                    this.Output = result.Output;
                    this.Error = result.Error;
                }

                public int ErrorCount { get; set; }

                public int ErrorFileCount { get; set; }

                public int ProcessedFileCount { get; set; }

                public bool Success { get; set; }

                public Output? OutputType { get; set; }
            }
        }
    }
}
