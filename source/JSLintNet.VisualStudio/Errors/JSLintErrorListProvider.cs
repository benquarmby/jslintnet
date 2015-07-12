namespace JSLintNet.VisualStudio.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EnvDTE;
    using EnvDTE80;
    using JSLintNet.Properties;
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

        private DTE2 environment;

        private IVsTextManager textManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintErrorListProvider"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public JSLintErrorListProvider(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            this.environment = this.serviceProvider.GetService<DTE, DTE2>();
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
        /// Gets the list of errors for the specified files.
        /// </summary>
        /// <param name="fileNames">The file names.</param>
        /// <returns>
        /// The list of errors for the specified files.
        /// </returns>
        public IList<JSLintErrorTask> GetErrors(params string[] fileNames)
        {
            return this.Tasks
                .OfType<JSLintErrorTask>()
                .Where(x => fileNames.Contains(x.Document, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Gets the list of errors for the specified project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>
        /// The list of errors for the specified project.
        /// </returns>
        public IList<JSLintErrorTask> GetErrors(Project project)
        {
            return this.Tasks
                .OfType<JSLintErrorTask>()
                .Where(x => x.MatchesProject(project))
                .ToList();
        }

        /// <summary>
        /// Gets the list of custom errors.
        /// </summary>
        /// <returns>
        /// The list of custom errors.
        /// </returns>
        public IList<CustomErrorTask> GetCustomErrors()
        {
            return this.Tasks
                .OfType<CustomErrorTask>()
                .ToList();
        }

        /// <summary>
        /// Adds the JSLint errors to the collection.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="jsLintErrors">The JSLint errors.</param>
        /// <param name="output">The output type.</param>
        /// <param name="hierarchy">The hierarchy.</param>
        public void AddJSLintErrors(string fileName, IEnumerable<IJSLintWarning> jsLintErrors, Output? output, IVsHierarchy hierarchy)
        {
            Action batch = () =>
            {
                var existing = this.GetExistingErrors(fileName);
                TaskErrorCategory category;
                Enum.TryParse(output.ToString(), out category);

                foreach (var jsLintError in jsLintErrors)
                {
                    if (!existing.Any(x => ErrorsEqual(x, jsLintError)))
                    {
                        var error = new JSLintErrorTask(fileName, jsLintError, category, hierarchy);
                        error.Navigate += this.OnTaskNavigate;

                        this.Tasks.Add(error);
                    }
                }
            };

            this.BatchAction(ErrorListAction.AddFile, new string[] { fileName }, batch);
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
        /// Clears errors for the specified files from the collection.
        /// </summary>
        /// <param name="fileNames">The file names.</param>
        public void ClearJSLintErrors(params string[] fileNames)
        {
            if (fileNames == null || fileNames.Length == 0)
            {
                return;
            }

            var tasks = this.GetErrors(fileNames);

            if (tasks.Count == 0)
            {
                return;
            }

            Action batch = () =>
            {
                foreach (var task in tasks)
                {
                    this.Tasks.Remove(task);
                }
            };

            this.BatchAction(ErrorListAction.ClearFile, fileNames, batch);
        }

        /// <summary>
        /// Clears errors for the specified project from the collection.
        /// </summary>
        /// <param name="project">The project.</param>
        public void ClearJSLintErrors(Project project)
        {
            var tasks = this.GetErrors(project);

            if (tasks.Count == 0)
            {
                return;
            }

            var fileNames = tasks.Select(x => x.Document).ToArray();

            Action batch = () =>
            {
                foreach (var task in tasks)
                {
                    this.Tasks.Remove(task);
                }
            };

            this.BatchAction(ErrorListAction.ClearFile, fileNames, batch);
        }

        /// <summary>
        /// Clears the custom errors.
        /// </summary>
        public void ClearCustomErrors()
        {
            var tasks = this.GetCustomErrors();

            if (tasks.Count == 0)
            {
                return;
            }

            Action batch = () =>
            {
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
            var viewKind = new Guid(EnvironmentConstants.vsViewKindTextView);

            if (ErrorHandler.Failed(openDocument.OpenDocumentViaProject(
                fileName,
                ref viewKind,
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
        /// Determines whether the errors are equal.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="error">The error.</param>
        /// <returns>True if the errors are equal; otherwise false.</returns>
        private static bool ErrorsEqual(ErrorItem item, IJSLintWarning error)
        {
            var description = string.Concat(Resources.ErrorTextPrefix, error.Message);

            return item.Description.Equals(description, StringComparison.OrdinalIgnoreCase) &&
                item.Line == error.Line &&
                item.Column == error.Column;
        }

        /// <summary>
        /// Wrapper that suspends error list refreshes for the duration of the batch function.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="fileNames">Names of the files.</param>
        /// <param name="batch">The batch.</param>
        private void BatchAction(ErrorListAction action, IEnumerable<string> fileNames, Action batch)
        {
            this.SuspendRefresh();

            try
            {
                batch();

                var handler = this.ErrorListChange;

                if (handler != null)
                {
                    var e = new ErrorListChangeEventArgs(action, fileNames);

                    handler(this, e);
                }
            }
            finally
            {
                this.ResumeRefresh();
            }
        }

        /// <summary>
        /// Gets any existing JSLint errors for the specified file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The list of existing JSLint errors.</returns>
        private IList<ErrorItem> GetExistingErrors(string fileName)
        {
            var list = new List<ErrorItem>();
            var errorItems = this.environment.ToolWindows.ErrorList.ErrorItems;

            if (errorItems.Count > 0)
            {
                for (int i = 1; i <= errorItems.Count; i += 1)
                {
                    var item = errorItems.Item(i);

                    if (item.Description.StartsWith(Resources.ErrorTextPrefix) && fileName.Equals(item.FileName, StringComparison.OrdinalIgnoreCase))
                    {
                        list.Add(errorItems.Item(i));
                    }
                }
            }

            return list;
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

            var viewKind = new Guid(EnvironmentConstants.vsViewKindTextView);

            this.textManager.NavigateToLineAndColumn(
                textLines,
                ref viewKind,
                task.Line,
                task.Column,
                task.Line,
                task.Column);
        }
    }
}
