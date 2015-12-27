// jslintnet.js
/*global jslint, REPORT*/
var jslintnet = function (source, optionJson, globalJson) {
    'use strict';

    var optionObject = JSON.parse(optionJson);
    var globalArray = JSON.parse(globalJson);
    var data = jslint(source, optionObject || undefined, globalArray || undefined);

    return JSON.stringify({
        edition: data.edition,
        directives: data.directives,
        imports: data.imports,
        json: data.json,
        module: data.module,
        ok: data.ok,
        property: data.property,
        stop: data.stop,
        warnings: data.warnings,
        report: REPORT.error(data),
        propertyDirective: REPORT.property(data)
    });
};
