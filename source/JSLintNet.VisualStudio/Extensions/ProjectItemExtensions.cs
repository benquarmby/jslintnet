namespace EnvDTE
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal enum IgnoreState
    {
        None,
        Implicit,
        Explicit
    }

    internal static class ProjectItemExtensions
    {
        public static bool IsIgnored(this ProjectItem projectItem, IList<string> ignore)
        {
            if (ignore != null)
            {
                var relative = projectItem.GetRelativePath();

                return ignore.Any(x => relative.StartsWith(x, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        public static IgnoreState GetIgnoreState(this ProjectItem projectItem, IList<string> ignore)
        {
            if (ignore != null)
            {
                var relative = projectItem.GetRelativePath();

                if (ignore.Any(x => relative.Equals(x, StringComparison.OrdinalIgnoreCase)))
                {
                    return IgnoreState.Explicit;
                }

                if (ignore.Any(x => relative.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
                {
                    return IgnoreState.Implicit;
                }
            }

            return IgnoreState.None;
        }

        public static string GetRelativePath(this ProjectItem projectItem)
        {
            if (!projectItem.IsLink())
            {
                var projectPath = projectItem.ContainingProject.Properties.Get<string>("FullPath");
                projectPath = Path.GetDirectoryName(projectPath);
                var fileName = projectItem.FileNames[0];

                return fileName.Substring(projectPath.Length);
            }

            return GetVirtualPath(projectItem);
        }

        public static bool IsLink(this ProjectItem projectItem)
        {
            if (projectItem.Kind == ProjectItemsExtensions.FolderKind)
            {
                return false;
            }

            return projectItem.Properties.Get<bool>("IsLink");
        }

        private static string GetVirtualPath(ProjectItem projectItem)
        {
            var parent = projectItem.Collection.Parent as ProjectItem;
            var path = projectItem.Name;

            while (parent != null)
            {
                path = Path.Combine(parent.Name, path);
                parent = parent.Collection.Parent as ProjectItem;
            }

            return @"\" + path;
        }
    }
}
