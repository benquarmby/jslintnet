namespace JSLintNet.VisualStudio.Extensions.ProjectItemCollections
{
    using System.Collections.Generic;
    using System.Linq;
    using EnvDTE;
    using ProjectItemCollection = EnvDTE.ProjectItems;

    internal class ProjectItemCollectionLocator : IProjectItemCollectionLocator
    {
        private ProjectItemCollection projectItems;

        public ProjectItemCollectionLocator(ProjectItemCollection projectItems)
        {
            this.projectItems = projectItems;
        }

        public ProjectItem Item(string itemName)
        {
            return this.projectItems
                .OfType<ProjectItem>()
                .FirstOrDefault(x => x.Name == itemName);
        }

        public IList<ProjectItem> Lintables(IList<string> ignore)
        {
            var projectItemList = new List<ProjectItem>();

            foreach (ProjectItem projectItem in this.projectItems)
            {
                AddLintable(projectItem, projectItemList, ignore);
            }

            return projectItemList;
        }

        private static void AddLintable(ProjectItem projectItem, List<ProjectItem> projectItemList, IList<string> ignore)
        {
            var fileName = projectItem.GetFileName();

            if (!projectItem.IsFolder())
            {
                if (JSLint.CanLint(fileName) && !projectItem.IsIgnored(ignore))
                {
                    projectItemList.Add(projectItem);
                }

                return;
            }

            foreach (ProjectItem subItem in projectItem.ProjectItems)
            {
                AddLintable(subItem, projectItemList, ignore);
            }
        }
    }
}
