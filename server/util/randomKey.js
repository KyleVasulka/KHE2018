const get = function (size = 4) {
    const key = Math.random().toString(36).substring(0, size).toString().trim();
    return key;
}

module.exports = {
    get: get
};