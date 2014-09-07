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
        public const string FolderKind = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";

        public const string SolutionFolderKind = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";

        public static bool IsFolder(this ProjectItem projectItem)
        {
            return FolderKind.Equals(projectItem.Kind, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsInSolutionFolder(this ProjectItem projectItem)
        {
            return SolutionFolderKind.Equals(projectItem.ContainingProject.Kind, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsIgnored(this ProjectItem projectItem, IList<string> ignore)
        {
            if (ignore != null && !projectItem.IsInSolutionFolder())
            {
                var relative = projectItem.GetRelativePath();

                return ignore.Any(x => relative.StartsWith(x, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        public static IgnoreState GetIgnoreState(this ProjectItem projectItem, IList<string> ignore)
        {
            if (ignore != null && !projectItem.IsInSolutionFolder())
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
                var fileName = projectItem.GetFileName();

                return fileName.Substring(projectPath.Length);
            }

            return GetVirtualPath(projectItem);
        }

        public static bool IsLink(this ProjectItem projectItem)
        {
            if (projectItem.IsFolder())
            {
                return false;
            }

            return projectItem.Properties.Get<bool>("IsLink");
        }

        public static string GetFileName(this ProjectItem projectItem)
        {
            if (projectItem.IsInSolutionFolder())
            {
                return projectItem.FileNames[1];
            }

            return projectItem.FileNames[0];
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
