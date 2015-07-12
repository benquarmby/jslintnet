namespace JSLintNet.Execution
{
    using System;

    internal static class ExecutionHelper
    {
        public static ExecutionResult Try(Action action)
        {
            try
            {
                action();

                return new ExecutionResult()
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResult()
                {
                    Success = false,
                    Exception = ex
                };
            }
        }

        public static ExecutionResult<T> Try<T>(Func<T> func)
        {
            try
            {
                var data = func();

                return new ExecutionResult<T>()
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResult<T>()
                {
                    Success = false,
                    Exception = ex
                };
            }
        }
    }
}
