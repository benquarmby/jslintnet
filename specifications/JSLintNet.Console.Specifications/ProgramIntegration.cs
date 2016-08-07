namespace JSLintNet.Console.Specifications
{
    using System;
    using System.IO;
    using IExpect;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Helpers;
    using Xunit;

    public class ProgramIntegration : IntegrationBase
    {
        private static readonly string ProgramPath = new Uri(typeof(Program).Assembly.CodeBase).LocalPath;

        private static readonly string ScriptsRoot = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\Resources\Scripts"));

        [Fact(DisplayName = "Should find errors in source directory")]
        public void Spec01()
        {
            var result = ProcessHelper.Execute(ProgramPath, @"""" + ScriptsRoot + @"""");

            I.Expect(result.ExitCode).ToBeGreaterThan(0);
        }

        [Fact(DisplayName = "Should stop when error limit reached")]
        public void Spec02()
        {
            var args = @"""" + ScriptsRoot + @""" /l """"Verbose"""" /s """ + ScriptsRoot + @"\Limits.json""";
            var result = ProcessHelper.Execute(ProgramPath, args);

            I.Expect(result.ExitCode).ToBeGreaterThan(100);
            I.Expect(result.ExitCode).ToBeLessThan(150);
            I.Expect(result.Error).ToContain("Total JSLint error limit reached:");
        }

        [Fact(DisplayName = "Should find errors in jQuery source")]
        public void Spec03()
        {
            var args = @"""" + ScriptsRoot + @"\jQuery""";
            var result = ProcessHelper.Execute(ProgramPath, args);

            I.Expect(result.ExitCode).ToBeGreaterThan(100);
        }
    }
}
