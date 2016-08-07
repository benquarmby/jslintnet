namespace JSLintNet.UI
{
    using JSLintNet.UI.Settings;

    internal class ViewFactory : IViewFactory
    {
        public IView CreateSettings()
        {
            return new SettingsView();
        }

        public IView CreateSettings(SettingsViewModel viewModel)
        {
            var view = new SettingsView()
            {
                DataContext = viewModel
            };

            viewModel.View = view;

            return view;
        }
    }
}
