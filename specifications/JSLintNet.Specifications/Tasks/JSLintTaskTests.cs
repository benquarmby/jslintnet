namespace JSLintNet.Specifications.Tasks
{
    using IExpect;
    using Xunit;

    [Trait("Category", "Unit")]
    public class JSLintTaskTests
    {
        [Fact(DisplayName = "Should log JSLint errors at the correct line and column")]
        public void CorrectLineColumn()
        {
            using (var fixture = new JSLintTaskFixture())
            {
                fixture.Instance.SourceDirectory = "Some Path";
                fixture.AddFile("file1.js", TaskResources.WindowGlobalStrict);

                var actual = fixture.Instance.Execute();
                var firstError = fixture.BuildEngine.ErrorEvents[0];

                Assert.False(actual);
                Assert.Equal(3, firstError.LineNumber);
                Assert.Equal(14, firstError.ColumnNumber);
            }
        }

        [Fact(DisplayName = "Should log each JSLint warning as a task error")]
        public void TaskErrors()
        {
            using (var fixture = new JSLintTaskFixture())
            {
                fixture.Instance.SourceDirectory = @"\\SHARE\path\";
                fixture.AddFile("first.js", TaskResources.WindowGlobalStrict);
                fixture.AddFile("second.js", TaskResources.AmdNoStrict);

                fixture.Instance.Execute();

                I.Expect(fixture.BuildEngine.ErrorEvents.Count).ToBe(3);
                I.Expect(fixture.BuildEngine.WarningEvents.Count).ToBe(0);
                I.Expect(fixture.BuildEngine.MessageEvents.Count).ToBe(0);
            }
        }

        [Fact(DisplayName = "Should log each JSLint warning as a task warning when outputting warnings")]
        public void TaskWarnings()
        {
            using (var fixture = new JSLintTaskFixture())
            {
                fixture.Instance.SourceDirectory = @"\\SHARE\path\";
                fixture.AddFile("JSLintNet.json", TaskResources.WarningSettings);
                fixture.AddFile("first.js", TaskResources.WindowGlobalStrict);
                fixture.AddFile("second.js", TaskResources.AmdNoStrict);

                fixture.Instance.Execute();

                I.Expect(fixture.BuildEngine.ErrorEvents.Count).ToBe(0);
                I.Expect(fixture.BuildEngine.WarningEvents.Count).ToBe(3);
                I.Expect(fixture.BuildEngine.MessageEvents.Count).ToBe(0);
            }
        }

        [Fact(DisplayName = "Should log each JSLint warning as a task message when outputting messages")]
        public void TaskMessages()
        {
            using (var fixture = new JSLintTaskFixture())
            {
                fixture.Instance.SourceDirectory = @"\\SHARE\path\";
                fixture.AddFile("JSLintNet.json", TaskResources.MessageSettings);
                fixture.AddFile("first.js", TaskResources.WindowGlobalStrict);
                fixture.AddFile("second.js", TaskResources.AmdNoStrict);

                fixture.Instance.Execute();

                I.Expect(fixture.BuildEngine.ErrorEvents.Count).ToBe(0);
                I.Expect(fixture.BuildEngine.WarningEvents.Count).ToBe(0);
                I.Expect(fixture.BuildEngine.MessageEvents.Count).ToBe(3);
            }
        }

        [Fact(DisplayName = "Should return true when no files found")]
        public void NoFilesOk()
        {
            using (var fixture = new JSLintTaskFixture())
            {
                fixture.Instance.SourceDirectory = @"C:\Source\";

                var actual = fixture.Instance.Execute();

                I.Expect(actual).ToBeTrue();
            }
        }

        [Fact(DisplayName = "Should return true when no files contain errors")]
        public void NoErrorsOk()
        {
            using (var fixture = new JSLintTaskFixture())
            {
                fixture.Instance.SourceDirectory = @"Some Path";
                fixture.AddFile("file1.js", TaskResources.GlobalVariableDef);

                var actual = fixture.Instance.Execute();

                I.Expect(actual).ToBeTrue();
            }
        }

        [Fact(DisplayName = "Should return false when one error found")]
        public void OneErrorFail()
        {
            using (var fixture = new JSLintTaskFixture())
            {
                fixture.Instance.SourceDirectory = @"D:\Solution\Project Dir";
                fixture.AddFile("startup.js", TaskResources.WindowGlobalStrict);

                var actual = fixture.Instance.Execute();

                I.Expect(fixture.Instance.ErrorCount).ToBe(1);
                I.Expect(actual).ToBeFalse();
            }
        }

        [Fact(DisplayName = "Should return false when many errors found")]
        public void ManyErrorsFail()
        {
            using (var fixture = new JSLintTaskFixture())
            {
                fixture.Instance.SourceDirectory = @"D:\Solution\Project Dir";
                fixture.AddFile("first.js", TaskResources.WindowGlobalStrict);
                fixture.AddFile("second.js", TaskResources.AmdNoStrict);

                var actual = fixture.Instance.Execute();

                I.Expect(fixture.Instance.ErrorCount).ToBe(3);
                I.Expect(actual).ToBeFalse();
            }
        }

        [Fact(DisplayName = "Should return true when errors found but treating errors as warnings")]
        public void WarningsOk()
        {
            using (var fixture = new JSLintTaskFixture())
            {
                fixture.Instance.SourceDirectory = @"Z:\Funky Path\repo-of-doom";
                fixture.AddFile("JSLintNet.json", TaskResources.WarningSettings);
                fixture.AddFile("second.js", TaskResources.AmdNoStrict);

                var actual = fixture.Instance.Execute();

                I.Expect(fixture.Instance.ErrorCount).ToBeGreaterThan(0);
                I.Expect(actual).ToBeTrue();
            }
        }
    }
}
