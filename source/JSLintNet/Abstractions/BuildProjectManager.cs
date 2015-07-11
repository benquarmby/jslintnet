namespace JSLintNet.Abstractions
{
    using System.Linq;
    using Microsoft.Build.Evaluation;

    internal class BuildProjectManager : IBuildProjectManager
    {
        public Project GetProject(string fullPath)
        {
            return ProjectCollection.GlobalProjectCollection
                .GetLoadedProjects(fullPath)
                .FirstOrDefault();
        }

        public bool TryGetProject(string fullPath, out Project project)
        {
            project = this.GetProject(fullPath);

            return project != null;
        }
    }
}
