namespace JSLintNet.VisualStudio.Extensions.Documents
{
    using EnvDTE;

    internal interface IDocumentAccessor
    {
        TextDocument TextDocument { get; }

        string Source { get; }

        string LineEnding { get; }
    }
}
