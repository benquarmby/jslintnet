namespace JSLintNet.Specifications
{
    using System.IO;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using Xunit;

    public class JSLintContextIntegration : IntegrationBase
    {
        [Collection("JSLintContextIntegration")]
        public class Constructor : IntegrationBase
        {
            [Fact(DisplayName = "Should throw given old jslint edition on file system")]
            public void Spec01()
            {
                try
                {
                    ConstructorHelper.CreateJSLint("var JSLINT = { edition: '2000-01-01' };");

                    var exception = I.Expect(() =>
                    {
                        using (new JSLintContext())
                        {
                        }
                    }).ToThrow();

                    I.Expect(exception.Message).ToContain("edition");
                }
                finally
                {
                    ConstructorHelper.DeleteJSLint();
                }
            }

            [Fact(DisplayName = "Should throw given invalid jslint from file system")]
            public void Spec02()
            {
                try
                {
                    ConstructorHelper.CreateJSLint("not.actually.jslint;");

                    using (new JSLintContext())
                    {
                    }

                    Assert.True(false, "Expected exception given invalid JSLint file.");
                }
                catch
                {
                }
                finally
                {
                    ConstructorHelper.DeleteJSLint();
                }
            }

            [Fact(DisplayName = "Should not throw given new jslint edition from file system")]
            public void Spec03()
            {
                try
                {
                    ConstructorHelper.CreateJSLint("var JSLINT = { edition: '3000-01-01' };");

                    using (new JSLintContext())
                    {
                    }
                }
                finally
                {
                    ConstructorHelper.DeleteJSLint();
                }
            }

            [Fact(DisplayName = "Should not throw given equal jslint edition from file system")]
            public void Spec04()
            {
                try
                {
                    ConstructorHelper.CreateJSLint(string.Format("var JSLINT = {{ edition: '{0}' }};", AssemblyInfo.Edition));

                    using (new JSLintContext())
                    {
                    }
                }
                finally
                {
                    ConstructorHelper.DeleteJSLint();
                }
            }

            private static class ConstructorHelper
            {
                public static readonly string JSLintPath = GetJSLintPath();

                public static string GetJSLintPath()
                {
                    var path = typeof(JSLintContext).Assembly.Location;
                    path = Path.GetDirectoryName(path);
                    path = Path.Combine(path, "jslint.js");

                    return path;
                }

                public static void CreateJSLint(string source)
                {
                    File.WriteAllText(JSLintPath, source);
                }

                public static void DeleteJSLint()
                {
                    File.Delete(JSLintPath);
                }
            }
        }

        [Collection("JSLintContextIntegration")]
        public class Lint : IntegrationBase
        {
            [Fact(DisplayName = "Should fail given invalid source")]
            public void Spec01()
            {
                using (var instance = new JSLintContext())
                {
                    var result = instance.Lint("some invalid source");

                    I.Expect(result.Warnings).Not.ToBeEmpty();
                }
            }

            [Fact(DisplayName = "Should have stopping error when maximum reached")]
            public void Spec02()
            {
                using (var instance = new JSLintContext())
                {
                    var result = instance.Lint(
                        "var GLOBAL VARIABLE",
                        new JSLintOptions()
                        {
                            MaximumErrors = 1
                        });

                    I.Expect(result.Warnings.Count).ToBe(2);
                    I.Expect(result.Stop).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should succeed given valid source")]
            public void Spec04()
            {
                using (var instance = new JSLintContext())
                {
                    var result = instance.Lint(@"
var ValidSource = (function () {
    'use strict';

    return {
        memberProperty: 'value',
        memberFunction: function (arg) {
            return this.memberProperty + arg;
        }
    };
}());");

                    I.Expect(result.Warnings).ToBeEmpty();
                }
            }
        }
    }
}
