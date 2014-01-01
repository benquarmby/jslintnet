namespace JSLintNet.UI.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using JSLintNet.UI.Views;

    internal abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IView View { get; set; }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool managed)
        {
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);

                handler(this, e);
            }
        }

        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("Only member expressions are supported.", "propertyExpression");
            }

            this.RaisePropertyChanged(memberExpression.Member.Name);
        }
    }
}
