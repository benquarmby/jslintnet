#pragma warning disable 1591

namespace JSLintNet.Abstractions
{
    using Microsoft.Build.Framework;

    internal interface IAbstractionFactory
    {
        ITaskLoggingHelper CreateTaskLoggingHelper(ITask task);

        ITaskLoggingHelper CreateTaskLoggingHelper(IBuildEngine buildEngine, string taskName);

        IJavaScriptContext CreateJavaScriptContext();
    }
}
