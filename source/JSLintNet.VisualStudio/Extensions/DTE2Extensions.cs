namespace EnvDTE80
{
    using System.Collections.Generic;
    using System.Linq;
    using EnvDTE;
    using JSLintNet;

    internal static class DTE2Extensions
    {
        public static bool AllSelectedAreLintable(this DTE2 environment)
        {
            var selectedItems = environment.GetSelectedItems();

            return selectedItems.Length > 0 && selectedItems.All(x => JSLint.CanLint(x.Name));
        }

        public static IList<ProjectItem> FindSelectedLintables(this DTE2 environment)
        {
            var selectedItems = environment.GetSelectedItems();
            var projectItems = new List<ProjectItem>();

            foreach (var selectedItem in selectedItems)
            {
                if (JSLint.CanLint(selectedItem.Name))
                {
                    var projectItem = selectedItem.Object as ProjectItem;

                    if (projectItem != null)
                    {
                        projectItems.Add(projectItem);
                    }
                }
            }

            return projectItems;
        }

        public static UIHierarchyItem[] GetSelectedItems(this DTE2 environment)
        {
            var hierarchy = environment.ToolWindows.GetToolWindow(Constants.vsWindowKindSolutionExplorer) as UIHierarchy;

            return hierarchy.SelectedItems as UIHierarchyItem[];
        }

        public static Project GetSelectedProject(this DTE2 environment)
        {
            var items = environment.GetSelectedItems();

            return items[0].Object as Project;
        }
    }
}
