const Server = require('socket.io');
const randomKey = require('../util/randomKey.js');
const countDownInterval = require('../util/countDownTimer.js');


const rooms = {};
const deadRooms = {};


const ON_EVENTS = {
    createRoom: 'createRoom',
    joinRoom: 'joinRoom',
    gatheringScores: 'gatheringScores',
    gameQuit: 'gameComplete',
    startGame: 'startGame'
}

const EMIT_EVENTS = {
    joinedRoom: 'joinedRoom',
    gameStarted: 'gameStarted',
    finalScores: 'finalScores',
    timeLeft: 'timeLeft',
    gameOver: 'gameOver'
}




function setUpGameRoom(io, key) {
    const room = io.sockets.in(key);

    rooms[key] = {
        created: Date.now(),
        state: "CREATED"
    };

    room.on(ON_EVENTS.startGame, function () {
        rooms[key].state = "GAME STARTED";

        room.emit(EMIT_EVENTS.gameStarted);

        countDownInterval.countDown(60, (secondsLeft) => {
            room.emit(EMIT_EVENTS.timeLeft, secondsLeft);
            rooms[key].timeLeft = secondsLeft;

        }, () => {

            const collectedScores = [];

            room.on(ON_EVENTS.gatheringScores, function (data) {
                const score = data.score;
                const userName = data.userName;
                const uid = data.uid;
                collectedScores.push({
                    score: score,
                    userName: userName,
                    uid: uid
                });
            })

            room.emit(EMIT_EVENTS.gameOver);

            countDownInterval.countDown(5, (timeLeft) => {
            }, () => {
                room.emit(EMIT_EVENTS.finalScores, collectedScores);
                emptyRoom(io, key);
                rooms[key].state = "GAME ENDED";

                deadRooms[key] = {
                    collectedScores: collectedScores,
                    ...rooms[key]
                }

                delete rooms[key];

            })

        })


    });

    room.on(ON_EVENTS.gameQuit, function (payload) {

        rooms[key].state = "GAME QUIT";

        deadRooms[key] = {
            ...rooms[key]
        }

        delete rooms[key];

        console.log('Game Quit');
        emptyRoom(io, key);
    })
}

function emptyRoom(io, key) {
    io.sockets.clients(key).forEach(function (s) {
        s.leave(key);
    });
}

const setUpSocketIO = function (server) {


    let io = Server(server, { pingInterval: 500 });

    io.on('connect_error', function (err) {
        console.log('Error connecting to server');
    });

    io.on('connection', function (socket) {

        socket.on(ON_EVENTS.createRoom, function () {
            const key = randomKey.get();
            console.log('Room being created with key: ', key);
            socket.join(key);
            socket.emit(EMIT_EVENTS.joinedRoom, { key: key });
            setUpGameRoom(io, key);
        });

        socket.on(ON_EVENTS.joinRoom, function (key) {
            const trimedKey = key.trim();
            console.log('Joining room with key: ', trimedKey);
            socket.join(trimedKey);
            socket.emit(EMIT_EVENTS.joinedRoom, { key: trimedKey });
        });

        socket.on('disconnect', function () {
            socket.emit('disconnected');
            console.log("connected socket", JSON.stringify(socket.id));
        });

        console.log('user connected')
    });

    return io;

}
module.exports = {
    setup: setUpSocketIO,
    activeRooms: () => rooms,
    deadRooms: () => deadRooms
}