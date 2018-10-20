const Server = require('socket.io');
const randomKey = require('../util/randomKey.js');
const countDownInterval = require('../util/countDownTimer.js');

const rooms = {};
const deadRooms = {};
let io;
let app;


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

function setUpListeners(socket) {

}


function setUpGameRoom(io, key, socket) {
    // rooms[key] = {
    //     created: Date.now(),
    //     state: "CREATED",
    //     emitter: io.sockets.in(key)
    // };



    socket.on(ON_EVENTS.broadcastLocalizationData, (dataStr) => {
        const data = JSON.parse(dataStr);

        room.emit(EMIT_EVENTS.dispactchLocalizationData, data);
    });

    socket.on(ON_EVENTS.setLocalizationData, (dataStr) => {
        const data = JSON.parse(dataStr);
        console.log("Localization Data: ", data);
        rooms[key].localizationData = data;
        room.emit(EMIT_EVENTS.dispactchLocalizationData, data);
    });

    socket.on(ON_EVENTS.receiveData, (dataStr) => {
        const data = JSON.parse(dataStr);
        console.log("Recieved data: ", data);
        rooms[key].data = data;
        room.emit(EMIT_EVENTS.sendData, data);
    });

    socket.on(ON_EVENTS.startGame, () => {
        rooms[key].state = "GAME STARTED";

        room.emit(EMIT_EVENTS.gameStarted);

        countDownInterval.countDown(60, (secondsLeft) => {
            room.emit(EMIT_EVENTS.timeLeft, secondsLeft);
            rooms[key].timeLeft = secondsLeft;

        }, () => {

            const collectedScores = [];

            room.on(ON_EVENTS.gatheringScores, (dataStr) => {
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

}

function emptyRoom(key) {
    io.sockets.clients(key).forEach((s) => {
        console.log(s);
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

function emitter(key) {
    return rooms[key].emitter;
}


function setupEnpoints(socket) {

    /**
     * 
     *     ******createRoom: 'createRoom',
    joinRoom: 'joinRoom',
    gatheringScores: 'gatheringScores',
    gameQuit: 'gameComplete',
    startGame: 'startGame',
    setLocalizationData: 'recieveLocalizationData',
    broadcastLocalizationData: 'broadcastLocalizationData',
    receiveData: 'recieveData',
    leaveRoom: ************ 'leaveRoom'
     * 
     */

    socket.on('createRoom', (data) => {
        console.log(data);
        const userData = JSON.parse(data);
        const key = randomKey.get();
        console.log('Room being created with key: ', key);


        rooms[key] = {
            created: Date.now(),
            state: "CREATED",
            emitter: io.sockets.in(key)
        };


        // setUpGameRoom(io, key, socket);
        userData.roomKey = key;
        // res.json({ key: key });
        joinLogic(socket, userData);
    })

    socket.on('/leaveRoom', (data) => {
        const userData = JSON.parse(data);
        const key = userData.roomKey;
        console.log('Leave room', userStr);
        emitter(key).emit(EMIT_EVENTS.memberDropped, userData);
        removeUserFromRoom(key, userData.uid);
    })

    socket.on('/endGame', (data) => {
        const userData = JSON.parse(data);
        const key = userData.roomKey;

        rooms[key].state = "GAME QUIT";

        deadRooms[key] = {
            ...rooms[key]
        }

        delete rooms[key];

        console.log('Game Quit');
        emptyRoom(key);
    });


    socket.on('/broadcastData', (dataStr) => {
        const data = JSON.parse(dataStr);
        const key = data.roomKey;
        emitter(key).emit(EMIT_EVENTS.sendData, data);
    });

    socket.on('/startGame', (dataStr) => {
        const data = JSON.parse(dataStr);
        const key = data.roomKey;

        rooms[key].state = "GAME STARTED";

        emitter(key).emit(EMIT_EVENTS.gameStarted);

        countDownInterval.countDown(60, (secondsLeft) => {
            emitter(key).emit(EMIT_EVENTS.timeLeft, secondsLeft);
            rooms[key].timeLeft = secondsLeft;

        }, () => {
            console.log('all done!')
            // const collectedScores = [];

            // room.on(ON_EVENTS.gatheringScores, (dataStr) => {
            //     const data = JSON.parse(dataStr);
            //     const score = data.score;
            //     const userName = data.userName;
            //     const uid = data.uid;
            //     collectedScores.push({
            //         score: score,
            //         userName: userName,
            //         uid: uid
            //     });
            // })

            // room.emit(EMIT_EVENTS.gameOver);

            // countDownInterval.countDown(5, () => {
            // }, () => {
            //     room.emit(EMIT_EVENTS.finalScores, collectedScores);
            //     emptyRoom(io, key);
            //     rooms[key].state = "GAME ENDED";

            //     deadRooms[key] = {
            //         collectedScores: collectedScores,
            //         ...rooms[key]
            //     }

            //     delete rooms[key];
            // })

        })


    });




}


const setUpSocketIO = (server, appParam) => {
    app = appParam;
    io = Server(server, { pingInterval: 500 });

    io.on('connect_error', (err) => {
        console.log('Error connecting to server');
    });

    io.on('connection', (socket) => {

        socket.on(ON_EVENTS.joinRoom, (userDataStr) => {
            const userData = JSON.parse(userDataStr);
            joinLogic(socket, userData);
        });

        setupEnpoints(socket);


        console.log('user connected', socket.id);
    });

    return io;

}

function joinLogic(socket, userData) {
    const key = userData.roomKey;
    console.log('Joining room with key: ', key);
    socket.join(key);

    const localizationData = rooms[key].localizationData;
    const payload = { key: key };
    if (localizationData) {
        payload.localizationData = localizationData;
    }
    emitter(key).emit(EMIT_EVENTS.joinedRoom, payload);

    trackUsersPerRoom(key, userData);
    emitter(key).emit(EMIT_EVENTS.newMemberJoined, userData);

    socket.on('disconnect', () => {
        emitter(key).emit(EMIT_EVENTS.memberDropped, userData);
        removeUserFromRoom(key, userData)
    });

}

module.exports = {
    setup: (server, appParam) => {
        setUpSocketIO(server, appParam);
    },
    activeRooms: () => rooms,
    deadRooms: () => deadRooms
}