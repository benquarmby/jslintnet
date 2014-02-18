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
            JSLintNetSettings settings;
            if (this.TryGetSettings(settingsPath, out settings))
            {
                settings.Files.Add(settingsPath);

                if (!string.IsNullOrEmpty(configuration))
                {
                    var mergeFile = string.Concat(
                        Path.GetFileNameWithoutExtension(settingsPath),
                        '.',
                        configuration,
                        Path.GetExtension(settingsPath));

                    var mergePath = Path.Combine(
                        Path.GetDirectoryName(settingsPath),
                        mergeFile);

                    settings.Files.Add(mergePath);

                    JSLintNetSettings merge;
                    if (this.TryGetSettings(mergePath, out merge))
                    {
                        settings.Merge(merge);
                    }
                }

                return settings;
            }

            return new JSLintNetSettings();
        }

        public void Save(JSLintNetSettings settings, string settingsPath)
        {
            var settingsJson = this.jsonProvider.SerializeSettings(settings);

            this.fileSystemWrapper.WriteAllText(settingsPath, settingsJson, Encoding.UTF8);
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
