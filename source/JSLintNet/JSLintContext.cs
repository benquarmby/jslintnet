namespace JSLintNet
{
    using System;
    using System.IO;
    using System.Text;
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
            return this.Lint(source, null);
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
            this.context.SetParameter(JSLintParameters.SourceParameter, source);
            this.context.SetParameter(JSLintParameters.OptionsParameter, this.jsonProvider.SerializeOptions(options));

            this.context.Run(string.Format(Resources.jslintRunFormat, JSLintParameters.SourceParameter, JSLintParameters.OptionsParameter));
            var rawData = this.context.Run(Resources.jslintData) as string;
            var data = this.jsonProvider.DeserializeData(rawData);

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
            var path = typeof(JSLintContext).Assembly.Location;
            path = Path.GetDirectoryName(path);
            path = Path.Combine(path, "jslint.js");

            if (!this.fileSystemWrapper.FileExists(path))
            {
                this.context.Run(Resources.jslint);

                return;
            }

            var source = this.fileSystemWrapper.ReadAllText(path, Encoding.UTF8);
            this.context.Run(source);
            var edition = this.context.Run(Resources.jslintEdition);

            if (AssemblyInfo.Edition.CompareTo(edition) > 0)
            {
                throw new Exception(string.Format(Resources.JSLintEditionErrorFormat, edition, AssemblyInfo.Edition));
            }
        }
    }
}
