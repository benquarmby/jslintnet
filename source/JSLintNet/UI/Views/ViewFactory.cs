namespace JSLintNet.UI.Views
{
    using JSLintNet.UI.ViewModels;

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
