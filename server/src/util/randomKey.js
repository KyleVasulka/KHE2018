const get = function (size = 4) {
    var anysize = 4;//the size of string 
    var charset = "abcdefghijklmnopqrstuvwxyz1234567890"; //from where to create
    result = "";
    for (var i = 0; i < anysize; i++)
        result += charset[Math.floor(Math.random() * charset.length)];
    console.log(result);
    return result;
}

module.exports = {
    get: get
};