namespace JSLintNet
{
    using Microsoft.ClearScript.V8;

    internal class JavaScriptContext : IJavaScriptContext
    {
        private V8ScriptEngine engine;

        public JavaScriptContext()
        {
            this.engine = new V8ScriptEngine();
        }

        public dynamic Script
        {
            get
            {
                return this.engine.Script;
            }
        }

        public object Run(string source)
        {
            return this.engine.Evaluate(source);
        }

        public void SetParameter(string name, object value)
        {
            this.engine.AddHostObject(name, value);
        }

        public void TerminateExecution()
        {
            this.engine.Interrupt();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.engine != null)
            {
                this.engine.Dispose();
            }
        }
    }
}
