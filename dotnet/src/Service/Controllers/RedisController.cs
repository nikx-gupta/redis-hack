using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Hack.Service.RedisDb.Controllers {
    [ApiController]
    [Route("[controller]/[action]")]
    public class RedisController {
        private readonly IDatabase _redisDb;

        public RedisController(ConnectionMultiplexer redisClient) {
            _redisDb = redisClient.GetDatabase();
        }

        [HttpGet("get")]
        public int Get() {
            var key = _redisDb.StringGet(new RedisKey("counter"));
            if (key == RedisValue.Null) {
                return 1;
            }

            return Convert.ToInt32(key);
        }


        [HttpGet("inc")]
        public int Increment() {
            var key = _redisDb.StringGet(new RedisKey("counter"));
            if (key == RedisValue.Null) {
                _redisDb.StringSet("counter", 1);
            }
            else {
                var inc = Convert.ToInt32(key) + 1;
                _redisDb.StringSet("counter", inc);
                return inc;
            }

            return 1;
        }
    }
}