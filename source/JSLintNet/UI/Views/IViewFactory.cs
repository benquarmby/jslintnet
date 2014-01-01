namespace JSLintNet.UI.Views
{
    using JSLintNet.UI.ViewModels;

    internal interface IViewFactory
    {
        IView CreateSettings();

        IView CreateSettings(SettingsViewModel viewModel);
    }
}
