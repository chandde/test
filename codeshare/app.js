const express = require('express');
const WebSocket = require('ws');
const _ = require('lodash');
const url = require('url');

// redis cache
const redis = require("redis");
const redisClient = redis.createClient(6379, '192.168.1.20');
redisClient.set('test', 'not hello world');

// mysql
var mysql      = require('mysql');
var mysqlConnection = mysql.createConnection({
  host     : '192.168.1.20',
  user     : 'codeshare1',
  password : '123456',
  database : 'codeshare'
});

mysqlConnection.connect(function(err) {
  if (err) {
    console.error('error connecting: ' + err.stack);
    return;
  }

  console.log('connected to mysql as id ' + mysqlConnection.threadId);
});

const wsMap = {};
// const cache = {};

const router = express.Router();

// homepage
router.get('/home', (req, res) => {
  console.log('received request for homepage');
  res.sendFile('home.html', { root: './dist/' });
});

// subpages
router.get('/*', (req, res) => {
  const chatId = req.originalUrl;
  console.log(`request for page ${chatId} received`);
  if (!wsMap[chatId]) {
    console.log(`create new wss for ${chatId}`);
    // check if we have cache for chatId, if not, check DB
    // redisClient.get(chatId, (err, value) => {
    //   if (err) {
    //     console.log(`cannot find cache for ${chatId}, query sql`);
    //     // redis cache miss, check if we have the record in DB
    //     mysqlConnection.query({sql: `SELECT * FROM codeshare.codeshare WHERE id = ${chatId}`}, (error, results, fields) => {
    //       console.log(error);
    //       console.log(results);
    //       console.log(fields);
    //     });
    //   }
    //   if (value) {
    //     console.log(`found cache for ${chatId} in redis, no-op`);
    //     // ws.send(value);
    //   }
    // });
    wsMap[chatId] = new WebSocket.Server({ noServer: true });
    wsMap[chatId].on('connection', function connection(ws) {
      if (!_.find(wsMap[chatId].clients, client => client === ws)) {
        // new client, send cache to help it up to speed, it's guaranteed the cache exists
        redisClient.get(chatId, (err, value) => {
          // if (err) {
          //   console.log(`did not find cache for ${chatId} in redis`);
          //   // redis cache miss, check if we have the record in DB
          //   mysqlConnection.query({sql: `SELECT * FROM codeshare.codeshare WHERE id = ${chatId}`}, (error, results, fields) => {
          //     console.log(error);
          //     console.log(results);
          //     console.log(fields);
          //   });
          // }
          if (value) {
            console.log(`found cache for ${chatId} in redis, sending ${value}`);
            ws.send(value);
          }
        });
      }
    
      ws.on('message', function incoming(data) {
        console.log(`received new data for ${chatId}: ${data}`);
        redisClient.set(chatId, data);
        wsMap[chatId].clients.forEach(function each(client) {
          console.log(`broadcast new data for ${chatId}: ${data}`);
          if (client !== ws && client.readyState === WebSocket.OPEN) {
            client.send(data);
          }
        });
      });
    });
  }
  res.sendFile('index.html', { root: './dist/' });
});

router.post('/newsession', function (req, res) {
  // generate a new random session id, how to ensure it's really random?
  console.log('received new session request');
  const sessionId = Math.random().toString(36).slice(2);
  console.log('respond with new session id ', sessionId);
  res.set({ 'Content-Type': 'application/json' });
  res.send({ newSessionId: sessionId });
});

const app = express();
app.use(express.static('./dist/'));
app.use('/', router);

const httpServer = app.listen(4000);

httpServer.on('upgrade', function upgrade(request, socket, head) {
  // console.log(request);
  const pathname = url.parse(request.url).pathname;
  console.log(`received upgrade for ${pathname}`);
  if (pathname && pathname !== '/') {    
    console.log(`dispatch connection event for ${pathname}`);
    wsMap[pathname].handleUpgrade(request, socket, head, function done(ws) {
      wsMap[pathname].emit('connection', ws, request);
    });    
  } else {
    socket.destroy();
  }
});
