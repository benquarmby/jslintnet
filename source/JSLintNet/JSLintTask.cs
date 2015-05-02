namespace JSLintNet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using JSLintNet.Abstractions;
    using JSLintNet.Helpers;
    using JSLintNet.Models;
    using JSLintNet.Properties;
    using JSLintNet.Settings;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Task that will run JSLint over source files.
    /// </summary>
    public class JSLintTask : Task
    {
        private IJSLintFactory jsLintFactory;

        private IFileSystemWrapper fileSystemWrapper;

        private ISettingsRepository settingsRepository;

        private ITaskItem[] sourceFiles;

        private string outputOverride;

        private bool sourceFilesSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintTask"/> class with default services.
        /// </summary>
        public JSLintTask()
            : this(new JSLintFactory(), new FileSystemWrapper(), new AbstractionFactory(), new SettingsRepository())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintTask" /> class with custom services.
        /// </summary>
        /// <param name="jsLintFactory">The JSLint factory.</param>
        /// <param name="fileSystemWrapper">The file system wrapper.</param>
        /// <param name="abstractionFactory">The task logging helper factory.</param>
        /// <param name="settingsRepository">The settings repository.</param>
        internal JSLintTask(IJSLintFactory jsLintFactory, IFileSystemWrapper fileSystemWrapper, IAbstractionFactory abstractionFactory, ISettingsRepository settingsRepository)
        {
            this.jsLintFactory = jsLintFactory;
            this.fileSystemWrapper = fileSystemWrapper;
            this.settingsRepository = settingsRepository;

            this.LoggingHelper = abstractionFactory.CreateTaskLoggingHelper(this);
        }

        private delegate void Logger(string subcategory, string errorCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs);

        /// <summary>
        /// Gets or sets the source directory to be processed by JSLint.
        /// </summary>
        /// <value>
        /// The source directory.
        /// </value>
        [Required]
        public string SourceDirectory { get; set; }

        /// <summary>
        /// Gets or sets the source files to be processed by JSLint.
        /// </summary>
        /// <value>
        /// The source files.
        /// </value>
        public ITaskItem[] SourceFiles
        {
            get
            {
                return this.sourceFiles;
            }

            set
            {
                this.sourceFiles = value;
                this.sourceFilesSet = true;
            }
        }

        /// <summary>
        /// Gets or sets the output override.
        /// </summary>
        /// <value>
        /// The output override.
        /// </value>
        [Obsolete("Use the Configuration property together with a matching JSLintNet.CONFIGURATION.json file to override settings.")]
        public string OutputOverride
        {
            get
            {
                return this.outputOverride;
            }

            set
            {
                this.outputOverride = value;
            }
        }

        /// <summary>
        /// Gets or sets the build configuration.
        /// </summary>
        /// <value>
        /// The build configuration.
        /// </value>
        public string Configuration { get; set; }

        /// <summary>
        /// Gets or sets a custom path to a JSLint.NET settings file if it is not located in the root of the source directory.
        /// </summary>
        /// <value>
        /// The settings file.
        /// </value>
        public string SettingsFile { get; set; }

        /// <summary>
        /// Gets or sets the path to the HTML report file.
        /// </summary>
        /// <value>
        /// The report file.
        /// </value>
        public string ReportFile { get; set; }

        /// <summary>
        /// Gets the error count.
        /// </summary>
        /// <value>
        /// The error count.
        /// </value>
        [Output]
        public int ErrorCount { get; private set; }

        /// <summary>
        /// Gets the count of files with errors.
        /// </summary>
        /// <value>
        /// The count of files with errors.
        /// </value>
        [Output]
        public int ErrorFileCount { get; private set; }

        /// <summary>
        /// Gets the processed file count.
        /// </summary>
        /// <value>
        /// The processed file count.
        /// </value>
        [Output]
        public int ProcessedFileCount { get; private set; }

        /// <summary>
        /// Gets the logging helper.
        /// </summary>
        /// <value>
        /// The logging helper.
        /// </value>
        internal ITaskLoggingHelper LoggingHelper { get; private set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the task successfully executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool Execute()
        {
            var settings = this.LoadSettings();
            var sourceFiles = this.GetSourceFiles(settings);

            Output output;
            if (!Enum.TryParse(this.outputOverride, out output))
            {
                output = settings.Output.HasValue ? settings.Output.Value : default(Output);
            }

            if (sourceFiles.Count > 0)
            {
                var options = settings.Options;
                var logger = this.GetLogger(output);
                var errorLimit = settings.ErrorLimitOrDefault();
                var fileLimit = settings.FileLimitOrDefault();
                var exceptions = 0;

                using (var context = this.jsLintFactory.CreateContext())
                using (var reportBuilder = this.jsLintFactory.CreateReportBuilder())
                {
                    reportBuilder.SourceDirectory = this.SourceDirectory;
                    reportBuilder.AddSettings(settings.Files);

                    foreach (var file in sourceFiles)
                    {
                        var result = ExecutionHelper.Try(() =>
                        {
                            var source = this.fileSystemWrapper.ReadAllText(file.Absolute, Encoding.UTF8);

                            return context.Lint(source, options);
                        });

                        if (result.Success)
                        {
                            var data = result.Data;
                            reportBuilder.AddFile(file.Virtual, data);

                            foreach (var error in data.Warnings)
                            {
                                logger(
                                    subcategory: AssemblyInfo.Product,
                                    errorCode: null,
                                    helpKeyword: null,
                                    file: file.Absolute,
                                    lineNumber: error.Line,
                                    columnNumber: error.Column,
                                    endLineNumber: 0,
                                    endColumnNumber: 0,
                                    message: string.Concat(Resources.ErrorTextPrefix, error.Message));
                            }

                            if (reportBuilder.ErrorCount >= errorLimit)
                            {
                                this.LoggingHelper.LogError(Resources.ErrorLimitReachedFormat, reportBuilder.ErrorCount);

                                break;
                            }
                        }
                        else
                        {
                            this.LoggingHelper.LogError(
                                Resources.ErrorEncounteredFormat,
                                file.Virtual,
                                Environment.NewLine,
                                result.Exception.Message);

                            exceptions += 1;

                            if (exceptions >= JSLintNetSettings.ExceptionLimit)
                            {
                                this.LoggingHelper.LogError(Resources.ExceptionLimitReachedFormat, exceptions);

                                break;
                            }
                        }

                        if (reportBuilder.ProcessedFileCount >= fileLimit)
                        {
                            this.LoggingHelper.LogError(Resources.FileLimitReachedFormat, reportBuilder.ProcessedFileCount);

                            break;
                        }
                    }

                    var reportFile = this.ReportFile;
                    if (!string.IsNullOrEmpty(reportFile))
                    {
                        if (!Path.IsPathRooted(reportFile))
                        {
                            reportFile = Path.Combine(this.SourceDirectory, reportFile);
                        }

                        this.fileSystemWrapper.WriteAllText(reportFile, reportBuilder.ToString(), Encoding.UTF8);
                    }

                    this.ErrorCount = reportBuilder.ErrorCount;
                    this.ErrorFileCount = reportBuilder.ErrorFileCount;
                    this.ProcessedFileCount = reportBuilder.ProcessedFileCount;
                }
            }
            else
            {
                this.ErrorCount = 0;
                this.ErrorFileCount = 0;
                this.ProcessedFileCount = 0;
            }

            return output != Output.Error || this.ErrorCount == 0;
        }

        private Logger GetLogger(Output output)
        {
            switch (output)
            {
                case Output.Warning:
                case Output.Message:
                    return this.LoggingHelper.LogWarning;
                default:
                    return this.LoggingHelper.LogError;
            }
        }

        private IList<TaskFile> GetSourceFiles(JSLintNetSettings settings)
        {
            var ignored = settings.NormalizeIgnore();
            var taskFiles = new List<TaskFile>();

            if (this.sourceFilesSet)
            {
                foreach (var item in this.SourceFiles)
                {
                    if (JSLint.CanLint(item.ItemSpec))
                    {
                        var taskFile = new TaskFile(this.SourceDirectory, item);

                        if (!taskFile.IsIgnored(ignored))
                        {
                            taskFiles.Add(taskFile);
                        }
                    }
                }

                return taskFiles;
            }

            var files = this.fileSystemWrapper.GetFiles(this.SourceDirectory, "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (JSLint.CanLint(file))
                {
                    var taskFile = new TaskFile(this.SourceDirectory, file);

                    if (!taskFile.IsIgnored(ignored))
                    {
                        taskFiles.Add(taskFile);
                    }
                }
            }

            return taskFiles;
        }

        private JSLintNetSettings LoadSettings()
        {
            var settingsPath = this.ResolveSettingsPath();

            if (!Path.IsPathRooted(settingsPath))
            {
                settingsPath = Path.Combine(this.SourceDirectory, settingsPath);
            }

            return this.settingsRepository.Load(settingsPath, this.Configuration);
        }

        private string ResolveSettingsPath()
        {
            var settingsPath = this.SettingsFile;

            if (!string.IsNullOrEmpty(settingsPath))
            {
                // There is an explicit settings path, return it
                return settingsPath;
            }

            if (this.sourceFilesSet)
            {
                // Try to find settings in the source files
                foreach (var item in this.SourceFiles)
                {
                    if (JSLintNetSettings.IsSettingsFile(item.ItemSpec))
                    {
                        // The item spec matches exactly, short circuit
                        break;
                    }

                    var link = item.GetMetadata("Link");

                    if (JSLintNetSettings.IsSettingsFile(link))
                    {
                        // Matching link found, return the absolute path
                        return item.ItemSpec;
                    }
                }
            }

            // Fall back to the default
            return JSLintNetSettings.FileName;
        }
    }
}
