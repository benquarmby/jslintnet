namespace JSLintNet.VisualStudio.Extensions.Projects
{
    using System.IO;
    using EnvDTE;

    internal class ProjectAccessor : IProjectAccessor
    {
        private readonly Project project;

        public ProjectAccessor(Project project)
        {
            this.project = project;
        }

        public string Directory
        {
            get
            {
                // Most web project types have a FullPath
                var path = this.project.Properties.Get<string>("FullPath");

                if (!string.IsNullOrEmpty(path))
                {
                    if (path.EndsWith(Path.DirectorySeparatorChar.ToString()) || path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                    {
                        // Remove any trailing separators
                        path = Path.GetDirectoryName(path);
                    }

                    return path;
                }

                // All other project types have a FullName
                var fullName = this.project.FullName;

                if (string.IsNullOrEmpty(fullName))
                {
                    // If neither the FullPath nor FullName exist, then this is
                    // most likely a "Misc Files" project with no usable path
                    return null;
                }

                // Other project types can use the project file's root directory
                return Path.GetDirectoryName(fullName);
            }
        }
    }
}
