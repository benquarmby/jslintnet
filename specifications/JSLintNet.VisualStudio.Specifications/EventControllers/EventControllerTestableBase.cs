namespace JSLintNet.VisualStudio.Specifications.EventControllers
{
    using System;
    using EnvDTE;
    using EnvDTE80;
    using JSLintNet.QualityTools;
    using JSLintNet.VisualStudio.EventControllers;
    using Moq;

    internal abstract class EventControllerTestableBase<T> : TestableBase<T>
        where T : EventControllerBase
    {
        public EventControllerTestableBase()
        {
            this.EnvironmentMock = new Mock<DTE>().As<DTE2>();
            this.EventsMock = new Mock<Events>();

            this.BeforeInit += this.OnBeforeInit;

            this.AfterInit += this.OnAfterInit;
        }

        public Mock<DTE2> EnvironmentMock { get; set; }

        public Mock<Events> EventsMock { get; set; }

        private void OnBeforeInit(object sender, EventArgs e)
        {
            this.GetMock<IServiceProvider>()
                .Setup(x => x.GetService(typeof(DTE)))
                .Returns(this.EnvironmentMock.Object);

            this.EnvironmentMock
                .SetupGet(x => x.Events)
                .Returns(this.EventsMock.Object);
        }

        private void OnAfterInit(object sender, EventArgs e)
        {
            this.Instance.Initialize();
        }
    }
}
