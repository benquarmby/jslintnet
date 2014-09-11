namespace EnvDTE
{
    using System;
    using JSLintNet.VisualStudio.Extensions.Documents;

    internal static class DocumentExtensions
    {
        static DocumentExtensions()
        {
            AccessorFactory = x => new DocumentAccessor(x);
        }

        internal static Func<Document, IDocumentAccessor> AccessorFactory { get; set; }

        public static IDocumentAccessor Access(this Document document)
        {
            return AccessorFactory(document);
        }
    }
}
