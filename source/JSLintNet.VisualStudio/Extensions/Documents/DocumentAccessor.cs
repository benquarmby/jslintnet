namespace JSLintNet.VisualStudio.Extensions.Documents
{
    using EnvDTE;

    internal class DocumentAccessor : IDocumentAccessor
    {
        private Document document;

        public DocumentAccessor(Document document)
        {
            this.document = document;
        }

        public TextDocument TextDocument
        {
            get
            {
                return (TextDocument)this.document.Object("TextDocument");
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
    }
}
