namespace EnvDTE
{
    using System;
    using JSLintNet.VisualStudio.Extensions.ProjectItemCollections;
    using ProjectItemCollection = EnvDTE.ProjectItems;

    internal static class ProjectItemCollectionExtensions
    {
        static ProjectItemCollectionExtensions()
        {
            LocatorFactory = x => new ProjectItemCollectionLocator(x);
        }

        internal static Func<ProjectItemCollection, IProjectItemCollectionLocator> LocatorFactory { get; set; }

        public static IProjectItemCollectionLocator Locate(this ProjectItemCollection collection)
        {
            return LocatorFactory(collection);
        }
    }
}
