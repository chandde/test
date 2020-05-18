// redis cache
const redis = require("redis");

exports.Cache = class Cache {
  constructor() {
    console.log('Cache ctor');
    this.redisClient = redis.createClient(6379, '192.168.1.123');
  }

  async get(key, found, notFound) {
    this.redisClient.get(key, (err, value) => {
      if(err || value == null || value == undefined) {
        console.log(err);
        notFound(err);
      } else {
        console.log(value);
        found(value);
      }
    });
  }

  set(key, value) {
    console.log(`set redis ${key}: ${value}`)
    this.redisClient.set(key, value);
  }
}