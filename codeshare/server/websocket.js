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

  createConnection(chatId) {
    // ignore existing chats
    if(this.wsMap[chatId]) {
      return;
    }
    
    this.wsMap[chatId] = new WebSocket.Server({ noServer: true });
    // console.log(this.wsMap);
    this.wsMap[chatId].on('connection', (ws) => {
      if (!_.find(this.wsMap[chatId].clients, client => client === ws)) {
        // this is a new client to an existing chat
        // try to get cache, if no cache, talk to sql and update cache
        // if not found in sql, return empty
        this.cache.get(chatId, (value) => {
          console.log(`found in redis ${chatId}: ${value}`)
          ws.send(value);
        }, () => {
          console.log(`not found in redis ${chatId}`)
          // cache not found, talk to db
          this.database.get(chatId, (value1) => {
            console.log(`found in mysql ${chatId}: ${value1}`)
            this.cache.set(chatId, value1);
            ws.send(value1);
          }, () => {
            console.log(`not found in mysql ${chatId}`)
          });
        });
      }

      ws.on('message', (data) => {
        console.log(`received new data for ${chatId}: ${data}`);
        this.cache.set(chatId, data);
        this.wsMap[chatId].clients.forEach(function each(client) {
          console.log(`broadcast new data for ${chatId}: ${data}`);
          if (client !== ws && client.readyState === WebSocket.OPEN) {
            client.send(data);
          }
        });

        // update database for permanent storage
        this.database.set(chatId, data);
      });
    });
  }

  handleUpgrade(chatId, request, socket, head) {
    this.wsMap[chatId].handleUpgrade(request, socket, head, (ws) => {
      this.wsMap[chatId].emit('connection', ws, request);
    });
  }
};
