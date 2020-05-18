const express = require('express');
const WebSocket = require('ws');
const _ = require('lodash');
const url = require('url');
const WsManager = require('./server/websocket.js');
const wsManager = new WsManager.WsManager();
const router = express.Router();

// homepage
router.get('/home', (req, res) => {
  console.log('received request for homepage');
  res.sendFile('home.html', { root: './dist/' });
});

// subpages
router.get('/*', (req, res) => {
  const chatId = req.originalUrl.substring(1);
  console.log(`request for page ${chatId} received`);
  if (chatId && !wsManager[chatId] && chatId != 'home' && chatId != 'favicon.ico') {
    wsManager.createConnection(chatId); 
    console.log(`create new wss for ${chatId}`);
  }

  res.sendFile('index.html', { root: './dist/' });
});

router.post('/newsession', function (req, res) {
  // TO DO: how to ensure it's really random?
  // at least check sql server to avoid dup? 
  console.log('received new session request');
  const sessionId = Math.random().toString(36).slice(2);
  console.log('respond with new session id ', sessionId);
  res.set({ 'Content-Type': 'application/json' });
  res.send({ newSessionId: sessionId });

  // TO DO should we instantiate the ws server in advance
});

const app = express();
app.use(express.static('./dist/'));
app.use('/', router);

const httpServer = app.listen(process.env.PORT);

httpServer.on('upgrade', function upgrade(request, socket, head) {
  const chatId = url.parse(request.url).pathname.substring(1);
  console.log(`received upgrade for ${chatId}`);
  if (chatId && chatId != 'home' && chatId != 'favicon.ico') {
    console.log(`dispatch connection event for ${chatId}`);
    wsManager.handleUpgrade(chatId, request, socket, head);
  } else {
    socket.destroy();
  }
});
