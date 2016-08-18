'use strict';
capsCollectionApp.filter('zpad', function () {

    function repeat(pattern, count) {
        if (count < 1) return '';
        var result = '';
        while (count > 1) {
            if (count & 1) result += pattern;
            count >>= 1, pattern += pattern;
        }
        return result + pattern;
    }

    return function (input, n) {
        if (input === undefined)
            input = "";
        if (input.length >= n)
            return input;
        var zeros = repeat('0', 5);
        return (zeros + input).slice(-1 * n);
    };
});
