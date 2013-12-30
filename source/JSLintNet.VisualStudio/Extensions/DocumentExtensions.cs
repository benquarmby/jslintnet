namespace EnvDTE
{
    internal static class DocumentExtensions
    {
        public static string GetSource(this Document document)
        {
            var textDocument = (TextDocument)document.Object("TextDocument");
            var editPoint = textDocument.StartPoint.CreateEditPoint();
            var source = editPoint.GetText(textDocument.EndPoint);

            return source;
        }
    }
}
