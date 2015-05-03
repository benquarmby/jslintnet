namespace JSLintNet.QualityTools.Fakes
{
    using System.Collections.Generic;
    using JSLintNet.Models;

    public class JSLintDataFake : IJSLintData
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

        public IList<IJSLintWarning> Warnings { get; set; }

        public IList<string> Global { get; set; }

        public bool Json { get; set; }

        public string Report { get; set; }

        public bool Stop { get; set; }

        public string Edition { get; set; }

        public IList<string> Imports { get; set; }

        public bool Module { get; set; }

        public bool OK { get; set; }

        public IDictionary<string, int> Property { get; set; }

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
