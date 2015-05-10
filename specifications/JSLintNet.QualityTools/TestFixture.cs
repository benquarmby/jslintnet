namespace JSLintNet.QualityTools
{
    using System.Reflection;
    using Autofac;
    using Autofac.Extras.Moq;

    /// <summary>
    /// A fixture for testable classes, providing logical scope and timing for mocking and other configuration.
    /// </summary>
    /// <typeparam name="T">Any testable class.</typeparam>
    public partial class TestFixture<T> : TestFixtureBase<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixture{T}"/> class.
        /// </summary>
        public TestFixture()
        {
            this.AutoMocker = AutoMock.GetLoose();
        }

        /// <summary>
        /// Gets the automatic mocker.
        /// </summary>
        /// <value>
        /// The automatic mocker.
        /// </value>
        public AutoMock AutoMocker { get; private set; }

        /// <summary>
        /// Resolves the instance to be tested.
        /// </summary>
        /// <returns>
        /// The instance to be tested.
        /// </returns>
        protected override T Resolve()
        {
            var builder = new ContainerBuilder();

            builder
                .RegisterType<T>()
                .FindConstructorsWith(x =>
                {
                    return x.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                });

            builder.Update(this.AutoMocker.Container);

            return this.AutoMocker.Create<T>();
        }
    }
}
