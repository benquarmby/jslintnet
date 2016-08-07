namespace JSLintNet.UI
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    internal abstract class ViewModelBase : INotifyPropertyChanged
    {
        public ViewModelBase(IView view)
        {
            this.View = view;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected IView View { get; private set; }

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

        protected virtual void HandleAndClose(EventHandler handler, bool? result)
        {
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            var view = this.View;
            view.DialogResult = result;
            view.Close();
        }
    }
}
