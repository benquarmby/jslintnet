namespace JSLintNet.Settings
{
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

        private bool TryGetSettings(string settingsPath, out JSLintNetSettings settings)
        {
            if (this.fileSystemWrapper.FileExists(settingsPath))
            {
                var settingsSource = this.fileSystemWrapper.ReadAllText(settingsPath, Encoding.UTF8);

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
