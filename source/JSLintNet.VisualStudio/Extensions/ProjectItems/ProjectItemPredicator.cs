namespace JSLintNet.VisualStudio.Extensions.ProjectItems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EnvDTE;
    using EnvDTE80;

    internal class ProjectItemPredicator : IProjectItemPredicator
    {
        private const string FolderKind = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";

        private ProjectItem projectItem;

        public ProjectItemPredicator(ProjectItem projectItem)
        {
            this.projectItem = projectItem;
        }

        public bool Folder
        {
            get
            {
                return FolderKind.Equals(this.projectItem.Kind, StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool InSolutionFolder
        {
            get
            {
                return ProjectKinds.vsProjectKindSolutionFolder.Equals(this.projectItem.ContainingProject.Kind, StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool Link
        {
            get
            {
                if (this.Folder)
                {
                    return false;
                }

                return this.projectItem.Properties.Get<bool>("IsLink");
            }
        }

        public bool Ignored(IList<string> ignore)
        {
            if (ignore != null && !this.InSolutionFolder)
            {
                var relative = this.projectItem.Access().RelativePath;

                return ignore.Any(x => relative.StartsWith(x, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }
    }
}
