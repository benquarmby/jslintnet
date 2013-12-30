var Calculator;

Calculator = (function () {
    'use strict';

    function add(first, second) {
        return first + second;
    }

    function subtract(first, second) {
        return first - second;
    }

    function multiply(first, second) {
        return first * second;
    }

    function divide(first, second) {
        return first / second;
    }

    return {
        add: add,
        subtract: subtract,
        multiply: multiply,
        divide: divide
    };
}());