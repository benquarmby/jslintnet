namespace JSLintNet
{
    using System.Collections.Generic;

    internal class JSLintToken : IJSLintToken
    {
        public string Id { get; set; }

        public bool Identifier { get; set; }

        public int From { get; set; }

        public int Thru { get; set; }

        public int Line { get; set; }

        public IList<string> Value { get; set; }

        public string Directive { get; set; }
    }
}
