namespace JSLintNet.Specifications
{
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
