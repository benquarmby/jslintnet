namespace JSLintNet.Specifications
{
    using System.Collections.Generic;
    using Xunit;

    [Trait("Category", "Unit")]
    public class JSLintTaskTests
    {
        [Fact(DisplayName = "Should log JSLint errors at the correct line and column")]
        public void CorrectLineColumn()
        {
            using (var fixture = new JSLintTaskFixture())
            {
                const string File1Script = @"
var global = unknown();";

                fixture.Instance.SourceDirectory = "Some Path";
                fixture.FileSystemWrapper.FileResults.Add(
                    "Some Path",
                    new List<string>()
                    {
                        "file1.js"
                    });

                fixture.FileSystemWrapper.Files.Add("Some Path\\file1.js", File1Script);

                var actual = fixture.Instance.Execute();
                var firstError = fixture.BuildEngine.ErrorEvents[0];

                Assert.False(actual);
                Assert.Equal(1, firstError.LineNumber);
                Assert.Equal(13, firstError.ColumnNumber);
            }
        }
    }
}
