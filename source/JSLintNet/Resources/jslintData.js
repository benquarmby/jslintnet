(function () {
    'use strict';

    var data = JSLINT.data();

    return JSON.stringify({
        errors: data.errors,
        functions: data.functions,
        global: data.global,
        json: data.json,
        error_report: JSLINT.error_report(data)
    });
}());
