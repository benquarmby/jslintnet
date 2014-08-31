namespace JSLintNet.VisualStudio.Specifications.Errors
{
    using System.Collections.Generic;
    using JSLintNet.Models;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using JSLintNet.Settings;
    using JSLintNet.VisualStudio.Errors;
    using Microsoft.VisualStudio.Shell.Interop;
    using Moq;
    using Xunit;

    public class JSLintErrorListProviderUnit
    {
        public class GetErrors
        {
            [Fact(DisplayName = "Should get all errors with matching file names")]
            public void Spec01()
            {
                using (var testable = new GetErrorsTestable())
                {
                    testable.AddErrors("file1.js", 3);
                    testable.AddErrors("file2.js", 2);
                    testable.AddErrors("file3.js", 1);

                    var errors = testable.Instance.GetErrors("file1.js", "file2.js");

                    I.Expect(errors.Count).ToBe(5);
                }
            }

            private class GetErrorsTestable : JSLintErrorListProviderTestableBase
            {
            }
        }

        public class ClearJSLintErrors
        {
            [Fact(DisplayName = "Should clear all JSLint errors with matching file names")]
            public void Spec01()
            {
                using (var testable = new ClearJSLintErrorsTestable())
                {
                    var errors = testable.Instance.Tasks;

                    testable.AddErrors("file1.js", 3);
                    testable.AddErrors("file2.js", 2);
                    testable.AddErrors("file3.js", 1);

                    I.Expect(errors.Count).ToBe(6);

                    testable.Instance.ClearJSLintErrors("file1.js", "file3.js");

                    I.Expect(errors.Count).ToBe(2);
                }
            }

            private class ClearJSLintErrorsTestable : JSLintErrorListProviderTestableBase
            {
            }
        }

        private abstract class JSLintErrorListProviderTestableBase : TestableBase<JSLintErrorListProvider>
        {
            public void AddErrors(string fileName, int errors)
            {
                var errorList = new List<IJSLintError>();

                for (var i = 0; i < errors; i += 1)
                {
                    errorList.Add(new JSLintError());
                }

                this.Instance.AddJSLintErrors(fileName, errorList, Output.Error, Mock.Of<IVsHierarchy>());
            }
        }
    }
}
