namespace JSLintNet.Specifications
{
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using Xunit;

    public class JSLintContextIntegration : IntegrationBase
    {
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

    var self = {
        memberProperty: 'value'
    };

    return {
        memberFunction: function (arg) {
            return self.memberProperty + arg;
        }
    };
}());");

                    I.Expect(result.Warnings).ToBeEmpty();
                }
            }

            [Fact(DisplayName = "Should populate directives")]
            public void Directives()
            {
                using (var instance = new JSLintContext())
                {
                    var result = instance.Lint(@"
/*jslint browser*/
/*global require, define*/
/*property act, value*/

define(['service', 'bag'], function (service, bag) {
    'use strict';

    return service.act(bag.value);
});");

                    I.Expect(result.Directives).Not.ToBeEmpty();
                    I.Expect(result.Directives[0].Directive).ToBe("jslint");
                    I.Expect(result.Directives[1].Directive).ToBe("global");
                    I.Expect(result.Directives[2].Directive).ToBe("property");
                }
            }

            [Fact(DisplayName = "Should produce a property directive")]
            public void PropertyDirective()
            {
                using (var instance = new JSLintContext())
                {
                    var result = instance.Lint(@"
var ValidSource = (function () {
    'use strict';

    var self = {
        memberProperty: 'value'
    };

    return {
        memberFunction: function (arg) {
            return self.memberProperty + arg;
        }
    };
}());");

                    I.Expect(result.PropertyDirective).ToStartWith("/*property");
                    I.Expect(result.PropertyDirective).ToContain("memberFunction, memberProperty");
                }
            }
        }
    }
}
