namespace JSLintNet.VisualStudio.EventControllers
{
    using System;
    using EnvDTE;
    using JSLintNet.VisualStudio.Errors;
    using JSLintNet.VisualStudio.Properties;

    internal class BuildEventController : EventControllerBase
    {
        private BuildEvents buildEvents;

        private vsBuildAction currentBuildAction;

        public BuildEventController(IServiceProvider serviceProvider, IJSLintErrorListProvider errorListProvider)
            : this(serviceProvider, errorListProvider, new VisualStudioJSLintProvider(serviceProvider, errorListProvider))
        {
        }

        public BuildEventController(IServiceProvider serviceProvider, IJSLintErrorListProvider errorListProvider, IVisualStudioJSLintProvider visualStudioJSLintProvider)
            : base(serviceProvider, errorListProvider, visualStudioJSLintProvider)
        {
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

            var project = this.Environment.Solution.Projects.FindByUniqueName(projectName);

            if (project == null)
            {
                return;
            }

            var settings = this.VisualStudioJSLintProvider.LoadSettings(project);

            if (!settings.RunOnBuild.GetValueOrDefault())
            {
                return;
            }

            this.ErrorListProvider.ClearCustomErrors();

            var ignored = settings.NormalizeIgnore();
            var items = project.ProjectItems.FindLintable(ignored);
            var errors = this.VisualStudioJSLintProvider.LintProjectItems(items, settings);

            if (errors > 0 && settings.CancelBuild.GetValueOrDefault())
            {
                this.Environment.ExecuteCommand("Build.Cancel");
                this.ErrorListProvider.AddCustomError(Resources.BuildCanceled);
            }
        }
    }
}
