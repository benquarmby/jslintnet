namespace JSLintNet
{
    internal class JSLintWarning : IJSLintWarning
    {
        public int Line { get; set; }

        public int Column { get; set; }

        public string Message { get; set; }

        public string Code { get; set; }
    }
}
