namespace JSLintNet.UI
{
    using JSLintNet.Settings;
    using JSLintNet.UI.Settings;

    internal class ViewFactory : IViewFactory
    {
        public IView CreateSettings(JSLintNetSettings settings)
        {
            var viewModel = new SettingsViewModel(settings);
            var view = new SettingsView()
            {
                DataContext = viewModel
            };

            viewModel.View = view;

            return view;
        }
    }
}
