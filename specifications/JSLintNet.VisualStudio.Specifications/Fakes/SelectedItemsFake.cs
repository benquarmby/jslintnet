namespace JSLintNet.VisualStudio.Specifications.Fakes
{
    using EnvDTE;

    public class SelectedItemsFake : EnumerableFake<SelectedItem>, SelectedItems
    {
        private DTE environment;

        public SelectedItemsFake(DTE environment)
        {
            this.environment = environment;
        }

        public bool MultiSelect
        {
            get
            {
                return this.Count > 1;
            }
        }

        public DTE Parent
        {
            get
            {
                return this.environment;
            }
        }

        public SelectionContainer SelectionContainer
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
