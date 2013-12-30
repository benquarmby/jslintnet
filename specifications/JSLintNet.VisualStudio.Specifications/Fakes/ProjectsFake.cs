namespace JSLintNet.VisualStudio.Specifications.Fakes
{
    using EnvDTE;

    public class ProjectsFake : EnumerableFake<Project>, Projects
    {
        public string Kind { get; set; }

        public DTE Parent { get; set; }

        public Properties Properties { get; set; }
    }
}
