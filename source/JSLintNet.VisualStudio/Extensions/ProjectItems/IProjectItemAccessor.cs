namespace JSLintNet.VisualStudio.Extensions.ProjectItems
{
    using System.Collections.Generic;

    internal interface IProjectItemAccessor
    {
        string FileName { get; }

        string RelativePath { get; }

        ProjectItemIgnoreState IgnoreState(IList<string> ignore);
    }
}
