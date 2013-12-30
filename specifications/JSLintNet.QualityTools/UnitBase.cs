namespace JSLintNet.QualityTools
{
    using System;
    using Xunit;

    [Trait("Category", "Unit")]
    public abstract class UnitBase : IDisposable
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
