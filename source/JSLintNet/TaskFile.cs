namespace JSLintNet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Build.Framework;

    internal class TaskFile
    {
        public TaskFile(string sourceDirectory, string file)
        {
            this.Virtual = GetVirtualPath(sourceDirectory, file);
            this.Absolute = GetAbsolutePath(sourceDirectory, file);
        }

        public TaskFile(string sourceDirectory, ITaskItem taskItem)
        {
            this.Virtual = GetVirtualPath(sourceDirectory, taskItem);
            this.Absolute = GetAbsolutePath(sourceDirectory, taskItem.ItemSpec);
        }

        public string Absolute { get; set; }

        public string Virtual { get; set; }

        public bool IsIgnored(IList<string> ignored)
        {
            return ignored.Any(x => this.Virtual.StartsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        private static string GetAbsolutePath(string sourceDirectory, string file)
        {
            if (Path.IsPathRooted(file))
            {
                return file;
            }

            return Path.Combine(sourceDirectory, file);
        }

        private static string GetVirtualPath(string sourceDirectory, ITaskItem taskItem)
        {
            var file = taskItem.GetMetadata("Link");
            if (string.IsNullOrEmpty(file))
            {
                file = taskItem.ItemSpec;
            }

            return GetVirtualPath(sourceDirectory, file);
        }

        private static string GetVirtualPath(string sourceDirectory, string file)
        {
            if (Path.IsPathRooted(file))
            {
                file = file.Substring(sourceDirectory.Length);
            }

            file = file.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            var directorySeparator = Path.DirectorySeparatorChar.ToString();
            if (!file.StartsWith(directorySeparator))
            {
                file = directorySeparator + file;
            }

            return file;
        }
    }
}
