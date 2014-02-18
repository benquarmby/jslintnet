namespace JSLintNet.Settings
{
    internal interface ISettingsRepository
    {
        JSLintNetSettings Load(string settingsPath);

        JSLintNetSettings Load(string settingsPath, string configuration);

        void Save(JSLintNetSettings settings, string settingsPath);
    }
}
