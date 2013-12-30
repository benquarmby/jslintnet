namespace JSLintNet.VisualStudio
{
    using System;
    using System.ComponentModel.Composition;
    using System.Runtime.InteropServices;
    using JSLintNet.VisualStudio.Errors;
    using JSLintNet.VisualStudio.EventControllers;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// The JSLint.NET Visual Studio package.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", AssemblyInfo.Version, IconResourceID = 400)]
    [Guid(JSLintCommands.PackageString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class JSLintPackage : Package
    {
        private IEventController menuEventController;

        private IEventController documentEventController;

        private IEventController buildEventController;

        [Export]
        internal static IJSLintErrorListProvider CurrentErrorListProvider { get; private set; }

        /// <summary>
        /// Called when the VSPackage is loaded by Visual Studio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var errorListProvider = new JSLintErrorListProvider(this);

            CurrentErrorListProvider = errorListProvider;

            this.menuEventController = new MenuEventController(this, errorListProvider);
            this.documentEventController = new DocumentEventController(this, errorListProvider);
            this.buildEventController = new BuildEventController(this, errorListProvider);

            this.menuEventController.Initialize();
            this.documentEventController.Initialize();
            this.buildEventController.Initialize();
        }
    }
}
