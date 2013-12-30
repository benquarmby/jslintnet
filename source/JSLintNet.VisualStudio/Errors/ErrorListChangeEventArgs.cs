namespace JSLintNet.VisualStudio.Errors
{
    using System;

    internal class ErrorListChangeEventArgs : EventArgs
    {
        public ErrorListChangeEventArgs(ErrorListAction action, string fileName)
        {
            this.Action = action;
            this.FileName = fileName;
        }

        public ErrorListAction Action { get; set; }

        public string FileName { get; set; }
    }
}
