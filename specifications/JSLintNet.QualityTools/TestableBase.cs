namespace JSLintNet.QualityTools
{
    using System;
    using System.Reflection;
    using Autofac;
    using Autofac.Extras.Moq;

    /// <summary>
    /// A container for testable classes, providing logical scope and timing for mocking and other configuration.
    /// </summary>
    /// <typeparam name="T">Any testable class.</typeparam>
    public abstract partial class TestableBase<T> : IDisposable
        where T : class
    {
        private T instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestableBase{T}"/> class.
        /// </summary>
        public TestableBase()
        {
            this.AutoMocker = AutoMock.GetLoose();
        }

        /// <summary>
        /// Occurs before the <see cref="Instance"/> is initialized.
        /// </summary>
        protected event EventHandler BeforeInit;

        /// <summary>
        /// Occurs after the <see cref="Instance"/> is initialized.
        /// </summary>
        protected event EventHandler AfterInit;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        /// <remarks>
        /// The instance is constructed lazily. <see cref="Init"/> will be called automatically.
        /// </remarks>
        public virtual T Instance
        {
            get
            {
                this.Init();

                return this.instance;
            }
        }

        /// <summary>
        /// Gets the automatic mocker.
        /// </summary>
        /// <value>
        /// The automatic mocker.
        /// </value>
        public AutoMock AutoMocker { get; private set; }

        /// <summary>
        /// Manually initializes the <see cref="Instance"/>.
        /// </summary>
        public void Init()
        {
            if (this.instance == null)
            {
                var before = this.BeforeInit;

                if (before != null)
                {
                    before(this, EventArgs.Empty);
                }

                this.instance = this.Construct();

                var after = this.AfterInit;

                if (after != null)
                {
                    after(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable = this.instance as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// Constructs the instance to be tested.
        /// </summary>
        /// <returns>
        /// The instance to be tested.
        /// </returns>
        protected virtual T Construct()
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
