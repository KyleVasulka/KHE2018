const get = function (size = 3) {
    return Math.random().toString(36).substring(0, size);
}

module.exports = {
    get: get
};