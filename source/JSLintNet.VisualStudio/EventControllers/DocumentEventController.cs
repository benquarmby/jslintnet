namespace JSLintNet.VisualStudio.EventControllers
{
    using System;
    using EnvDTE;
    using JSLintNet.Settings;
    using JSLintNet.VisualStudio.Errors;

    internal class DocumentEventController : EventControllerBase
    {
        private DocumentEvents documentEvents;

        public DocumentEventController(IServiceProvider serviceProvider, IJSLintErrorListProvider errorListProvider)
            : this(serviceProvider, errorListProvider, new VisualStudioJSLintProvider(serviceProvider, errorListProvider))
        {
        }

        public DocumentEventController(IServiceProvider serviceProvider, IJSLintErrorListProvider errorListProvider, IVisualStudioJSLintProvider visualStudioJSLintProvider)
            : base(serviceProvider, errorListProvider, visualStudioJSLintProvider)
        {
            this.documentEvents = this.Environment.Events.DocumentEvents;
        }

        public override void Initialize()
        {
            this.documentEvents.DocumentSaved += this.OnDocumentSaved;
            this.documentEvents.DocumentOpened += this.OnDocumentOpened;
        }

        private void OnDocumentSaved(Document document)
        {
            this.LintDocument(document, x => x.RunOnSave);
        }

        private void OnDocumentOpened(Document document)
        {
            this.LintDocument(document, x => x.RunOnOpen);
        }

        private void LintDocument(Document document, Func<JSLintNetSettings, bool?> setting)
        {
            if (JSLint.CanLint(document.Name))
            {
                var settings = this.VisualStudioJSLintProvider.LoadSettings(document.ProjectItem.ContainingProject);
                var ignored = settings.NormalizeIgnore();

                if (setting(settings).GetValueOrDefault() && !document.ProjectItem.Is().Ignored(ignored))
                {
                    this.VisualStudioJSLintProvider.LintDocument(document, settings);
                }
            }
        }
    }
}
