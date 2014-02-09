namespace JSLintNet.VisualStudio.EventControllers
{
    using System;
    using EnvDTE;
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
        }

        private void OnDocumentSaved(Document document)
        {
            if (JSLint.CanLint(document.Name))
            {
                var settings = this.VisualStudioJSLintProvider.LoadSettings(document.ProjectItem.ContainingProject);
                var ignored = settings.NormalizeIgnore();

                if (settings.RunOnSave.GetValueOrDefault() && !document.ProjectItem.IsIgnored(ignored))
                {
                    this.VisualStudioJSLintProvider.LintDocument(document, settings);
                }
            }
        }
    }
}
