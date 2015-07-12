namespace JSLintNet.Specifications.Tasks
{
    using System;
    using JSLintNet.Abstractions;
    using JSLintNet.QualityTools;

    public class JSLintTaskFixture : AutofacTestFixture<JSLintTask>
    {
        public JSLintTaskFixture()
        {
            this.BuildEngine = new BuildEngineStub();
            this.FileSystemWrapper = new FileSystemWrapperStub();
        }

        public BuildEngineStub BuildEngine { get; set; }

        public FileSystemWrapperStub FileSystemWrapper { get; set; }

        public void AddFile(string fileName, string contents)
        {
            var sourceDirectory = this.Instance.SourceDirectory;

            if (string.IsNullOrEmpty(sourceDirectory))
            {
                throw new InvalidOperationException("Cannot add a file without first setting the source directory.");
            }

            this.FileSystemWrapper.AddFile(sourceDirectory, fileName, contents);
        }

        protected override void BeforeResolve()
        {
            base.BeforeResolve();

            this.RegisterModule(new CoreModule())
                .RegisterInstance<IFileSystemWrapper>(this.FileSystemWrapper);
        }

        protected override void AfterResolve()
        {
            base.AfterResolve();

            this.Instance.BuildEngine = this.BuildEngine;
        }
    }
}
