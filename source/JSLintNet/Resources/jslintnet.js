// jslintnet.js
/*global JSLINT*/
var JSLintNet = (function () {
    'use strict';

    return {
        run: function (source, options) {
            JSLINT(source, JSON.parse(options));

            var data = JSLINT.data();

            return JSON.stringify({
                errors: data.errors,
                functions: data.functions,
                global: data.global,
                json: data.json,
                error_report: JSLINT.error_report(data)
            });
        }
    };
}());
