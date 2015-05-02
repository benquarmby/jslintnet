// jslintnet.js
/*global jslint, REPORT*/
var jslintnet = function (source, options, globals) {
    'use strict';

    var data = jslint(source, JSON.parse(options), JSON.parse(globals));

    return JSON.stringify({
        warnings: data.warnings,
        functions: data.functions,
        global: data.global,
        json: data.json,
        stop: data.stop,
        report: REPORT.error(data)
    });
};
