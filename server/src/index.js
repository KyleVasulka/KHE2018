
const PORT = process.env.PORT || 8080;


const http = require('http');
const bodyParser = require('body-parser');
const cors = require('cors');
const express = require('express');
const socketIO = require('./socketIO/socketIO');

const app = express();
const server = http.createServer(app);

app.use(cors());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());


app.get('/', function (req, res) {
    res.send('Hello Server');
})

socketIO.setup(server);

app.get('/activeRooms', function (req, res) {
    res.send(socketIO.activeRooms());
})

app.get('/deadRooms', function (req, res) {
    res.send(socketIO.deadRooms());
})


server.listen(PORT, () => {
    console.log('Running server on port %s', PORT);
});





