namespace JSLintNet.Abstractions
{
    using Microsoft.Build.Framework;

    internal class AbstractionFactory : IAbstractionFactory
    {
        public ITaskLoggingHelper CreateTaskLoggingHelper(ITask task)
        {
            return new TaskLoggingHelper(task);
        }

        public ITaskLoggingHelper CreateTaskLoggingHelper(IBuildEngine buildEngine, string taskName)
        {
            return new TaskLoggingHelper(buildEngine, taskName);
        }

        public IJavaScriptContext CreateJavaScriptContext()
        {
            return new JavaScriptContext();
        }
    }
}
