namespace JSLintNet
{
    using System;
    using System.Collections.Generic;
    using JSLintNet.Abstractions;
    using JSLintNet.Json;
    using JSLintNet.Models;
    using JSLintNet.Properties;

    /// <summary>
    /// Provides a context for running JSLint.
    /// </summary>
    public class JSLintContext : IJSLintContext
    {
        private IJavaScriptContext context;

        private IJsonProvider jsonProvider;

        private IFileSystemWrapper fileSystemWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintContext"/> class.
        /// </summary>
        public JSLintContext()
            : this(new AbstractionFactory(), new JsonProvider(), new FileSystemWrapper())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintContext" /> class.
        /// </summary>
        /// <param name="abstractionFactory">The abstraction factory.</param>
        /// <param name="jsonProvider">The JSON provider.</param>
        /// <param name="fileSystemWrapper">The file system wrapper.</param>
        internal JSLintContext(IAbstractionFactory abstractionFactory, IJsonProvider jsonProvider, IFileSystemWrapper fileSystemWrapper)
        {
            this.context = abstractionFactory.CreateJavaScriptContext();
            this.jsonProvider = jsonProvider;
            this.fileSystemWrapper = fileSystemWrapper;

            this.LoadJSLint();
        }

        /// <summary>
        /// Validates the specified source using JSLint.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        /// A <see cref="IJSLintData" /> containing any validation errors.
        /// </returns>
        public IJSLintData Lint(string source)
        {
            return this.Lint(source, null, null);
        }

        /// <summary>
        /// Validates the specified source using JSLint with the provided options.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// A <see cref="IJSLintData" /> containing any validation errors.
        /// </returns>
        public IJSLintData Lint(string source, JSLintOptions options)
        {
            return this.Lint(source, options, null);
        }

        /// <summary>
        /// Validates the specified source using JSLint with the provided options and global variables.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        /// <param name="globalVariables">The global variables.</param>
        /// <returns>
        /// A <see cref="IJSLintData" /> instance containing any validation warnings.
        /// </returns>
        public IJSLintData Lint(string source, JSLintOptions options, IList<string> globalVariables)
        {
            var jsonOptions = this.jsonProvider.SerializeOptions(options);
            var jsonGlobalVariables = this.jsonProvider.SerializeObject(globalVariables);
            var jsonData = this.context.Script.jslintnet(source, jsonOptions, jsonGlobalVariables);
            var data = this.jsonProvider.DeserializeData(jsonData);

            return data;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.context != null)
                {
                    this.context.Dispose();
                }
            }
        }

        private void LoadJSLint()
        {
            this.context.Run(Resources.jslint);
            this.context.Run(Resources.report);
            this.context.Run(Resources.jslintnet);
        }
    }
}
