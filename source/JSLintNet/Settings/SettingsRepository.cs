namespace JSLintNet.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Text;
    using JSLintNet.Abstractions;
    using JSLintNet.Json;

    internal class SettingsRepository : ISettingsRepository
    {
        private IFileSystemWrapper fileSystemWrapper;

        private IJsonProvider jsonProvider;

        public SettingsRepository()
            : this(new FileSystemWrapper(), new JsonProvider())
        {
        }

        public SettingsRepository(IFileSystemWrapper fileSystemWrapper, IJsonProvider jsonProvider)
        {
            this.fileSystemWrapper = fileSystemWrapper;
            this.jsonProvider = jsonProvider;
        }

        public JSLintNetSettings Load(string settingsPath)
        {
            return this.Load(settingsPath, null);
        }

        public JSLintNetSettings Load(string settingsPath, string configuration)
        {
            var mergePath = GetMergePath(settingsPath, configuration);

            JSLintNetSettings settings;
            if (this.TryGetSettings(settingsPath, out settings))
            {
                JSLintNetSettings merge;
                if (!string.IsNullOrEmpty(mergePath) && this.TryGetSettings(mergePath, out merge))
                {
                    settings.Merge(merge);
                }
            }
            else
            {
                settings = new JSLintNetSettings();
            }

            settings.Files.Add(settingsPath);
            if (!string.IsNullOrEmpty(mergePath))
            {
                settings.Files.Add(mergePath);
            }

            return settings;
        }

        public void Save(JSLintNetSettings settings, string settingsPath)
        {
            settings.Version = AssemblyInfo.InformationalVersion;
            var settingsJson = this.jsonProvider.SerializeSettings(settings);

            this.fileSystemWrapper.WriteAllText(settingsPath, settingsJson, Encoding.UTF8);
        }

        private static string GetMergePath(string settingsPath, string configuration)
        {
            if (string.IsNullOrEmpty(configuration))
            {
                return null;
            }

            var mergeFile = string.Concat(
                Path.GetFileNameWithoutExtension(settingsPath),
                '.',
                configuration,
                Path.GetExtension(settingsPath));

            return Path.Combine(
                Path.GetDirectoryName(settingsPath),
                mergeFile);
        }

        private static void MigrateOptions(JSLintOptions jsLintOptions, IDictionary<string, object> options)
        {
            if (options.ContainsKey("evil"))
            {
                var evil = options["evil"] as bool?;

                if (evil.HasValue)
                {
                    jsLintOptions.TolerateEval = evil;
                }
            }
        }

        private static void MigrateGlobalVariables(IList<string> globalVariables, IDictionary<string, object> options)
        {
            if (options.ContainsKey("predef"))
            {
                var predef = options["predef"] as IDictionary<string, object>;

                if (predef != null)
                {
                    foreach (var key in predef.Keys)
                    {
                        globalVariables.Add(key);
                    }
                }
            }
        }

        private bool TryGetSettings(string settingsPath, out JSLintNetSettings settings)
        {
            if (this.fileSystemWrapper.FileExists(settingsPath))
            {
                var settingsSource = this.fileSystemWrapper.ReadAllText(settingsPath, Encoding.UTF8);

                if (!string.IsNullOrEmpty(settingsSource))
                {
                    settings = this.jsonProvider.DeserializeSettings(settingsSource);

                    this.MaybeMigrate(settings, settingsSource);

                    return settings != null;
                }
            }

            settings = null;
            return false;
        }

        private void MaybeMigrate(JSLintNetSettings settings, string settingsSource)
        {
            if (settings != null && !AssemblyInfo.InformationalVersion.Equals(settings.Version, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    dynamic oldSettings = this.jsonProvider.DeserializeObject<ExpandoObject>(settingsSource);
                    var options = oldSettings.options as IDictionary<string, object>;

                    if (options != null)
                    {
                        MigrateOptions(settings.Options, options);
                        MigrateGlobalVariables(settings.GlobalVariables, options);
                    }
                }
                catch
                {
                }
            }
        }
    }
}
