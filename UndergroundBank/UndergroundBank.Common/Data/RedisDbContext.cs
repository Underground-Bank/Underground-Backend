using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace UndergroundBank.Common.Data
{
    public class RedisDbContext
    {
        private readonly IDatabase _db;

        public RedisDbContext(string connectionString)
        {
            var connection = ConnectionMultiplexer.Connect(connectionString);
            _db = connection.GetDatabase();
        }

        public async Task AddToken(string token)
        {
            await _db.StringSetAsync(token, "blacklisted", TimeSpan.FromMinutes(10));
        }

        public async Task<bool> IsBlackToken(string token)
        {
            return await _db.KeyExistsAsync(token);
        }
    }
}
