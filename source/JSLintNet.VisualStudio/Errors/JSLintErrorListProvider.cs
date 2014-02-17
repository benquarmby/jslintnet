namespace JSLintNet.VisualStudio.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JSLintNet.Models;
    using JSLintNet.Settings;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TextManager.Interop;
    using EnvironmentConstants = EnvDTE.Constants;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    /// <summary>
    /// Provides error list services for the JSLint.NET Visual Studio package.
    /// </summary>
    internal class JSLintErrorListProvider : ErrorListProvider, IJSLintErrorListProvider
    {
        private IServiceProvider serviceProvider;

        private IVsTextManager textManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintErrorListProvider"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public JSLintErrorListProvider(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            this.textManager = this.serviceProvider.GetService<VsTextManagerClass, IVsTextManager>();
        }

        public event ErrorListChangeHandler ErrorListChange;

        /// <summary>
        /// Gets the error count.
        /// </summary>
        /// <value>
        /// The error count.
        /// </value>
        public int ErrorCount
        {
            get
            {
                return this.Tasks.Count;
            }
        }

        /// <summary>
        /// Gets the list of errors for the specified file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>
        /// The list of errors for the specified file.
        /// </returns>
        public IList<JSLintErrorTask> GetErrors(string fileName)
        {
            return this.Tasks
                .OfType<JSLintErrorTask>()
                .Where(x => x.Document == fileName)
                .ToList();
        }

        /// <summary>
        /// Adds the JSLint errors to the collection.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="jsLintErrors">The JSLint errors.</param>
        /// <param name="output">The output type.</param>
        /// <param name="hierarchy">The hierarchy.</param>
        public void AddJSLintErrors(string fileName, IEnumerable<IJSLintError> jsLintErrors, Output? output, IVsHierarchy hierarchy)
        {
            Action batch = () =>
            {
                TaskErrorCategory category;
                Enum.TryParse(output.ToString(), out category);

                foreach (var jsLintError in jsLintErrors)
                {
                    var error = new JSLintErrorTask(fileName, jsLintError, category, hierarchy);
                    error.Navigate += this.OnTaskNavigate;

                    this.Tasks.Add(error);
                }
            };

            this.BatchAction(ErrorListAction.AddFile, fileName, batch);
        }

        /// <summary>
        /// Adds a custom error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public void AddCustomError(string message, params object[] args)
        {
            Action batch = () =>
            {
                var task = new CustomErrorTask(message, args);

                this.Tasks.Add(task);
            };

            this.BatchAction(ErrorListAction.AddCustom, null, batch);
        }

        /// <summary>
        /// Clears errors for the specified file from the collection.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public void ClearJSLintErrors(string fileName)
        {
            Action batch = () =>
            {
                var tasks = this.GetErrors(fileName);

                foreach (var task in tasks)
                {
                    this.Tasks.Remove(task);
                }
            };

            this.BatchAction(ErrorListAction.ClearFile, fileName, batch);
        }

        /// <summary>
        /// Clears the custom errors.
        /// </summary>
        public void ClearCustomErrors()
        {
            Action batch = () =>
            {
                var tasks = this.Tasks
                    .OfType<CustomErrorTask>()
                    .ToArray();

                foreach (var task in tasks)
                {
                    this.Tasks.Remove(task);
                }
            };

            this.BatchAction(ErrorListAction.ClearCustom, null, batch);
        }

        /// <summary>
        /// Clears all errors from the collection.
        /// </summary>
        public void ClearAllErrors()
        {
            this.BatchAction(ErrorListAction.ClearAll, null, () => this.Tasks.Clear());
        }

        /// <summary>
        /// Disposes this provider.
        /// </summary>
        /// <param name="disposing">Parameter is set to true if the task list can be disposed, that is, if the <see cref="T:Microsoft.VisualStudio.Shell.TaskProvider.TaskCollection" /> contains tasks.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ClearAllErrors();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Tries to the get window frame for the given file.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="windowFrame">The window frame.</param>
        /// <returns>
        /// <c>true</c> if fetching the window frame was successful, otherwise <c>false</c>.
        /// </returns>
        private static bool TryGetWindowFrame(IServiceProvider serviceProvider, string fileName, out IVsWindowFrame windowFrame)
        {
            IVsUIShellOpenDocument openDocument;

            if (!serviceProvider.TryGetService(out openDocument))
            {
                windowFrame = null;
                return false;
            }

            IOleServiceProvider oleServiceProvider;
            IVsUIHierarchy hierarchy;
            uint itemid;
            var codeViewGuid = new Guid(EnvironmentConstants.vsViewKindCode);

            if (ErrorHandler.Failed(openDocument.OpenDocumentViaProject(
                fileName,
                ref codeViewGuid,
                out oleServiceProvider,
                out hierarchy,
                out itemid,
                out windowFrame)))
            {
                return false;
            }

            return windowFrame != null;
        }

        /// <summary>
        /// Tries to get the text lines from a window.
        /// </summary>
        /// <param name="windowFrame">The window frame.</param>
        /// <param name="textLines">The text lines.</param>
        /// <returns>
        /// <c>true</c> if fetching the text lines was successful, otherwise <c>false</c>.
        /// </returns>
        private static bool TryGetTextLines(IVsWindowFrame windowFrame, out IVsTextLines textLines)
        {
            object docData;
            windowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocData, out docData);
            textLines = docData as IVsTextLines;

            if (textLines != null)
            {
                return true;
            }

            var bufferProvider = docData as IVsTextBufferProvider;

            if (bufferProvider != null)
            {
                if (!ErrorHandler.Failed(bufferProvider.GetTextBuffer(out textLines)))
                {
                    return textLines != null;
                }
            }

            return false;
        }

        /// <summary>
        /// Wrapper that suspends error list refreshes for the duration of the batch function.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="batch">The batch.</param>
        private void BatchAction(ErrorListAction action, string fileName, Action batch)
        {
            this.SuspendRefresh();

            try
            {
                batch();

                var handler = this.ErrorListChange;

                if (handler != null)
                {
                    var e = new ErrorListChangeEventArgs(action, fileName);

                    handler(this, e);
                }
            }
            finally
            {
                this.ResumeRefresh();
            }
        }

        /// <summary>
        /// Called when a task is navigated to.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnTaskNavigate(object sender, EventArgs e)
        {
            var task = (JSLintErrorTask)sender;

            IVsWindowFrame windowFrame;
            if (!TryGetWindowFrame(this.serviceProvider, task.Document, out windowFrame))
            {
                return;
            }

            IVsTextLines textLines;
            if (!TryGetTextLines(windowFrame, out textLines))
            {
                return;
            }

            var codeViewGuid = new Guid(EnvironmentConstants.vsViewKindCode);

            this.textManager.NavigateToLineAndColumn(
                textLines,
                ref codeViewGuid,
                task.Line,
                task.Column,
                task.Line,
                task.Column);
        }
    }
}
