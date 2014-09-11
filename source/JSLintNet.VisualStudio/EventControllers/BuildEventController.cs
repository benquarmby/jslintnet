namespace JSLintNet.VisualStudio.EventControllers
{
    using System;
    using System.Linq;
    using EnvDTE;
    using EnvDTE80;
    using JSLintNet.Abstractions;
    using JSLintNet.VisualStudio.Errors;
    using JSLintNet.VisualStudio.Properties;
    using BuildProject = Microsoft.Build.Evaluation.Project;

    internal class BuildEventController : EventControllerBase
    {
        private const string MSBuildImport = "JSLintNet.MSBuild.targets";

        private IBuildProjectManager buildProjectManager;

        private BuildEvents buildEvents;

        private vsBuildAction currentBuildAction;

        public BuildEventController(IServiceProvider serviceProvider, IJSLintErrorListProvider errorListProvider)
            : this(serviceProvider, errorListProvider, new BuildProjectManager(), new VisualStudioJSLintProvider(serviceProvider, errorListProvider))
        {
        }

        public BuildEventController(IServiceProvider serviceProvider, IJSLintErrorListProvider errorListProvider, IBuildProjectManager buildProjectManager, IVisualStudioJSLintProvider visualStudioJSLintProvider)
            : base(serviceProvider, errorListProvider, visualStudioJSLintProvider)
        {
            this.buildProjectManager = buildProjectManager;
            this.buildEvents = this.Environment.Events.BuildEvents;
        }

        public override void Initialize()
        {
            this.buildEvents.OnBuildBegin += this.OnBuildBegin;
            this.buildEvents.OnBuildProjConfigBegin += this.OnBuildProjectConfigBegin;
        }

        private void OnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            this.currentBuildAction = action;
        }

        private void OnBuildProjectConfigBegin(string projectName, string projectConfig, string platform, string solutionConfig)
        {
            if (this.currentBuildAction == vsBuildAction.vsBuildActionClean)
            {
                return;
            }

            var project = this.Environment.Locate().ProjectByUniqueName(projectName);

            if (project == null)
            {
                return;
            }

            var settings = this.VisualStudioJSLintProvider.LoadSettings(project);
            var runOnBuild = settings.RunOnBuild.GetValueOrDefault();
            var nuGetInstalled = this.NuGetInstalled(project.FullName);

            if (!(runOnBuild || nuGetInstalled))
            {
                return;
            }

            this.ErrorListProvider.ClearCustomErrors();

            if (nuGetInstalled)
            {
                this.ErrorListProvider.ClearJSLintErrors(project);
                return;
            }

            var ignored = settings.NormalizeIgnore();
            var items = project.ProjectItems.Locate().Lintables(ignored);
            var errors = this.VisualStudioJSLintProvider.LintProjectItems(items, settings);

            if (errors > 0 && settings.CancelBuild.GetValueOrDefault())
            {
                this.Environment.ExecuteCommand("Build.Cancel");
                this.ErrorListProvider.AddCustomError(Resources.BuildCanceled);
            }
        }

        private bool NuGetInstalled(string fullPath)
        {
            BuildProject project;

            if (this.buildProjectManager.TryGetProject(fullPath, out project))
            {
                if (project.Imports.Any(x => x.ImportingElement.Project.EndsWith(MSBuildImport)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
