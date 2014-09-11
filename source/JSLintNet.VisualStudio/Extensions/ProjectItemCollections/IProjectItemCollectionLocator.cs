namespace JSLintNet.VisualStudio.Extensions.ProjectItemCollections
{
    using System.Collections.Generic;
    using EnvDTE;

    internal interface IProjectItemCollectionLocator
    {
        ProjectItem Item(string itemName);

        IList<ProjectItem> Lintables(IList<string> ignore);
    }
}
