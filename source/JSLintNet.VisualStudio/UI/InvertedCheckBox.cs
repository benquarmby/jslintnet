namespace JSLintNet.VisualStudio.UI
{
    using System.Windows.Controls;

    internal class InvertedCheckBox : CheckBox
    {
        public InvertedCheckBox()
        {
            this.IsThreeState = true;
        }

        protected override void OnToggle()
        {
            if (this.IsChecked == true)
            {
                this.IsChecked = false;
            }
            else if (this.IsChecked == false)
            {
                this.IsChecked = this.IsThreeState ? null : (bool?)true;
            }
            else
            {
                this.IsChecked = true;
            }
        }
    }
}
