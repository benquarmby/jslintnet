namespace JSLintNet.QualityTools.Fakes
{
    internal class JSLintWarningFake : JSLintWarning
    {
        public JSLintWarningFake()
        {
        }

        public JSLintWarningFake(string fileName, int index)
        {
            this.Line = index;
            this.Column = index;
            this.Message = fileName + " message " + index;
            this.Code = fileName + " code " + index;
        }
    }
}
