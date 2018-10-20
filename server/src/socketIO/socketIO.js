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
    startGame: 'startGame',
    setLocalizationData: 'recieveLocalizationData',
    broadcastLocalizationData: 'broadcastLocalizationData',
    receiveData: 'recieveData',
    leaveRoom: 'leaveRoom'
}

const EMIT_EVENTS = {
    joinedRoom: 'joinedRoom',
    gameStarted: 'gameStarted',
    finalScores: 'finalScores',
    timeLeft: 'timeLeft',
    gameOver: 'gameOver',
    dispactchLocalizationData: 'dispactchLocalizationData',
    newMemberJoined: 'newMemberJoined',
    memberDropped: 'memberDropped',
    sendData: 'sendData'
}


function setUpGameRoom(io, key, socket) {
    const room = io.sockets.in(key);

    rooms[key] = {
        created: Date.now(),
        state: "CREATED"
    };



    socket.on(ON_EVENTS.leaveRoom, function (userStr) {
        console.log('Leave room', userStr);
        const user = JSON.parse(userStr);
        const key = user.currentRoomKey;
        room.emit(EMIT_EVENTS.memberDropped, user);
        removeUserFromRoom(key, user.uid);
        socket.leave(key);
    });


    socket.on(ON_EVENTS.broadcastLocalizationData, function (dataStr) {
        const data = JSON.parse(dataStr);

        room.emit(EMIT_EVENTS.dispactchLocalizationData, data);
    });

    socket.on(ON_EVENTS.setLocalizationData, function (dataStr) {
        const data = JSON.parse(dataStr);
        console.log("Localization Data: ", data);
        rooms[key].localizationData = data;
        room.emit(EMIT_EVENTS.dispactchLocalizationData, data);
    });

    socket.on(ON_EVENTS.receiveData, function (dataStr) {
        const data = JSON.parse(dataStr);
        console.log("Recieved data: ", data);
        rooms[key].data = data;
        room.emit(EMIT_EVENTS.sendData, data);
    });

    socket.on(ON_EVENTS.startGame, function () {
        rooms[key].state = "GAME STARTED";

        room.emit(EMIT_EVENTS.gameStarted);

        countDownInterval.countDown(60, (secondsLeft) => {
            room.emit(EMIT_EVENTS.timeLeft, secondsLeft);
            rooms[key].timeLeft = secondsLeft;

        }, () => {

            const collectedScores = [];

            room.on(ON_EVENTS.gatheringScores, function (dataStr) {
                const data = JSON.parse(dataStr);
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

            countDownInterval.countDown(5, () => {
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

    room.on(ON_EVENTS.gameQuit, function (dataStr) {
        const data = JSON.parse(dataStr);

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

function trackUsersPerRoom(key, user) {
    const users = rooms[key].users;
    if (users && users.length) {
        users.push(user);
    } else {
        rooms[key].users = [user]
    }
}

function removeUserFromRoom(key, uid) {
    const users = rooms[key].users;
    if (users && users.length) {
        rooms[key].users = users.filter(item => item.uid !== uid);
    } else {
        console.log("no user to remove");
    }
}


const setUpSocketIO = function (server) {
    let io = Server(server, { pingInterval: 500 });

    io.on('connect_error', function (err) {
        console.log('Error connecting to server');
    });

    io.on('connection', function (socket) {

        socket.on(ON_EVENTS.createRoom, function (userDataStr) {
            const userData = JSON.parse(userDataStr);
            const key = randomKey.get();
            console.log('Room being created with key: ', key);
            setUpGameRoom(io, key, socket);
            userData.currentRoomKey = key;
            joinLogic(socket, userData, io);
        });

        socket.on(ON_EVENTS.joinRoom, function (userDataStr) {
            const userData = JSON.parse(userDataStr);
            joinLogic(socket, userData, io);
        });


        console.log('user connected', socket.id);
    });

    return io;

}

function joinLogic(socket, userData, io) {
    const key = userData.currentRoomKey;
    console.log('Joining room with key: ', key);
    socket.join(key);

    const localizationData = rooms[key].localizationData;
    const payload = { key: key };
    if (localizationData) {
        payload.localizationData = localizationData;
    }
    socket.emit(EMIT_EVENTS.joinedRoom, payload);

    trackUsersPerRoom(key, userData);
    io.sockets.in(key).emit(EMIT_EVENTS.newMemberJoined, userData);

    socket.on('disconnect', () => {
        io.sockets.in(key).emit(EMIT_EVENTS.memberDropped, userData);
        removeUserFromRoom(key, userData)
    });

}

module.exports = {
    setup: setUpSocketIO,
    activeRooms: () => rooms,
    deadRooms: () => deadRooms
}