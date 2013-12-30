#pragma warning disable 1591

namespace JSLintNet.Abstractions
{
    using Microsoft.Build.Framework;

    internal class TaskLoggingHelper : Microsoft.Build.Utilities.TaskLoggingHelper, ITaskLoggingHelper
    {
        public TaskLoggingHelper(ITask task)
            : base(task)
        {
        }

        public TaskLoggingHelper(IBuildEngine buildEngine, string taskName)
            : base(buildEngine, taskName)
        {
        }
    }
}
