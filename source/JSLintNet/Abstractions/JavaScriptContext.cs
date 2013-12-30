#pragma warning disable 1591

namespace JSLintNet.Abstractions
{
    using Noesis.Javascript;

    internal class JavaScriptContext : JavascriptContext, IJavaScriptContext
    {
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
