const express = require('express');
const WebSocket = require('ws');

const wss = new WebSocket.Server({ port: 4010 });

wss.on('connection', (wss) => {
  wss.on('message', (message) => {
    console.log('received from client', message);
    wss.send('ack');
  });

  // wss.send('hello from server');
});

const router = express.Router();
const app = express();

app.use(express.static('./dist/'));

router.get('/', (req, res) => {
  res.sendFile('index.html', { root: './dist/' });
});

app.use('/', router);
app.listen(4000);
