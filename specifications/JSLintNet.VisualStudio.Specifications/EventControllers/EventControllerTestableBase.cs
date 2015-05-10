namespace JSLintNet.VisualStudio.Specifications.EventControllers
{
    using System;
    using EnvDTE;
    using EnvDTE80;
    using JSLintNet.QualityTools;
    using JSLintNet.VisualStudio.EventControllers;
    using Moq;

    internal abstract class EventControllerTestableBase<T> : TestFixture<T>
        where T : EventControllerBase
    {
        public EventControllerTestableBase()
        {
            this.EnvironmentMock = new Mock<DTE>().As<DTE2>();
            this.EventsMock = new Mock<Events>();
        }

        public Mock<DTE2> EnvironmentMock { get; set; }

        public Mock<Events> EventsMock { get; set; }

        protected override void BeforeResolve()
        {
            base.BeforeResolve();

            this.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(DTE)))
                .Returns(this.EnvironmentMock.Object);

            this.EnvironmentMock
                .SetupGet(x => x.Events)
                .Returns(this.EventsMock.Object);
        }

        protected override void AfterResolve()
        {
            base.AfterResolve();

            this.Instance.Initialize();
        }
    }
}
