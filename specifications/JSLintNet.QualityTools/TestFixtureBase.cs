namespace JSLintNet.QualityTools
{
    using System;

    public abstract class TestFixtureBase<T> : IDisposable
        where T : class
    {
        private bool initialized;

        private T instance;

        public T Instance
        {
            get
            {
                this.Initialize();

                return this.instance;
            }
        }

        public void Initialize()
        {
            if (this.initialized)
            {
                return;
            }

            this.initialized = true;

            this.BeforeResolve();
            this.instance = this.Resolve();
            this.AfterResolve();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

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

        protected virtual void BeforeResolve()
        {
        }

        protected abstract T Resolve();

        protected virtual void AfterResolve()
        {
        }
    }
}
