const express = require('express');
const WebSocket = require('ws');
const _ = require('lodash');
const url = require('url');

// const wss = new WebSocket.Server({ port: 4010 });

// let cache;

const wsMap = {};
const cache = {};

// wss.on('connection', function connection(ws) {
//   if (!_.find(wss.clients, client => client === ws)) {
//     // new client, send cache to help it up to speed
//     if (cache) {
//       ws.send(cache);
//     }
//   }

//   ws.on('message', function incoming(data) {
//     cache = data;
//     wss.clients.forEach(function each(client) {
//       if (client !== ws && client.readyState === WebSocket.OPEN) {
//         client.send(data);
//       }
//     });
//   });
// });

const router = express.Router();
const app = express();

// let httpServer;

app.use(express.static('./dist/'));

// homepage
router.get('/', (req, res) => {
  res.sendFile('index.html', { root: './dist/' });
});

// subpages
router.get('/*', (req, res) => {
  const chatId = req.originalUrl;
  console.log(`request for page ${chatId} received`);
  if (!wsMap[chatId]) {
    console.log(`create new wss for ${chatId}`);
    wsMap[chatId] = new WebSocket.Server({ noServer: true });
    wsMap[chatId].on('connection', function connection(ws) {
      if (!_.find(wsMap[chatId].clients, client => client === ws)) {
        // new client, send cache to help it up to speed
        if (cache[chatId]) {
          console.log(`found cache for ${chatId}, sending ${cache[chatId]}`);
          ws.send(cache[chatId]);
        }
      }
    
      ws.on('message', function incoming(data) {
        console.log(`received new data for ${chatId}: ${data}`);
        cache[chatId] = data;
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

app.use('/', router);

const httpServer = app.listen(4000);

httpServer.on('upgrade', function upgrade(request, socket, head) {
  console.log(request);
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
