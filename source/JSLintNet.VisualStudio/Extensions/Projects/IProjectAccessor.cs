namespace JSLintNet.VisualStudio.Extensions.Projects
{
    internal interface IProjectAccessor
    {
        /// <summary>
        /// Gets the directory of the project.
        /// </summary>
        /// <value>
        /// The directory.
        /// </value>
        string Directory { get; }
    }
}
