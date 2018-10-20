
const PORT=process.env.PORT || 8080;


const http = require('http');
const bodyParser = require('body-parser');
const cors = require('cors');
const express = require('express');
const setUpSocketIO = require('./socketIO/socketIO');

const app = express();
const server = http.createServer(app);

app.use(cors());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());


app.get('/', function (req, res) {
    res.send('Hello Server');
})

setUpSocketIO.setup(server);

server.listen(PORT, () => {
    console.log('Running server on port %s', PORT);
});



