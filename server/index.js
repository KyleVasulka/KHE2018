const randomKey = require('./util/randomKey.js');

const Server = require('socket.io');
const PORT = 8080;
const http = require('http');
const bodyParser = require('body-parser');
const cors = require('cors');
const express = require('express');


const app = express();
const server = http.createServer(app);
let io = Server(server);



app.use(cors());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());


const EVENTS = {
    createRoom: 'createRoom',
    roomJoined: 'roomJoined',
    joinRoom: 'joinRoom',
    waitingRoomTimer: 'waitingRoomTimer'
}

app.get('/', function (req, res) {
    res.send('Hello Server');
})


io.on('connect_error', function (err) {
    console.log('Error connecting to server');
});

io.on('connection', function (socket) {


    socket.on(EVENTS.createRoom, function () {
        const key = randomKey.get();
        console.log('Room being created with key: ', key);

        socket.join(key);
        socket.emit(EVENTS.roomJoined, { key: key });

        let i = 0;
        setInterval(function () {
            io.sockets.in(key).emit(EVENTS.waitingRoomTimer, i++);
        }, 1000);


    });

    socket.on(EVENTS.joinRoom, function (key) {
        const trimedKey = key.trim();
        console.log('Joining room with key: ', trimedKey);
        socket.join(trimedKey);
        socket.emit(EVENTS.roomJoined, { key: trimedKey });
    });

    console.log('user connected')
});


server.listen(PORT, () => {
    console.log('Running server on port %s', PORT);
});



