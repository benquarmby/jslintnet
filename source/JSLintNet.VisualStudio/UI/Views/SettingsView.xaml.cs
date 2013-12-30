namespace JSLintNet.VisualStudio.UI.Views
{
    using System.Windows;

    public partial class SettingsView : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsView"/> class.
        /// </summary>
        public SettingsView()
        {
            this.InitializeComponent();
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
