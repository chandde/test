using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace MainService
{
    public class Cache : ICache
    {
        ConnectionMultiplexer redis;
        IDatabase db;

        public Cache(string server)
        {
            // initialize redis
            redis = ConnectionMultiplexer.Connect(server);
            db = redis.GetDatabase(-1);
        }

        public User GetUser(string userId)
        {
            var ret = db.StringGet(userId);
            if (ret.HasValue)
            {
                return ret.Box() as User;
            }

            return null;
        }
        public File GetFile(string fileId)
        {
            var ret = db.StringGet(fileId);
            if (ret.HasValue)
            {
                return ret.Box() as File;
            }

            return null;
        }
        public void UpdateUser(User user)
        {
            // db.StringSet(new RedisKey(user.Id), new RedisValue(JsonSerializer.Serialize(user)));
        }

        public void UpdateFile(File file)
        {
            // db.StringSet(new RedisKey(file.Id), new RedisValue(JsonSerializer.Serialize(file)));
        }
    }
}
