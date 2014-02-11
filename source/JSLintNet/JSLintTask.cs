namespace JSLintNet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using JSLintNet.Abstractions;
    using JSLintNet.Helpers;
    using JSLintNet.Json;
    using JSLintNet.Models;
    using JSLintNet.Properties;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Task that will run JSLint over source files.
    /// </summary>
    public class JSLintTask : Task
    {
        private IJSLintFactory jsLintFactory;

        private IFileSystemWrapper fileSystemWrapper;

        private IJsonProvider jsonProvider;

        private ITaskItem[] sourceFiles;

        private bool sourceFilesSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintTask"/> class with default services.
        /// </summary>
        public JSLintTask()
            : this(new JSLintFactory(), new FileSystemWrapper(), new AbstractionFactory(), new JsonProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSLintTask" /> class with custom services.
        /// </summary>
        /// <param name="jsLintFactory">The JSLint factory.</param>
        /// <param name="fileSystemWrapper">The file system wrapper.</param>
        /// <param name="abstractionFactory">The task logging helper factory.</param>
        /// <param name="jsonProvider">The JSON provider.</param>
        internal JSLintTask(IJSLintFactory jsLintFactory, IFileSystemWrapper fileSystemWrapper, IAbstractionFactory abstractionFactory, IJsonProvider jsonProvider)
        {
            this.jsLintFactory = jsLintFactory;
            this.fileSystemWrapper = fileSystemWrapper;
            this.jsonProvider = jsonProvider;

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
        public string OutputOverride { get; set; }

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
            if (!Enum.TryParse(this.OutputOverride, out output))
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
                    reportBuilder.SettingsFile = settings.File;

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

                            foreach (var error in data.Errors)
                            {
                                logger(
                                    subcategory: AssemblyInfo.Product,
                                    errorCode: null,
                                    helpKeyword: null,
                                    file: file.Absolute,
                                    lineNumber: error.Line,
                                    columnNumber: error.Character,
                                    endLineNumber: 0,
                                    endColumnNumber: 0,
                                    message: string.Concat(Resources.ErrorTextPrefix, error.Reason));
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
            JSLintNetSettings settings;
            var settingsPath = this.SettingsFile;

            if (string.IsNullOrEmpty(settingsPath))
            {
                settingsPath = Path.Combine(this.SourceDirectory, JSLintNetSettings.FileName);
            }
            else if (!Path.IsPathRooted(settingsPath))
            {
                settingsPath = Path.Combine(this.SourceDirectory, settingsPath);
            }

            if (this.TryGetSettings(settingsPath, out settings))
            {
                settings.File = settingsPath;

                if (!string.IsNullOrEmpty(this.Configuration))
                {
                    var settingsFile = string.Concat(
                        Path.GetFileNameWithoutExtension(settingsPath),
                        '.',
                        this.Configuration + Path.GetExtension(settingsPath));

                    settingsPath = Path.Combine(Path.GetDirectoryName(settingsPath), settingsFile);

                    JSLintNetSettings merge;
                    if (this.TryGetSettings(settingsPath, out merge))
                    {
                        settings.Merge(merge);
                    }
                }

                return settings;
            }

            return new JSLintNetSettings();
        }

        private bool TryGetSettings(string path, out JSLintNetSettings settings)
        {
            if (this.fileSystemWrapper.FileExists(path))
            {
                var settingsSource = this.fileSystemWrapper.ReadAllText(path, Encoding.UTF8);

                if (!string.IsNullOrEmpty(settingsSource))
                {
                    settings = this.jsonProvider.DeserializeSettings(settingsSource);
                    return settings != null;
                }
            }

            settings = null;
            return false;
        }
    }
}
