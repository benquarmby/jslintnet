namespace JSLintNet.QualityTools.Fakes
{
    internal class JSLintDataFake : JSLintData
    {
        public JSLintDataFake()
        {
        }

        public JSLintDataFake(string fileName, int errorCount)
        {
            this.FileName = fileName;
            this.SetWarnings(errorCount);
        }

        public string FileName { get; set; }

        public void SetWarnings(int count)
        {
            var warnings = new IJSLintWarning[count];

            for (int i = 0; i < count; i++)
            {
                var number = i + 1;
                warnings[i] = new JSLintWarningFake(this.FileName, number);
            }

            this.Warnings = warnings;
        }
    }
}
