namespace JSLintNet.Specifications
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Build.Framework;

    public class BuildEngineStub : IBuildEngine
    {
        public BuildEngineStub()
        {
            this.CustomEvents = new List<CustomBuildEventArgs>();
            this.ErrorEvents = new List<BuildErrorEventArgs>();
            this.WarningEvents = new List<BuildWarningEventArgs>();
            this.MessageEvents = new List<BuildMessageEventArgs>();
        }

        public List<CustomBuildEventArgs> CustomEvents { get; set; }

        public List<BuildErrorEventArgs> ErrorEvents { get; set; }

        public List<BuildWarningEventArgs> WarningEvents { get; set; }

        public List<BuildMessageEventArgs> MessageEvents { get; set; }

        public int ColumnNumberOfTaskNode { get; set; }

        public bool ContinueOnError { get; set; }

        public int LineNumberOfTaskNode { get; set; }

        public string ProjectFileOfTaskNode { get; set; }

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
            this.CustomEvents.Add(e);
        }

        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            this.ErrorEvents.Add(e);
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            this.MessageEvents.Add(e);
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            this.WarningEvents.Add(e);
        }

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            return true;
        }
    }
}
