namespace JSLintNet.UI
{
    using System;
    using System.ComponentModel;

    internal interface IView
    {
        event EventHandler Activated;

        event EventHandler Closed;

        event CancelEventHandler Closing;

        event EventHandler ContentRendered;

        event EventHandler Deactivated;

        event EventHandler LocationChanged;

        event EventHandler SourceInitialized;

        event EventHandler StateChanged;

        object DataContext { get; set; }

        bool? DialogResult { get; set; }

        bool Activate();

        void Close();

        void DragMove();

        void Hide();

        void Show();

        bool? ShowDialog();
    }
}
