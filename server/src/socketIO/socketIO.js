const Server = require('socket.io');
const randomKey = require('../util/randomKey.js');
const countDownInterval = require('../util/countDownTimer.js');

const rooms = {};
const deadRooms = {};
let io;
let app;
let globalScoreTracker = {};


const ON_EVENTS = {
    createRoom: 'createRoom',
    joinRoom: 'joinRoom',
    gatheringScores: 'gatheringScores',
    stopGame: 'stopGame',
    startGame: 'startGame',
    setLocalizationData: 'recieveLocalizationData',
    broadcastLocalizationData: 'broadcastLocalizationData',
    receiveData: 'recieveData',
    leaveRoom: 'leaveRoom',
    broadcastData: 'broadcastData',
    userReady: 'userReady',
    requestLocalizationData: 'requestLocalizationData'

}

const EMIT_EVENTS = {
    joinedRoom: 'joinedRoom',
    gameStarted: 'gameStarted',
    finalScores: 'finalScores',
    timeLeft: 'timeLeft',
    gameOver: 'gameOver',
    broadcastLocalizationData: 'broadcastLocalizationData',
    newMemberJoined: 'newMemberJoined',
    memberDropped: 'memberDropped',
    broadcastData: 'broadcastData',
    invalidKey: 'invalidKey',
    usersStatusChange: 'usersStatusChange'
}


function emptyRoom(key) {
    io.sockets.clients(key).forEach((s) => {
        console.log(s);
        s.leave(key);
    });
}

function trackUsersPerRoom(key, user) {
    if(!rooms[key].users){
        rooms[key].users = {}
    }
    rooms[key].users[user.uid] = user;
}

function removeUserFromRoom(key, uid) {
    if(rooms[key].users){
        delete rooms[key].users[uid];
    }
}

function emitter(key) {
    return rooms[key].emitter();
}


function setupChannels(socket) {

    socket.on(ON_EVENTS.createRoom, (data) => {
        const userData = JSON.parse(data);
        const key = randomKey.get();
        console.log('Room being created with key: ', key);
        rooms[key] = {
            created: Date.now(),
            state: "CREATED",
            emitter: () => io.sockets.in(key)
        };
        userData.roomKey = key;
        joinLogic(socket, userData);
    })

    socket.on(ON_EVENTS.joinRoom, (userDataStr) => {
        const userData = JSON.parse(userDataStr);
        joinLogic(socket, userData);
    });
    socket.on(ON_EVENTS.userReady, (userDataStr) => {
        const userData = JSON.parse(userDataStr);
        rooms[key].users[userData.uid].ready = true;
        emitter(key).emit(EMIT_EVENTS.usersStatusChange, rooms[key].users);
    });

    
    socket.on(ON_EVENTS.leaveRoom, (data) => {
        const userData = JSON.parse(data);
        const key = userData.roomKey;
        console.log('Leave room', userData);
        if (userData.isHost) {
            emptyRoom(key);
        } else {
            emitter(key).emit(EMIT_EVENTS.memberDropped, userData);
            removeUserFromRoom(key, userData.uid);
            socket.leave(key);
        }

    })

    socket.on(ON_EVENTS.stopGame, (data) => {
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


    socket.on(ON_EVENTS.broadcastLocalizationData, (dataStr) => {
        const data = JSON.parse(dataStr);
        const key = data.roomKey;
        emitter(key).emit(EMIT_EVENTS.broadcastLocalizationData, data.achor);
    });

    socket.on(ON_EVENTS.requestLocalizationData, (dataStr) => {
        const data = JSON.parse(dataStr);
        const key = data.roomKey;
        emitter(key).emit(EMIT_EVENTS.broadcastLocalizationData, rooms[key].localizationData);
    });
    

    socket.on(ON_EVENTS.setLocalizationData, (dataStr) => {
        const data = JSON.parse(dataStr);
        const key = data.roomKey;
        console.log("Localization User Data: ", data);
        rooms[key].localizationData = data.achor;
        emitter(key).emit(EMIT_EVENTS.broadcastLocalizationData, data.achor);
    });

    socket.on(ON_EVENTS.broadcastData, (dataStr) => {
        const data = JSON.parse(dataStr);
        const key = data.roomKey;
        emitter(key).emit(EMIT_EVENTS.broadcastData, data);
    });

    socket.on(ON_EVENTS.gatheringScores, (dataStr) => {
        const data = JSON.parse(dataStr);
        const key = data.roomKey;
        const uid = data.uid;
        globalScoreTracker[key][uid] = data;
    })

    socket.on(ON_EVENTS.startGame, (dataStr) => {
        const data = JSON.parse(dataStr);
        const key = data.roomKey;

        rooms[key].state = "GAME STARTED";

        emitter(key).emit(EMIT_EVENTS.gameStarted);

        countDownInterval.countDown(60, (secondsLeft) => {
            emitter(key).emit(EMIT_EVENTS.timeLeft, secondsLeft);
            rooms[key].timeLeft = secondsLeft;

        }, () => {
            console.log('all done!')

            room.emit(EMIT_EVENTS.gameOver);

            countDownInterval.countDown(5, () => {
            }, () => {
                room.emit(EMIT_EVENTS.finalScores, globalScoreTracker[key]);
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


const setUpSocketIO = (server, appParam) => {
    app = appParam;
    io = Server(server, { pingInterval: 500 });

    io.on('connect_error', (err) => {
        console.log('Error connecting to server');
    });

    io.on('connection', (socket) => {
        setupChannels(socket);
        console.log('user connected', socket.id);
    });

    return io;

}

function joinLogic(socket, userData) {
    const key = userData.roomKey;

    if (rooms.hasOwnProperty(key)) {
        console.log('Joining room with key: ', key);
        socket.join(key);

        const localizationData = rooms[key].localizationData;
        const payload = { roomKey: key };
        if (localizationData) {
            payload.localizationData = localizationData;
        }
        emitter(key).emit(EMIT_EVENTS.joinedRoom, payload);

        trackUsersPerRoom(key, userData);
        emitter(key).emit(EMIT_EVENTS.newMemberJoined, userData);

        socket.on('disconnect', () => {
            emitter(key).emit(EMIT_EVENTS.memberDropped, userData);
            removeUserFromRoom(key, userData.uid)
        });

    } else {
        socket.emit(EMIT_EVENTS.invalidKey, "Invalid room key");
    }


}

module.exports = {
    setup: (server, appParam) => {
        setUpSocketIO(server, appParam);
    },
    activeRooms: () => rooms,
    deadRooms: () => deadRooms
}