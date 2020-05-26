// redis cache
const redis = require("redis");

exports.Cache = class Cache {
  constructor() {
    console.log('Cache ctor');
    this.redisClient = redis.createClient(6379, 'localhost');
  }

  async get(key, found, notFound) {
    const start = new Date();
    this.redisClient.get(key, (err, value) => {
      console.log(`took ${new Date() - start}ms to get redis`);
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
    const start = new Date();
    this.redisClient.set(key, value, () => {
      console.log(`took ${new Date() - start}ms to set redis`);
    });
  }
}