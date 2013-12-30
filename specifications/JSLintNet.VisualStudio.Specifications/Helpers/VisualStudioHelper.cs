namespace JSLintNet.VisualStudio.Specifications.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using EnvDTE;

    public static class VisualStudioHelper
    {
        public static IEnumerable<DTE> GetInstances()
        {
            IRunningObjectTable runningObjectTable;
            var retVal = GetRunningObjectTable(0, out runningObjectTable);

            if (retVal == 0)
            {
                IEnumMoniker enumMoniker;
                runningObjectTable.EnumRunning(out enumMoniker);

                var fetched = IntPtr.Zero;
                var monikers = new IMoniker[1];

                while (enumMoniker.Next(1, monikers, fetched) == 0)
                {
                    var moniker = monikers[0];

                    IBindCtx bindCtx;
                    CreateBindCtx(0, out bindCtx);

                    string displayName;
                    moniker.GetDisplayName(bindCtx, null, out displayName);

                    if (displayName.StartsWith("!VisualStudio"))
                    {
                        object dte;
                        runningObjectTable.GetObject(moniker, out dte);

                        yield return dte as DTE;
                    }
                }
            }
        }

        public static DTE GetCurrentEnvironment()
        {
            return GetInstances().FirstOrDefault(x => x.Solution.FullName.Contains("JSLint.NET.VisualStudio"));
        }

        [DllImport("ole32.dll")]
        private static extern void CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);
    }
}
