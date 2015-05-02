namespace JSLintNet.QualityTools.Fakes
{
    using JSLintNet.Models;

    public class JSLintErrorFake : IJSLintWarning
    {
        public JSLintErrorFake(string fileName, int index)
        {
            this.Line = index;
            this.Column = index;
            this.Message = fileName + " message " + index;
            this.Evidence = fileName + " evidence " + index;
            this.Code = fileName + " code " + index;
        }

        public int Line { get; set; }

        public int Column { get; set; }

        public string Message { get; set; }

        public string Evidence { get; set; }

        public string Code { get; set; }
    }
}
