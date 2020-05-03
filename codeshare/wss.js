const WebSocket = require('ws');

export class WSS {
  start() {
    this.wss = new WebSocket.Server({ port: 3010 });

    this.on('connection', function connection(ws) {
      ws.on('message', function incoming(message) {
        console.log('received from client', message);
        ws.send('ack');
      });

      ws.send('hello from server');
    });
  }
}