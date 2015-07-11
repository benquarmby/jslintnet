namespace JSLintNet.Abstractions
{
    using Microsoft.Build.Evaluation;

    internal interface IBuildProjectManager
    {
        Project GetProject(string fullPath);

        bool TryGetProject(string fullPath, out Project project);
    }
}
