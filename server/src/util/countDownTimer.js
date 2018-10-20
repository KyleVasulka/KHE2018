
module.exports = {
    countDown = (seconds, body, end) => {
        let i = seconds;
        const interval = setInterval(function () {
            body(i);
            if (--i === 0) {
                clearInterval(interval);
                end();
            }
        }, 1000);
        return interval;
    }
}