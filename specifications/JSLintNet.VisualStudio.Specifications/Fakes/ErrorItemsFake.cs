namespace JSLintNet.VisualStudio.Specifications.Fakes
{
    using System;
    using System.Collections.Generic;
    using EnvDTE;
    using EnvDTE80;

    public class ErrorItemsFake : ErrorItems
    {
        public ErrorItemsFake(DTE environment)
        {
            this.DTE = environment;
            this.List = new List<ErrorItem>();
        }

        public IList<ErrorItem> List { get; private set; }

        public int Count
        {
            get
            {
                return this.List.Count;
            }
        }

        public DTE DTE { get; private set; }

        public ErrorList Parent
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ErrorItem Item(object index)
        {
            return this.List[(int)index];
        }
    }
}
