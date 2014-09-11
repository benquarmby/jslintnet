namespace EnvDTE
{
    using System;
    using JSLintNet.VisualStudio.Extensions.ProjectItems;

    internal static class ProjectItemExtensions
    {
        static ProjectItemExtensions()
        {
            GetterFactory = x => new ProjectItemAccessor(x);
            PredicatorFactory = x => new ProjectItemPredicator(x);
        }

        internal static Func<ProjectItem, IProjectItemAccessor> GetterFactory { get; set; }

        internal static Func<ProjectItem, IProjectItemPredicator> PredicatorFactory { get; set; }

        public static IProjectItemAccessor Access(this ProjectItem projectItem)
        {
            return GetterFactory(projectItem);
        }

        public static IProjectItemPredicator Is(this ProjectItem projectItem)
        {
            return PredicatorFactory(projectItem);
        }
    }
}
