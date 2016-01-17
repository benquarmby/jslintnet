namespace JSLintNet.VisualStudio.Extensions.Documents
{
    using EnvDTE;
    using System;

    internal class DocumentAccessor : IDocumentAccessor
    {
        private const string TextDocumentKey = "TextDocument";

        private Document document;

        public DocumentAccessor(Document document)
        {
            this.document = document;
        }

        public TextDocument TextDocument
        {
            get
            {
                return (TextDocument)this.document.Object(TextDocumentKey);
            }
        }

        public string Source
        {
            get
            {
                var textDocument = this.TextDocument;
                var editPoint = textDocument.StartPoint.CreateEditPoint();
                var source = editPoint.GetText(textDocument.EndPoint);

                return source;
            }
        }

        public string LineEnding
        {
            get
            {
                var textDocument = this.TextDocument;

                if (textDocument.EndPoint.Line == 1)
                {
                    return Environment.NewLine;
                }

                var startPoint = textDocument.StartPoint.CreateEditPoint();
                startPoint.CharRight(startPoint.LineLength);

                return startPoint.GetText(1);
            }
        }
    }
}
