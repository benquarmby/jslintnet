namespace JSLintNet.UI
{
    using JSLintNet.Settings;
    using JSLintNet.UI.Settings;

    internal class ViewFactory : IViewFactory
    {
        public IView CreateSettings(JSLintNetSettings settings)
        {
            var view = new SettingsView();
            var viewModel = new SettingsViewModel(view, settings);
            view.DataContext = viewModel;

            return view;
        }
    }
}
