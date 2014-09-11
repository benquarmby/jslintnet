namespace JSLintNet.VisualStudio.Extensions.ProjectItems
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using EnvDTE;

    internal class ProjectItemAccessor : IProjectItemAccessor
    {
        private ProjectItem projectItem;

        public ProjectItemAccessor(ProjectItem projectItem)
        {
            this.projectItem = projectItem;
        }

        public string FileName
        {
            get
            {
                if (this.projectItem.Is().InSolutionFolder)
                {
                    return this.projectItem.FileNames[1];
                }

                return this.projectItem.FileNames[0];
            }
        }

        public string RelativePath
        {
            get
            {
                if (!this.projectItem.Is().Link)
                {
                    var projectPath = this.projectItem.ContainingProject.Properties.Get<string>("FullPath");
                    projectPath = Path.GetDirectoryName(projectPath);
                    var fileName = this.FileName;

                    return fileName.Substring(projectPath.Length);
                }

                return this.VirtualPath;
            }
        }

        private string VirtualPath
        {
            get
            {
                var parent = this.projectItem.Collection.Parent as ProjectItem;
                var path = this.projectItem.Name;

                while (parent != null)
                {
                    path = Path.Combine(parent.Name, path);
                    parent = parent.Collection.Parent as ProjectItem;
                }

                return @"\" + path;
            }
        }

        public ProjectItemIgnoreState IgnoreState(IList<string> ignore)
        {
            if (ignore != null && !this.projectItem.Is().InSolutionFolder)
            {
                var relative = this.RelativePath;

                if (ignore.Any(x => relative.Equals(x, StringComparison.OrdinalIgnoreCase)))
                {
                    return ProjectItemIgnoreState.Explicit;
                }

                if (ignore.Any(x => relative.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
                {
                    return ProjectItemIgnoreState.Implicit;
                }
            }

            return ProjectItemIgnoreState.None;
        }
    }
}
