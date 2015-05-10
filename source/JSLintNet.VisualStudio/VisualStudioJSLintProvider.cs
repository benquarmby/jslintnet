namespace JSLintNet.VisualStudio
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using EnvDTE;
    using JSLintNet.Abstractions;
    using JSLintNet.Helpers;
    using JSLintNet.Settings;
    using JSLintNet.VisualStudio.Errors;
    using JSLintNet.VisualStudio.Properties;
    using Microsoft.VisualStudio.Shell.Interop;
    using CoreResources = JSLintNet.Properties.Resources;

    /// <summary>
    /// Provides JSLint services specific to a Visual Studio environment.
    /// </summary>
    internal class VisualStudioJSLintProvider : IVisualStudioJSLintProvider
    {
        private IServiceProvider serviceProvider;

        private IJSLintErrorListProvider errorListProvider;

        private IJSLintFactory jsLintFactory;

        private IFileSystemWrapper fileSystemWrapper;

        private ISettingsRepository settingsRepository;

        private ICacheProvider cacheProvider;

        private IVsStatusbar statusBar;

        private IVsSolution solutionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStudioJSLintProvider" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="errorListProvider">The error list provider.</param>
        public VisualStudioJSLintProvider(IServiceProvider serviceProvider, IJSLintErrorListProvider errorListProvider)
            : this(serviceProvider, errorListProvider, new JSLintFactory(), new FileSystemWrapper(), new SettingsRepository(), new CacheProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStudioJSLintProvider" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="errorListProvider">The error list provider.</param>
        /// <param name="jsLintFactory">The JSLint factory.</param>
        /// <param name="fileSystemWrapper">The file system wrapper.</param>
        /// <param name="settingsRepository">The settings repository.</param>
        /// <param name="cacheProvider">The cache provider.</param>
        public VisualStudioJSLintProvider(IServiceProvider serviceProvider, IJSLintErrorListProvider errorListProvider, IJSLintFactory jsLintFactory, IFileSystemWrapper fileSystemWrapper, ISettingsRepository settingsRepository, ICacheProvider cacheProvider)
        {
            this.serviceProvider = serviceProvider;
            this.errorListProvider = errorListProvider;
            this.jsLintFactory = jsLintFactory;
            this.fileSystemWrapper = fileSystemWrapper;
            this.settingsRepository = settingsRepository;
            this.cacheProvider = cacheProvider;

            this.statusBar = this.serviceProvider.GetService<SVsStatusbar, IVsStatusbar>();
            this.solutionService = this.serviceProvider.GetService<SVsSolution, IVsSolution>();
        }

        /// <summary>
        /// Validates the specified document using JSLint.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>
        /// The total number of JSLint errors found.
        /// </returns>
        public int LintDocument(Document document)
        {
            var settings = this.LoadSettings(document.ProjectItem.ContainingProject);

            return this.LintDocument(document, settings);
        }

        /// <summary>
        /// Validates the specified document using JSLint.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// The total number of JSLint errors found.
        /// </returns>
        public int LintDocument(Document document, JSLintNetSettings settings)
        {
            this.errorListProvider.ClearJSLintErrors(document.FullName);

            int errors;
            var source = document.Access().Source;
            var hierarchy = this.GetHierarchy(document.ProjectItem.ContainingProject);

            using (var jsLintContext = this.jsLintFactory.CreateContext())
            {
                var result = jsLintContext.Lint(source, settings.Options);

                this.errorListProvider.AddJSLintErrors(document.FullName, result.Errors, settings.Output, hierarchy);

                errors = result.Errors.Count;
                var text = GetMessageText(errors);
                this.SetStatusBar(text);
            }

            return errors;
        }

        /// <summary>
        /// Validates the specified project items using JSLint.
        /// </summary>
        /// <param name="projectItems">The project items.</param>
        /// <returns>
        /// The total number of JSLint errors found.
        /// </returns>
        public int LintProjectItems(IList<ProjectItem> projectItems)
        {
            if (projectItems.Count < 1)
            {
                return 0;
            }

            var settings = this.LoadSettings(projectItems[0].ContainingProject);

            return this.LintProjectItems(projectItems, settings);
        }

        /// <summary>
        /// Validates the specified project items using JSLint.
        /// </summary>
        /// <param name="projectItems">The project items.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// The total number of JSLint errors found.
        /// </returns>
        public int LintProjectItems(IList<ProjectItem> projectItems, JSLintNetSettings settings)
        {
            if (projectItems.Count < 1)
            {
                return 0;
            }

            var hierarchy = this.GetHierarchy(projectItems[0].ContainingProject);
            var errors = 0;
            var files = 0;
            var errorFiles = 0;
            var exceptions = 0;
            var errorLimit = settings.ErrorLimitOrDefault();
            var filesLimit = settings.FileLimitOrDefault();

            using (var jsLintContext = this.jsLintFactory.CreateContext())
            {
                foreach (var item in projectItems)
                {
                    var fileName = item.Access().FileName;
                    this.errorListProvider.ClearJSLintErrors(fileName);

                    var isOpen = item.Document != null;

                    var result = ExecutionHelper.Try(() =>
                    {
                        string source;
                        if (isOpen)
                        {
                            source = item.Document.Access().Source;
                        }
                        else
                        {
                            source = this.fileSystemWrapper.ReadAllText(fileName, Encoding.UTF8);
                        }

                        return jsLintContext.Lint(source, settings.Options);
                    });

                    if (result.Success)
                    {
                        var data = result.Data;

                        if (data.Errors.Count > 0)
                        {
                            errors += data.Errors.Count;
                            errorFiles += 1;

                            this.errorListProvider.AddJSLintErrors(fileName, data.Errors, settings.Output, hierarchy);

                            if (errors >= errorLimit)
                            {
                                this.errorListProvider.AddCustomError(CoreResources.ErrorLimitReachedFormat, errors);

                                break;
                            }
                        }
                    }
                    else
                    {
                        var ex = result.Exception;

                        this.errorListProvider.AddCustomError(CoreResources.ProcessingExceptionFormat, fileName, ex.Message);

                        exceptions += 1;

                        if (exceptions >= JSLintNetSettings.ExceptionLimit)
                        {
                            this.errorListProvider.AddCustomError(CoreResources.ExceptionLimitReachedFormat, exceptions);

                            break;
                        }
                    }

                    files += 1;

                    if (files >= filesLimit)
                    {
                        this.errorListProvider.AddCustomError(CoreResources.FileLimitReachedFormat, files);

                        break;
                    }
                }
            }

            var text = GetMessageText(projectItems.Count, errorFiles, errors);
            this.SetStatusBar(text);

            return errors;
        }

        /// <summary>
        /// Loads the settings for the specified project, automatically merging from the current build configuration.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>
        /// Settings for the specified project, or null if none could be found.
        /// </returns>
        public JSLintNetSettings LoadSettings(Project project)
        {
            return this.LoadSettings(project, true);
        }

        /// <summary>
        /// Loads the settings for the specified project and optionally merging from the current build configuration.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="merge">Set to <c>true</c> to merge the settings from the current build configuration, otherwise <c>false</c>.</param>
        /// <returns>
        /// Settings for the specified project, or null if none could be found.
        /// </returns>
        public JSLintNetSettings LoadSettings(Project project, bool merge)
        {
            var settingsPath = GetSettingsPath(project);

            if (string.IsNullOrEmpty(settingsPath))
            {
                return new JSLintNetSettings();
            }

            var configuration = merge && project.ConfigurationManager != null ?
                project.ConfigurationManager.ActiveConfiguration.ConfigurationName :
                null;

            var settingsKey = string.IsNullOrEmpty(configuration) ?
                settingsPath :
                configuration + "_" + settingsPath;

            if (this.cacheProvider.Contains(settingsKey))
            {
                return this.cacheProvider.Get<JSLintNetSettings>(settingsKey);
            }

            var settings = this.settingsRepository.Load(settingsPath, configuration);

            this.cacheProvider.Set(settingsKey, settings, 10, settings.Files.ToArray());

            return settings;
        }

        /// <summary>
        /// Saves the settings for the specified project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="settings">The settings.</param>
        public void SaveSettings(Project project, JSLintNetSettings settings)
        {
            var settingsPath = GetSettingsPath(project);

            this.settingsRepository.Save(settings, settingsPath);
            var item = project.ProjectItems.Locate().Item(JSLintNetSettings.FileName);

            if (item == null)
            {
                project.ProjectItems.AddFromFile(settingsPath);
            }
        }

        private static string GetSettingsPath(Project project)
        {
            var fileName = JSLintNetSettings.FileName;
            var settingsItem = project.ProjectItems.Locate().Item(fileName);

            if (settingsItem != null)
            {
                return settingsItem.Access().FileName;
            }

            var projectDirectory = project.Access().Directory;

            if (string.IsNullOrEmpty(projectDirectory))
            {
                return null;
            }

            return Path.Combine(projectDirectory, fileName);
        }

        private static string GetMessageText(int errors)
        {
            var errorsText = errors == 1 ? Resources.JSLintErrorFound : Resources.JSLintErrorsFound;

            return string.Concat(
                errors,
                " ",
                errorsText);
        }

        private static string GetMessageText(int files, int errorFiles, int errors)
        {
            var filesText = files == 1 ? Resources.FileScanned : Resources.FilesScanned;
            var errorFilesText = errorFiles == 1 ? Resources.FileWithErrors : Resources.FilesWithErrors;
            var errorsText = errors == 1 ? Resources.JSLintErrorFound : Resources.JSLintErrorsFound;

            return string.Concat(
                files,
                " ",
                filesText,
                ", ",
                errorFiles,
                " ",
                errorFilesText,
                ", ",
                errors,
                " ",
                errorsText);
        }

        private void SetStatusBar(string text)
        {
            this.statusBar.FreezeOutput(0);
            this.statusBar.SetText(text);
        }

        private IVsHierarchy GetHierarchy(Project project)
        {
            IVsHierarchy hierarchy;
            this.solutionService.GetProjectOfUniqueName(project.UniqueName, out hierarchy);

            return hierarchy;
        }
    }
}
