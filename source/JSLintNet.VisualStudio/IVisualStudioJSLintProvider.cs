namespace JSLintNet.VisualStudio
{
    using System.Collections.Generic;
    using EnvDTE;

    /// <summary>
    /// Provides JSLint services specific to a Visual Studio environment.
    /// </summary>
    internal interface IVisualStudioJSLintProvider
    {
        /// <summary>
        /// Validates the specified document using JSLint.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>
        /// The total number of JSLint errors found.
        /// </returns>
        int LintDocument(Document document);

        /// <summary>
        /// Validates the specified document using JSLint.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// The total number of JSLint errors found.
        /// </returns>
        int LintDocument(Document document, JSLintNetSettings settings);

        /// <summary>
        /// Validates the specified project items using JSLint.
        /// </summary>
        /// <param name="projectItems">The project items.</param>
        /// <returns>
        /// The total number of JSLint errors found.
        /// </returns>
        int LintProjectItems(IList<ProjectItem> projectItems);

        /// <summary>
        /// Validates the specified project items using JSLint.
        /// </summary>
        /// <param name="projectItems">The project items.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// The total number of JSLint errors found.
        /// </returns>
        int LintProjectItems(IList<ProjectItem> projectItems, JSLintNetSettings settings);

        /// <summary>
        /// Loads the settings for the specified project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>
        /// Settings for the specified project, or null if none could be found.
        /// </returns>
        JSLintNetSettings LoadSettings(Project project);

        /// <summary>
        /// Saves the settings for the specified project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="settings">The settings.</param>
        void SaveSettings(Project project, JSLintNetSettings settings);
    }
}
