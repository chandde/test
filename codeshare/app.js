const express = require('express');
const WebSocket = require('ws');
const _ = require('lodash');
const url = require('url');
const WsManager = require('./server/websocket.js');
const wsManager = new WsManager.WsManager();
const router = express.Router();

router.post('/newsession', function (req, res) {
  // TO DO: how to ensure it's really random?
  // at least check sql server to avoid dup? 
  console.log('received new session request');
  const sessionId = Math.random().toString(36).slice(2);
  console.log('respond with new session id ', sessionId);
  res.set({ 'Content-Type': 'application/json' });
  res.send({ newSessionId: sessionId });

  wsManager.createConnection(sessionId);
  console.log(`create wss handler for /newsession ${sessionId}`);
});

// subpages
// pad?id=123
router.get('/pad', (req, res) => {
  const connectionId = req.query['id'];
  console.log(`request for page ${connectionId} received`);
  if (connectionId && connectionId.length > 0) {
    wsManager.createConnection(connectionId);
    console.log(`create new wss for ${connectionId}`);
  }

  res.sendFile('pad.html', { root: './dist/' });
});

const app = express();
app.use(express.static('./dist/'));
app.use('/', router);

// node app.js 4001
// node argv[0]
// app.js argv[1]
// 40001 argv[2]
console.log(`listening on port ${process.argv[2]}`);
const httpServer = app.listen(process.argv[2], '0.0.0.0');

httpServer.on('upgrade', function upgrade(request, socket, head) {
  console.log(`received upgrade for ${request.url}`);
  const path = url.parse(request.url).pathname; // .substring(1);
  if (path && path.indexOf('/wss/') === 0) {
    const connectionId = path.substring(5);
    if (connectionId && connectionId.length > 0) {
      console.log(`dispatch upgrade for ${connectionId}`);
      wsManager.handleUpgrade(connectionId, request, socket, head);
    }
  } else {
    socket.destroy();
  }
});
