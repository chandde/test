const _ = require('lodash');
const WebSocket = require('ws');
const Cache = require('./redis.js');
const Database = require('./mysql.js');

exports.WsManager = class WsManager {
  constructor() {
    console.log('wsmanager ctor');
    this.cache = new Cache.Cache();
    this.database = new Database.Database();
    this.wsMap = {};
  }

  createConnection(connectionId) {
    console.log(`WsManager.createConnection for ${connectionId}`);
    // ignore existing chats
    if(this.wsMap[connectionId]) {
      console.log(`${connectionId} is found in map, no need to create new server`);
      return;
    }

    this.wsMap[connectionId] = new WebSocket.Server({ noServer: true });
    // console.log(this.wsMap);
    this.wsMap[connectionId].on('connection', (ws) => {
      if (!_.find(this.wsMap[connectionId].clients, client => client === ws)) {
        // this is a new client to an existing chat
        // try to get cache, if no cache, talk to sql and update cache
        // if not found in sql, return empty
        this.cache.get(connectionId, (value) => {
          console.log(`found in redis ${connectionId}: ${value}`)
          ws.send(value);
        }, () => {
          console.log(`not found in redis ${connectionId}`)
          // cache not found, talk to db
          this.database.get(connectionId, (value1) => {
            console.log(`found in mysql ${connectionId}: ${value1}`)
            this.cache.set(connectionId, value1);
            ws.send(value1);
          }, () => {
            console.log(`not found in mysql ${connectionId}`)
          });
        });
      }

      ws.on('message', (data) => {
        console.log(`received new data for ${connectionId}: ${data}`);
        this.cache.set(connectionId, data);
        this.wsMap[connectionId].clients.forEach(function each(client) {
          // console.log(`broadcast new data for ${connectionId}: ${data}`);
          if (client !== ws && client.readyState === WebSocket.OPEN) {
            client.send(data);
          }
        });
        
        this.database.set(connectionId, data);
      });

      ws.on('close', () => {
        console.log(`this is ${this.wsMap[connectionId].clients.size} clients connecting to ${connectionId}`);
        if(this.wsMap[connectionId].clients.size === 0) {
          console.log(`no clients connecting to ${connectionId}, clean up and close server connection`);
          this.wsMap[connectionId].close();
          delete this.wsMap[connectionId];
        }
      })
    });
  }

  handleUpgrade(connectionId, request, socket, head) {
    // the client may request a ws connection the server does not have
    // in which case we need to first build the ws connection
    this.createConnection(connectionId);

    console.log(`WsManager.handleUpgrade for ${connectionId}`);
    this.wsMap[connectionId].handleUpgrade(request, socket, head, (ws) => {
      this.wsMap[connectionId].emit('connection', ws, request);
    });
  }
};
