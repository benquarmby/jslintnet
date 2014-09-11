namespace JSLintNet.VisualStudio.Extensions.ProjectItems
{
    using System.Collections.Generic;

    internal interface IProjectItemPredicator
    {
        bool Folder { get; }

        bool InSolutionFolder { get; }

        bool Link { get; }

        bool Ignored(IList<string> ignore);
    }
}
