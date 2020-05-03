// const WebSocket = require('ws');

// // create a id/wss map
// export function createWsServer() {
//     const map = new Map();

//     const wss = new WebSocket.Server({ noServer: true });

//     let cache;

//     wss.on('connection', function connection(ws) {
//         if (!_.find(wss.clients, client => client === ws)) {
//             // new client, send cache to help it up to speed
//             if (cache) {
//                 ws.send(cache);
//             }
//         }

//         ws.on('message', function incoming(data) {
//             cache = data;
//             wss.clients.forEach(function each(client) {
//                 if (client !== ws && client.readyState === WebSocket.OPEN) {
//                     client.send(data);
//                 }
//             });
//         });
//     });
// }