namespace JSLintNet.VisualStudio.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ErrorListChangeEventArgs : EventArgs
    {
        public ErrorListChangeEventArgs(ErrorListAction action, IEnumerable<string> fileNames)
        {
            this.Action = action;
            this.FileNames = fileNames;
        }

        public ErrorListAction Action { get; set; }

        public IEnumerable<string> FileNames { get; set; }

        public bool ContainsFile(string fileName)
        {
            return this.FileNames != null && this.FileNames.Contains(fileName, StringComparer.OrdinalIgnoreCase);
        }
    }
}
