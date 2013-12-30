#pragma warning disable 1591

namespace JSLintNet.Abstractions
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.Build.Framework;

    internal class AbstractionFactory : IAbstractionFactory
    {
        private const string JavaScriptLibrary = "Noesis.Javascript";

        static AbstractionFactory()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnDomainAssemblyResolve;
        }

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

        private static Assembly OnDomainAssemblyResolve(object sender, ResolveEventArgs e)
        {
            if (e.Name.StartsWith(JavaScriptLibrary))
            {
                return LoadJavaScriptAssembly();
            }

            return null;
        }

        private static Assembly LoadJavaScriptAssembly()
        {
            var jsLintPath = new Uri(typeof(AbstractionFactory).Assembly.CodeBase).LocalPath;
            var libraryPath = Path.Combine(
                Path.GetDirectoryName(jsLintPath),
                Environment.Is64BitProcess ? "amd64" : "x86",
                JavaScriptLibrary + ".dll");

            return Assembly.LoadFile(libraryPath);
        }
    }
}
