namespace JSLintNet.VisualStudio.Extensions.Environments
{
    using System;
    using EnvDTE;
    using EnvDTE80;
    using Environment = EnvDTE80.DTE2;

    internal class EnvironmentLocator : IEnvironmentLocator
    {
        private Environment environment;

        public EnvironmentLocator(Environment environment)
        {
            this.environment = environment;
        }

        public Project ProjectByUniqueName(string uniqueName)
        {
            var projects = this.environment.Solution.Projects;

            foreach (Project project in projects)
            {
                Project match;

                if (TryFindByUniqueName(project, uniqueName, out match))
                {
                    return match;
                }
            }

            return null;
        }

        private static bool TryFindByUniqueName(Project project, string uniqueName, out Project match)
        {
            if (project.Kind != ProjectKinds.vsProjectKindSolutionFolder && project.UniqueName.Equals(uniqueName, StringComparison.OrdinalIgnoreCase))
            {
                match = project;
                return true;
            }

            foreach (ProjectItem item in project.ProjectItems)
            {
                if (item.SubProject != null && TryFindByUniqueName(item.SubProject, uniqueName, out match))
                {
                    return true;
                }
            }

            match = null;
            return false;
        }
    }
}
