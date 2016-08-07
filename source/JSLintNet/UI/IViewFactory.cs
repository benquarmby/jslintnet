namespace JSLintNet.UI
{
    using JSLintNet.Settings;

    internal interface IViewFactory
    {
        IView CreateSettings(JSLintNetSettings settings);
    }
}
