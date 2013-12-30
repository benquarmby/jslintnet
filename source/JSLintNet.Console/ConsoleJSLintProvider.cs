namespace JSLintNet.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using JSLintNet.Abstractions;
    using JSLintNet.Console.Properties;
    using JSLintNet.Helpers;
    using JSLintNet.Json;
    using CoreResources = JSLintNet.Properties.Resources;

    internal class ConsoleJSLintProvider
    {
        private static readonly Assembly ThisAssembly = typeof(ConsoleJSLintProvider).Assembly;

        private IJSLintFactory jsLintFactory;

        private IFileSystemWrapper fileSystemWrapper;

        private IJsonProvider jsonProvider;

        private IConsoleWriter consoleWriter;

        public ConsoleJSLintProvider()
            : this(new JSLintFactory(), new FileSystemWrapper(), new JsonProvider(), new ConsoleWriter())
        {
        }

        public ConsoleJSLintProvider(IJSLintFactory jsLintFactory, IFileSystemWrapper fileSystemWrapper, IJsonProvider jsonProvider, IConsoleWriter consoleWriter)
        {
            this.jsLintFactory = jsLintFactory;
            this.fileSystemWrapper = fileSystemWrapper;
            this.jsonProvider = jsonProvider;
            this.consoleWriter = consoleWriter;
        }

        public void WriteHeader()
        {
            var title = ThisAssembly.GetAttributeValue<AssemblyTitleAttribute>(x => x.Title);
            var version = ThisAssembly.GetAttributeValue<AssemblyInformationalVersionAttribute>(x => x.InformationalVersion);
            var copyright = ThisAssembly.GetAttributeValue<AssemblyCopyrightAttribute>(x => x.Copyright);

            this.consoleWriter
                .WriteLine()
                .WriteLine("{0} v{1}", title, version)
                .WriteLine(copyright);
        }

        public int WriteHelp()
        {
            var exeName = Path.GetFileNameWithoutExtension(new Uri(ThisAssembly.CodeBase).LocalPath);

            this.consoleWriter
                .WriteLine()
                .WriteLine(Resources.HelpSyntaxHeading)
                .WriteLine(4, Resources.HelpSyntaxFormat, exeName)
                .WriteLine()
                .WriteLine(Resources.HelpOptionsHeading)
                .WriteLine(2, Resources.HelpHelpOption)
                .WriteLine(4, Resources.HelpHelpDescription)
                .WriteLine()
                .WriteLine(2, Resources.HelpSourceDirectoryOption)
                .WriteLine(4, Resources.HelpSourceDirectoryDescription)
                .WriteLine()
                .WriteLine(2, Resources.HelpSettingsFileOption)
                .WriteLine(4, Resources.HelpSettingsFileDescriptionFormat, JSLintNetSettings.FileName)
                .WriteLine()
                .WriteLine(2, Resources.HelpReportFileOption)
                .WriteLine(4, Resources.HelpReportFileDescription)
                .WriteLine()
                .WriteLine(2, Resources.HelpLogLevelOption)
                .WriteLine(4, Resources.HelpLogLevelDescription)
                .WriteLine(6, Resources.HelpLogLevelSilent)
                .WriteLine(6, Resources.HelpLogLevelNormal)
                .WriteLine(6, Resources.HelpLogLevelVerbose);

            return 0;
        }

        public int Lint(ConsoleOptions options)
        {
            var sourceDirectory = options.SourceDirectory;
            var settings = options.Settings;
            var ignored = settings.NormalizeIgnore();
            var sourceFiles = options.SourceFiles.Where(x => JSLint.CanLint(x) && !IsIgnored(sourceDirectory, x, ignored)).ToArray();
            var errorLimit = settings.ErrorLimitOrDefault();
            var fileLimit = settings.FileLimitOrDefault();
            var exceptions = 0;

            using (var context = this.jsLintFactory.CreateContext())
            using (var reportBuilder = this.jsLintFactory.CreateReportBuilder())
            {
                reportBuilder.SourceDirectory = sourceDirectory;
                reportBuilder.SettingsFile = settings.File;

                foreach (var file in sourceFiles)
                {
                    var relative = GetRelativePath(sourceDirectory, file);
                    var result = ExecutionHelper.Try(() =>
                    {
                        var source = this.fileSystemWrapper.ReadAllText(file, Encoding.UTF8);

                        return context.Lint(source, settings.Options);
                    });

                    if (result.Success)
                    {
                        var data = result.Data;
                        var count = data.Errors.Count;

                        reportBuilder.AddFile(relative, data);

                        if (count > 0 && options.LogLevel == LogLevel.Verbose)
                        {
                            this.consoleWriter
                                .WriteErrorLine()
                                .WriteErrorLine(Resources.ErrorFileSummaryFormat, relative, count);

                            foreach (var error in data.Errors)
                            {
                                this.consoleWriter
                                    .WriteErrorLine(4, Resources.ErrorItemFormat, error.Reason, error.Line, error.Character);
                            }
                        }

                        if (reportBuilder.ErrorCount >= errorLimit)
                        {
                            this.consoleWriter
                                .WriteErrorLine()
                                .WriteErrorLine(CoreResources.ErrorLimitReachedFormat, reportBuilder.ErrorCount);

                            break;
                        }
                    }
                    else
                    {
                        var ex = result.Exception;

                        this.consoleWriter
                            .WriteErrorLine()
                            .WriteErrorLine(Resources.ExceptionFileFormat, relative)
                            .WriteErrorLine(ex.Message);

                        exceptions += 1;

                        if (exceptions >= JSLintNetSettings.ExceptionLimit)
                        {
                            this.consoleWriter
                                .WriteErrorLine()
                                .WriteErrorLine(CoreResources.ExceptionLimitReachedFormat, exceptions);

                            break;
                        }
                    }

                    if (reportBuilder.ProcessedFileCount >= fileLimit)
                    {
                        this.consoleWriter
                            .WriteErrorLine()
                            .WriteErrorLine(CoreResources.FileLimitReachedFormat, reportBuilder.ProcessedFileCount);

                        break;
                    }
                }

                if (options.LogLevel != LogLevel.Silent)
                {
                    this.consoleWriter
                        .WriteLine()
                        .WriteLine(Resources.SummarySourceDirectoryFormat, reportBuilder.SourceDirectory)
                        .WriteLine(Resources.SummarySettingsFileFormat, reportBuilder.SettingsFile)
                        .WriteLine(Resources.SummaryProcessedFileCountFormat, reportBuilder.ProcessedFileCount)
                        .WriteLine(Resources.SummaryErrorFileCountFormat, reportBuilder.ErrorFileCount)
                        .WriteLine(Resources.SummaryErrorCountFormat, reportBuilder.ErrorCount);
                }

                if (!string.IsNullOrEmpty(options.ReportFile))
                {
                    this.fileSystemWrapper.WriteAllText(options.ReportFile, reportBuilder.ToString(), Encoding.UTF8);
                }

                return reportBuilder.ErrorCount;
            }
        }

        public int WriteError(Exception exception)
        {
            this.consoleWriter
                .WriteErrorLine()
                .WriteErrorLine(Resources.ErrorEncounteredFormat, exception.Message);

            return -1;
        }

        private static string GetRelativePath(string sourceDirectory, string file)
        {
            var relative = file.Substring(sourceDirectory.Length);
            var directorySeparator = Path.DirectorySeparatorChar.ToString();

            if (!relative.StartsWith(directorySeparator))
            {
                relative = directorySeparator + relative;
            }

            return relative;
        }

        private static bool IsIgnored(string sourceDirectory, string file, IList<string> ignore)
        {
            var relative = GetRelativePath(sourceDirectory, file);

            return ignore.Any(x => relative.StartsWith(x, StringComparison.OrdinalIgnoreCase));
        }
    }
}
