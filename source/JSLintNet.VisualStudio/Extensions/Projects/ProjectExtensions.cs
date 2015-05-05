namespace EnvDTE
{
    using System;
    using JSLintNet.VisualStudio.Extensions.Projects;

    internal static class ProjectExtensions
    {
        static ProjectExtensions()
        {
            AccessorFactory = x => new ProjectAccessor(x);
        }

        internal static Func<Project, IProjectAccessor> AccessorFactory { get; set; }

        public static IProjectAccessor Access(this Project project)
        {
            return AccessorFactory(project);
        }
    }
}
