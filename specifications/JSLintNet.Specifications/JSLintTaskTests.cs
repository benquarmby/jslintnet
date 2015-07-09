namespace JSLintNet.Specifications
{
    using Xunit;

    public class JSLintTaskTests
    {
        [Fact]
        public void Something()
        {
            using (var fixture = new JSLintTaskFixture())
            {
                fixture.Instance.SourceDirectory = "Some Path";

                fixture.Instance.Execute();
            }
        }
    }
}
