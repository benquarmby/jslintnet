namespace JSLintNet
{
    using System.Collections.Generic;

    internal class JSLintData : IJSLintData
    {
        public string Edition { get; set; }

        public IList<IJSLintToken> Directives { get; set; }

        public IList<string> Imports { get; set; }

        public bool Json { get; set; }

        public bool Module { get; set; }

        public bool OK { get; set; }

        public IDictionary<string, int> Property { get; set; }

        public string PropertyDirective { get; set; }

        public bool Stop { get; set; }

        public IList<IJSLintWarning> Warnings { get; set; }

        public string Report { get; set; }
    }
}
