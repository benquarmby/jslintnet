namespace JSLintNet.UI
{
    using JSLintNet.UI.Settings;

    internal interface IViewFactory
    {
        IView CreateSettings();

        IView CreateSettings(SettingsViewModel viewModel);
    }
}
