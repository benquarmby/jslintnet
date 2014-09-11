namespace JSLintNet.VisualStudio.Extensions.Environments
{
    using EnvDTE;

    internal interface IEnvironmentLocator
    {
        Project ProjectByUniqueName(string uniqueName);
    }
}
