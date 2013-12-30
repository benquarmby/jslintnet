namespace JSLintNet.VisualStudio.UI
{
    using System;

    internal class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action<object> execute)
            : base(execute)
        {
        }
    }
}
