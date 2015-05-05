namespace JSLintNet.VisualStudio.Errors
{
    using EnvDTE;
    using JSLintNet.Models;
    using JSLintNet.Properties;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// A task that handles JSLint errors.
    /// </summary>
    internal class JSLintErrorTask : ErrorTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintErrorTask" /> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="jsLintWarning">The JSLint error.</param>
        /// <param name="category">The category.</param>
        /// <param name="hierarchy">The hierarchy.</param>
        public JSLintErrorTask(string document, IJSLintWarning jsLintWarning, TaskErrorCategory category, IVsHierarchy hierarchy)
        {
            this.Document = document;
            this.Category = TaskCategory.BuildCompile;
            this.ErrorCategory = category;
            this.Line = jsLintWarning.Line;
            this.Column = jsLintWarning.Column;
            this.Text = GetText(jsLintWarning);
            this.HierarchyItem = hierarchy;
        }

        /// <summary>
        /// Determines whether the task matches the specified project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>
        /// <c>true</c> if the task matches the specified project, otherwise <c>false</c>.
        /// </returns>
        public bool MatchesProject(Project project)
        {
            if (this.HierarchyItem != null)
            {
                object raw;

                this.HierarchyItem.GetProperty(
                    (uint)VSConstants.VSITEMID.Root,
                    (int)__VSHPROPID.VSHPROPID_ExtObject,
                    out raw);

                return raw == project;
            }

            return false;
        }

        /// <summary>
        /// Gets a text representation of the specified error.
        /// </summary>
        /// <param name="warning">The error.</param>
        /// <returns>
        /// A text representation of the error.
        /// </returns>
        private static string GetText(IJSLintWarning warning)
        {
            return string.Concat(Resources.ErrorTextPrefix, warning.Message);
        }
    }
}
