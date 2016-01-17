namespace JSLintNet.VisualStudio
{
    using System;
    using System.ComponentModel.Design;

    internal static class JSLintCommands
    {
        public const string PackageString = "20d11160-ceed-4510-8fc7-ab34c7c34caa";

        public static readonly Guid CommandSet = new Guid("ceb9d912-cf68-49ec-b2db-b1e02540ad2e");

        public static readonly CommandID ErrorListClear = new CommandID(CommandSet, 0x0100);

        public static readonly CommandID ItemNodeRun = new CommandID(CommandSet, 0x0110);

        public static readonly CommandID ItemNodeIgnore = new CommandID(CommandSet, 0x0120);

        public static readonly CommandID FolderNodeRun = new CommandID(CommandSet, 0x0130);

        public static readonly CommandID FolderNodeIgnore = new CommandID(CommandSet, 0x0140);

        public static readonly CommandID ProjectNodeRun = new CommandID(CommandSet, 0x0150);

        public static readonly CommandID ProjectNodeSettings = new CommandID(CommandSet, 0x0160);

        public static readonly CommandID CodeWindowRun = new CommandID(CommandSet, 0x0170);

        public static readonly CommandID CodeWindowGenerateProperty = new CommandID(CommandSet, 0x0180);
    }
}
