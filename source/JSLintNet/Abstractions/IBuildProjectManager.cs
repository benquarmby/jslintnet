namespace JSLintNet.Abstractions
{
    using Microsoft.Build.Evaluation;

    internal interface IBuildProjectManager
    {
        void SetGlobalPropert(string name, string value);

        Project GetProject(string fullPath);

        bool TryGetProject(string fullPath, out Project project);
    }
}
