namespace JSLintNet.VisualStudio.Specifications.Errors
{
    using System;
    using System.Collections.Generic;
    using EnvDTE;
    using EnvDTE80;
    using JSLintNet.Models;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using JSLintNet.Settings;
    using JSLintNet.VisualStudio.Errors;
    using JSLintNet.VisualStudio.Specifications.Fakes;
    using Microsoft.VisualStudio.Shell.Interop;
    using Moq;
    using Xunit;

    public class JSLintErrorListProviderUnit
    {
        public class GetErrors : UnitBase
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

        public class ClearJSLintErrors : UnitBase
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

        private abstract class JSLintErrorListProviderTestableBase : TestFixture<JSLintErrorListProvider>
        {
            public JSLintErrorListProviderTestableBase()
            {
                this.EnvironmentMock = new Mock<DTE>().As<DTE2>();
                this.ErrorItemsFake = new ErrorItemsFake((DTE)this.EnvironmentMock.Object);
            }

            public Mock<DTE2> EnvironmentMock { get; set; }

            public ErrorItemsFake ErrorItemsFake { get; set; }

            public void AddErrors(string fileName, int errors)
            {
                var errorList = new List<IJSLintError>();

                for (var i = 0; i < errors; i += 1)
                {
                    errorList.Add(new JSLintError());
                }

                this.Instance.AddJSLintErrors(fileName, errorList, Output.Error, Mock.Of<IVsHierarchy>());
            }

            protected override void BeforeResolve()
            {
                base.BeforeResolve();

                var toolWindows = Mock.Of<ToolWindows>(y =>
                    y.ErrorList == Mock.Of<ErrorList>(z =>
                        z.ErrorItems == this.ErrorItemsFake));

                this.EnvironmentMock
                    .SetupGet(x => x.ToolWindows)
                    .Returns(toolWindows);

                this.GetMock<IServiceProvider>()
                    .Setup(x => x.GetService(typeof(DTE)))
                    .Returns(this.EnvironmentMock.Object);
            }
        }
    }
}
