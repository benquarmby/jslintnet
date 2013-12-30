namespace JSLintNet.VisualStudio.Errors
{
    using Microsoft.VisualStudio.Shell;

    internal class CustomErrorTask : ErrorTask
    {
        public CustomErrorTask(string message, params object[] args)
        {
            this.Category = TaskCategory.BuildCompile;
            this.ErrorCategory = TaskErrorCategory.Error;
            this.Text = string.Format(message, args);
        }
    }
}
