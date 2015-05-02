namespace JSLintNet.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
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
                    testable.Initialize();

                    testable.Verify<IAbstractionFactory>(x => x.CreateJavaScriptContext());
                }
            }

            [Fact(DisplayName = "Should setup jslintnet script inside a javascript context")]
            public void Spec02()
            {
                using (var testable = new ConstructorTestable())
                {
                    testable.Initialize();

                    I.Expect(testable.ContextRuns[0]).ToStartWith("// jslintnet.js");
                }
            }

            [Fact(DisplayName = "Should setup jslint inside a javascript context")]
            public void Spec03()
            {
                using (var testable = new ConstructorTestable())
                {
                    testable.Initialize();

                    I.Expect(testable.ContextRuns[1]).ToStartWith("// jslint.js");
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
            [Fact(DisplayName = "Should run JSLint over source")]
            public void Spec01()
            {
                using (var testable = new LintTestable())
                {
                    testable.GetMock<IJsonProvider>()
                        .Setup(x => x.SerializeOptions(null))
                        .Returns("null");

                    testable.Instance.Lint("some source");

                    testable.Verify<IJsonProvider>(x => x.DeserializeData(@"some source_null"));
                }
            }

            [Fact(DisplayName = "Should run JSLint over source with serialized options")]
            public void Spec02()
            {
                using (var testable = new LintTestable())
                {
                    var options = new JSLintOptions()
                    {
                        TolerateEval = true
                    };

                    testable.GetMock<IJsonProvider>()
                        .Setup(x => x.SerializeOptions(options))
                        .Returns(@"{""some"":""json""}");

                    testable.Instance.Lint("some source", options);

                    testable.Verify<IJsonProvider>(x => x.DeserializeData(@"some source_{""some"":""json""}"));
                }
            }

            [Fact(DisplayName = "Should return deserialized results")]
            public void Spec03()
            {
                using (var testable = new LintTestable())
                {
                    testable.GetMock<IJsonProvider>()
                        .Setup(x => x.SerializeOptions(null))
                        .Returns("null");

                    var actual = testable.Instance.Lint("some source");

                    testable.Verify<IJsonProvider>(x => x.DeserializeData(@"some source_null"));
                    I.Expect(actual).Not.ToBeNull();
                }
            }

            private class LintTestable : JSLintContextTestableBase
            {
            }
        }

        private abstract class JSLintContextTestableBase : TestFixture<JSLintContext>
        {
            public JSLintContextTestableBase()
            {
                this.ContextRuns = new List<string>();
                this.JavaScriptContextMock = new Mock<IJavaScriptContext>();
                this.Script = new ExpandoObject();
                this.Script.JSLintNet = new ExpandoObject();
                this.Script.JSLintNet.run = new Func<string, string, string>((x, y) => x + "_" + y);
            }

            public List<string> ContextRuns { get; private set; }

            public dynamic Script { get; private set; }

            public Mock<IJavaScriptContext> JavaScriptContextMock { get; private set; }

            protected override void BeforeResolve()
            {
                base.BeforeResolve();

                this.JavaScriptContextMock
                    .Setup(x => x.Run(It.IsAny<string>()))
                    .Callback((string x) => this.ContextRuns.Add(x))
                    .Returns(new Dictionary<string, object>());

                this.JavaScriptContextMock
                    .SetupGet(x => x.Script)
                    .Returns(this.Script as ExpandoObject);

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
