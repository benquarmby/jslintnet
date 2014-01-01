namespace JSLintNet.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using JSLintNet.Abstractions;
    using JSLintNet.Console.Properties;
    using JSLintNet.Json;

    internal class ConsoleOptionsFactory
    {
        private IFileSystemWrapper fileSystemWrapper;

        private IJsonProvider jsonProvider;

        public ConsoleOptionsFactory()
            : this(new FileSystemWrapper(), new JsonProvider())
        {
        }

        public ConsoleOptionsFactory(IFileSystemWrapper fileSystemWrapper, IJsonProvider jsonProvider)
        {
            this.fileSystemWrapper = fileSystemWrapper;
            this.jsonProvider = jsonProvider;
        }

        public ConsoleOptions Create(string[] args)
        {
            var options = this.Parse(args);

            if (!options.Help)
            {
                if (!options.SettingsEditor)
                {
                    this.ResolveSourceDirectory(options);
                    this.ResolveSourceFiles(options);
                }

                this.ResolveSettings(options);
            }

            return options;
        }

        private static KeyValuePair<string, string> GetOption(Queue<string> arguments)
        {
            var key = arguments.Dequeue();
            var value = GetValue(arguments);

            return new KeyValuePair<string, string>(key, value);
        }

        private static string GetValue(Queue<string> arguments)
        {
            if (arguments.Count > 0 && !IsKey(arguments.Peek()))
            {
                return arguments.Dequeue();
            }

            return null;
        }

        private static bool IsKey(string value)
        {
            return value.StartsWith("/") || value.StartsWith("-");
        }

        private ConsoleOptions Parse(string[] args)
        {
            var options = new ConsoleOptions();

            if (args == null || args.Length < 1)
            {
                options.Help = true;

                return options;
            }

            var arguments = new Queue<string>(args);
            var first = arguments.Dequeue();

            if (IsKey(first))
            {
                switch (first.ToLowerInvariant())
                {
                    case "/?":
                    case "/h":
                    case "/help":
                    case "-?":
                    case "-h":
                    case "-help":
                        options.Help = true;

                        return options;
                }
            }
            else
            {
                if (first.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    options.SettingsFile = first;
                    options.SettingsEditor = true;

                    return options;
                }
                else
                {
                    options.SourceDirectory = first;
                }
            }

            if (string.IsNullOrEmpty(options.SourceDirectory))
            {
                throw new ArgumentException(Resources.ExceptionInvalidSourceDirectoryArgument);
            }

            while (arguments.Count > 0)
            {
                var option = GetOption(arguments);

                switch (option.Key.ToLowerInvariant())
                {
                    case "/s":
                    case "/settingsfile":
                    case "-s":
                    case "-settingsfile":
                        if (string.IsNullOrEmpty(option.Value))
                        {
                            throw new ArgumentException(Resources.ExceptionInvalidSettingsFileArgument, option.Key);
                        }

                        options.SettingsFile = option.Value;

                        break;
                    case "/r":
                    case "/reportfile":
                    case "-r":
                    case "-reportfile":
                        if (string.IsNullOrEmpty(option.Value))
                        {
                            throw new ArgumentException(Resources.ExceptionInvalidReportFileArgument, option.Key);
                        }

                        options.ReportFile = option.Value;

                        break;
                    case "/l":
                    case "/loglevel":
                    case "-l":
                    case "-loglevel":
                        {
                            LogLevel logLevel;
                            Enum.TryParse(option.Value, true, out logLevel);

                            options.LogLevel = logLevel;
                        }

                        break;
                    default:
                        throw new ArgumentException(Resources.ExceptionUnrecognizedArgument, option.Key);
                }
            }

            return options;
        }

        private void ResolveSourceDirectory(ConsoleOptions options)
        {
            options.SourceDirectory = this.fileSystemWrapper.ResolveDirectory(options.SourceDirectory);
        }

        private void ResolveSettings(ConsoleOptions options)
        {
            JSLintNetSettings settings = null;
            var settingsPath = options.SettingsFile;

            if (string.IsNullOrEmpty(settingsPath))
            {
                settingsPath = Path.Combine(options.SourceDirectory, JSLintNetSettings.FileName);
            }
            else
            {
                settingsPath = this.fileSystemWrapper.ResolveFile(settingsPath);
            }

            options.SettingsFile = settingsPath;

            if (this.fileSystemWrapper.FileExists(settingsPath))
            {
                var settingsSource = this.fileSystemWrapper.ReadAllText(settingsPath, Encoding.UTF8);

                settings = this.jsonProvider.DeserializeSettings(settingsSource);

                if (settings != null)
                {
                    settings.File = settingsPath;
                }
            }

            options.Settings = settings ?? new JSLintNetSettings();
        }

        private void ResolveSourceFiles(ConsoleOptions options)
        {
            options.SourceFiles = this.fileSystemWrapper.GetFiles(options.SourceDirectory, "*", SearchOption.AllDirectories);
        }
    }
}
