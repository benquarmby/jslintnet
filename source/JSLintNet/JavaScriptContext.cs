namespace JSLintNet
{
    using System;
    using System.Threading;
    using System.Windows.Forms;
    using JSLintNet.Properties;

    internal class JavaScriptContext : IJavaScriptContext
    {
        private WebBrowser browser;

        private Thread thread;

        public JavaScriptContext()
        {
            if (!this.Initialize())
            {
                throw new Exception(Resources.JavaScriptContextFailedError);
            }
        }

        public void InjectScript(string source)
        {
            this.browser.Invoke(new Action(() =>
            {
                var script = this.browser.Document.CreateElement("script");
                dynamic element = script.DomElement;

                element.type = "text/javascript";
                element.text = source;

                this.browser.Document.GetElementsByTagName("head")[0].AppendChild(script);
            }));
        }

        public object InvokeFunction(string function, params object[] args)
        {
            return this.browser.Invoke(new Func<object>(() =>
            {
                var document = this.browser.Document;

                return document.InvokeScript(function, args);
            }));
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool managed)
        {
            if (managed)
            {
                if (this.browser != null && !this.browser.IsDisposed)
                {
                    this.browser.Invoke(new Action(() =>
                    {
                        this.browser.Dispose();

                        if (this.thread != null)
                        {
                            // Exit the message loop
                            Application.ExitThread();
                        }
                    }));

                    this.browser = null;
                }

                if (this.thread != null)
                {
                    this.thread.Join(1000);
                    this.thread = null;
                }
            }
        }

        private bool Initialize()
        {
            var reset = new AutoResetEvent(false);

            WebBrowserDocumentCompletedEventHandler handler = (object sender, WebBrowserDocumentCompletedEventArgs e) =>
            {
                reset.Set();
            };

            this.thread = new Thread(() =>
            {
                this.browser = new WebBrowser()
                {
                    ScriptErrorsSuppressed = true,
                    DocumentText = Resources.BrowserContext
                };

                this.browser.DocumentCompleted += handler;

                // Start the message loop on this thread
                Application.Run();
            });

            this.thread.IsBackground = true;
            this.thread.SetApartmentState(ApartmentState.STA);
            this.thread.Start();

            var completed = reset.WaitOne(1000);

            this.browser.DocumentCompleted -= handler;

            return completed;
        }
    }
}
