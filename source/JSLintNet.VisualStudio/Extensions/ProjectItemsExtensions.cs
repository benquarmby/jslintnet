namespace EnvDTE
{
    using System.Collections.Generic;
    using JSLintNet;

    internal static class ProjectItemsExtensions
    {
        public static bool ItemExists(this ProjectItems projectItems, string itemName)
        {
            return projectItems.FindItem(itemName) != null;
        }

        public static bool TryFindItem(this ProjectItems projectItems, string itemName, out ProjectItem item)
        {
            item = projectItems.FindItem(itemName);

            return item != null;
        }

        public static ProjectItem FindItem(this ProjectItems projectItems, string itemName)
        {
            foreach (ProjectItem item in projectItems)
            {
                if (item.Name == itemName)
                {
                    return item;
                }
            }

            return null;
        }

        public static IList<ProjectItem> FindLintable(this ProjectItems projectItems)
        {
            return FindLintable(projectItems, null);
        }

        public static IList<ProjectItem> FindLintable(this ProjectItems projectItems, IList<string> ignore)
        {
            var projectItemList = new List<ProjectItem>();

            foreach (ProjectItem projectItem in projectItems)
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
