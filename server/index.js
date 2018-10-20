const randomKey =  require('./util/randomKey.js');

const Server = require('socket.io');
const PORT = 8080;
const http = require('http');
const bodyParser = require('body-parser');
const cors = require('cors');
const express = require('express');


const app = express();
const server = http.createServer(app);



app.use(cors());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());


app.get('/', function (req, res) {
    res.send('Hello World 222 !' + randomKey.get());
})

let io = Server(server);


io.on('connect_error', function (err) {
    console.log('Error connecting to server');
});

io.on('connection', function (socket) {
    socket.on('my other event', function (data) {
        console.log(data);
    });
    console.log('user connected')
});

server.listen(PORT, () => {
    console.log('Running server on port %s', PORT);
});



