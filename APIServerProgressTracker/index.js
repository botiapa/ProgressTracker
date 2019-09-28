const listenPort = 8000;

const express = require('express');
const bodyParser = require('body-parser');

const app = express();
app.use(express.json()) // for parsing application/json
app.use(express.urlencoded({ extended: true })) // for parsing application/x-www-form-urlencoded

const dbconn = require('./database')();

const wsHandler = require('./websocket');
wsHandler.init(dbconn);

const routes = require('./routes')(app, wsHandler, dbconn);
app.listen(listenPort, () => {  console.log('We are live on ' + listenPort);});


