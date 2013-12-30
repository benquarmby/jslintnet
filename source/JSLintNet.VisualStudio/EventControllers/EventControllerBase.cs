namespace JSLintNet.VisualStudio.EventControllers
{
    using System;
    using EnvDTE;
    using EnvDTE80;
    using JSLintNet.VisualStudio.Errors;

    /// <summary>
    /// Controls package events.
    /// </summary>
    internal abstract class EventControllerBase : IEventController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventControllerBase"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="errorListProvider">The error list provider.</param>
        /// <param name="visualStudioJSLintProvider">The Visual Studio JSLint provider.</param>
        public EventControllerBase(IServiceProvider serviceProvider, IJSLintErrorListProvider errorListProvider, IVisualStudioJSLintProvider visualStudioJSLintProvider)
        {
            this.ServiceProvider = serviceProvider;
            this.ErrorListProvider = errorListProvider;
            this.VisualStudioJSLintProvider = visualStudioJSLintProvider;

            this.Environment = this.ServiceProvider.GetService<DTE, DTE2>();
        }

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        /// <value>
        /// The service provider.
        /// </value>
        protected IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Gets the error list provider.
        /// </summary>
        /// <value>
        /// The error list provider.
        /// </value>
        protected IJSLintErrorListProvider ErrorListProvider { get; private set; }

        /// <summary>
        /// Gets the environment.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        protected DTE2 Environment { get; private set; }

        /// <summary>
        /// Gets the Visual Studio JSLint provider.
        /// </summary>
        /// <value>
        /// The Visual Studio JSLint provider.
        /// </value>
        protected IVisualStudioJSLintProvider VisualStudioJSLintProvider { get; private set; }

        /// <summary>
        /// Initializes this instance and binds related events.
        /// </summary>
        public abstract void Initialize();
    }
}
