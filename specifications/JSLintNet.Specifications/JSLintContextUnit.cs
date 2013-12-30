namespace JSLintNet.Specifications
{
    using System.Collections.Generic;
    using JSLintNet.Abstractions;
    using JSLintNet.Json;
    using JSLintNet.Models;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using Moq;
    using Xunit;

    public class JSLintContextUnit
    {
        public class Constructor : UnitBase
        {
            [Fact(DisplayName = "Should create a new javascript context")]
            public void Spec01()
            {
                using (var testable = new ConstructorTestable())
                {
                    testable.Init();

                    testable.Verify<IAbstractionFactory>(x => x.CreateJavaScriptContext());
                }
            }

            [Fact(DisplayName = "Should setup jslint inside a javascript context")]
            public void Spec02()
            {
                using (var testable = new ConstructorTestable())
                {
                    testable.Init();

                    I.Expect(testable.ContextRuns[0]).ToStartWith("// jslint.js");
                }
            }

            private class ConstructorTestable : JSLintContextTestableBase
            {
            }
        }

        public class Dispose : UnitBase
        {
            [Fact(DisplayName = "Should dispose of javascript context")]
            public void Spec01()
            {
                using (var testable = new DisposeTestable())
                {
                    testable.Instance.Dispose();

                    testable.JavaScriptContextMock.Verify(x => x.Dispose());
                }
            }

            private class DisposeTestable : JSLintContextTestableBase
            {
            }
        }

        public class Lint : UnitBase
        {
            [Fact(DisplayName = "Should pass source to javascript context")]
            public void Spec01()
            {
                using (var testable = new LintTestable())
                {
                    testable.Instance.Lint("some source");

                    testable.JavaScriptContextMock.Verify(x => x.SetParameter(JSLintParameters.SourceParameter, "some source"));
                }
            }

            [Fact(DisplayName = "Should pass serialized options to javascript context")]
            public void Spec02()
            {
                using (var testable = new LintTestable())
                {
                    testable.GetMock<IJsonProvider>()
                        .Setup(x => x.SerializeOptions(It.IsAny<JSLintOptions>()))
                        .Returns(@"{""some"":""json""}");

                    var options = new JSLintOptions()
                    {
                        TolerateStupidPractices = true
                    };

                    testable.Instance.Lint("some source", options);

                    testable.JavaScriptContextMock.Verify(x => x.SetParameter(JSLintParameters.OptionsParameter, @"{""some"":""json""}"));
                }
            }

            [Fact(DisplayName = "Should run JSLint over source")]
            public void Spec03()
            {
                using (var testable = new LintTestable())
                {
                    testable.Instance.Lint("some source");

                    I.Expect(testable.ContextRuns[1]).ToStartWith("JSLINT(" + JSLintParameters.SourceParameter);
                }
            }

            [Fact(DisplayName = "Should run JSLint with options")]
            public void Spec04()
            {
                using (var testable = new LintTestable())
                {
                    testable.Instance.Lint("some source");

                    I.Expect(testable.ContextRuns[1]).ToContain(JSLintParameters.OptionsParameter);
                }
            }

            [Fact(DisplayName = "Should fetch data from JSLint run")]
            public void Spec05()
            {
                using (var testable = new LintTestable())
                {
                    testable.Instance.Lint("some source");

                    I.Expect(testable.ContextRuns[2]).ToContain("JSLINT.data()");
                }
            }

            private class LintTestable : JSLintContextTestableBase
            {
            }
        }

        private abstract class JSLintContextTestableBase : TestableBase<JSLintContext>
        {
            public JSLintContextTestableBase()
            {
                this.ContextRuns = new List<string>();
                this.JavaScriptContextMock = new Mock<IJavaScriptContext>();

                this.BeforeInit += this.OnBeforeInit;
            }

            public List<string> ContextRuns { get; set; }

            public Mock<IJavaScriptContext> JavaScriptContextMock { get; set; }

            private void OnBeforeInit(object sender, System.EventArgs e)
            {
                var mock = Mock.Of<IJavaScriptContext>(x => x.Run(It.IsAny<string>()) == new Dictionary<string, object>());

                this.JavaScriptContextMock
                    .Setup(x => x.Run(It.IsAny<string>()))
                    .Callback((string x) => this.ContextRuns.Add(x))
                    .Returns(new Dictionary<string, object>());

                this.GetMock<IJsonProvider>()
                    .Setup(x => x.DeserializeData(It.IsAny<string>()))
                    .Returns(() => new JSLintData());

                this.GetMock<IAbstractionFactory>()
                    .Setup(x => x.CreateJavaScriptContext())
                    .Returns(this.JavaScriptContextMock.Object);
            }
        }
    }
}
