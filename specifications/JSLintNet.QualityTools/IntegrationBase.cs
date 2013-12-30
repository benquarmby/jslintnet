namespace JSLintNet.QualityTools
{
    using System;
    using Xunit;

    [Trait("Category", "Integration")]
    public abstract class IntegrationBase : IDisposable
    {
        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
