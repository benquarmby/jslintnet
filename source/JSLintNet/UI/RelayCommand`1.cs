namespace JSLintNet.UI
{
    using System;
    using System.Windows.Input;

    internal class RelayCommand<T> : ICommand
    {
        private Action<T> execute;

        private bool canExecute = true;

        public RelayCommand(Action<T> execute)
        {
            this.execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return this.canExecute;
        }

        public void Execute(object parameter)
        {
            this.canExecute = false;
            this.RaiseCanExecuteChanged();

            this.execute((T)parameter);

            this.canExecute = true;
            this.RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged()
        {
            var handler = this.CanExecuteChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
