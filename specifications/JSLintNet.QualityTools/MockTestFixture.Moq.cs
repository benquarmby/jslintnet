namespace JSLintNet.QualityTools
{
    using System;
    using System.Linq.Expressions;
    using Moq;

    public partial class MockTestFixture<T> : IDisposable
    {
        public Mock<TService> GetMock<TService>()
            where TService : class
        {
            return this.AutoMocker.Mock<TService>();
        }

        public void Verify<TService>(Expression<Action<TService>> expression)
            where TService : class
        {
            this.AutoMocker.Mock<TService>().Verify(expression);
        }

        public void Verify<TService>(Expression<Action<TService>> expression, Times times)
            where TService : class
        {
            this.AutoMocker.Mock<TService>().Verify(expression, times);
        }

        public void Verify<TService>(Expression<Action<TService>> expression, Func<Times> times)
            where TService : class
        {
            this.AutoMocker.Mock<TService>().Verify(expression, times);
        }

        public void VerifyGet<TService, TProperty>(Expression<Func<TService, TProperty>> expression)
            where TService : class
        {
            this.AutoMocker.Mock<TService>().VerifyGet(expression);
        }
    }
}
