namespace JSLintNet.QualityTools.Fakes
{
    using JSLintNet.Models;

    public class JSLintErrorFake : IJSLintError
    {
        public JSLintErrorFake(string fileName, int index)
        {
            this.Line = index;
            this.Character = index;
            this.Reason = fileName + " message " + index;
            this.Evidence = fileName + " evidence " + index;
            this.Code = fileName + " code " + index;
        }

        public int Line { get; set; }

        public int Character { get; set; }

        public string Reason { get; set; }

        public string Evidence { get; set; }

        public string Code { get; set; }
    }
}
