window.timeHelper = {
    getOffsetMinutes: function () {
        return -new Date().getTimezoneOffset(); // negative because JS is inverted vs .NET
    }
};