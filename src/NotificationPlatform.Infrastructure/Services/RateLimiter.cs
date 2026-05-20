using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NotificationPlatform.Core.Interfaces;
using StackExchange.Redis;

namespace NotificationPlatform.Infrastructure.Services
{
    public class RedisRateLimiter : IRateLimiter
    {
        private readonly IDatabase _db;
        private readonly Dictionary<string, int> _requestsPerMinute;

        public RedisRateLimiter(IConnectionMultiplexer redis, IConfiguration config)
        {
            _db = redis.GetDatabase();
            _requestsPerMinute = new Dictionary<string, int>
            {
                { "email", config.GetValue<int>("NotificationPlatform:RateLimiting:Email:RequestsPerMinute", 1000) },
                { "sms", config.GetValue<int>("NotificationPlatform:RateLimiting:SMS:RequestsPerMinute", 500) },
                { "push", config.GetValue<int>("NotificationPlatform:RateLimiting:Push:RequestsPerMinute", 2000) },
                { "whatsapp", config.GetValue<int>("NotificationPlatform:RateLimiting:WhatsApp:RequestsPerMinute", 300) }
            };
        }

        public async Task<bool> AllowRequestAsync(string tenantId, string channel)
        {
            var key = $"ratelimit:{tenantId}:{channel}";
            var limit = _requestsPerMinute[channel.ToLower()];

            var current = await _db.StringGetAsync(key);
            var count = current.IsNullOrEmpty ? 0 : int.Parse(current.ToString());

            if (count >= limit)
            {
                return false;
            }

            var transaction = _db.CreateTransaction();
            transaction.StringIncrementAsync(key);
            transaction.KeyExpireAsync(key, TimeSpan.FromMinutes(1));
            await transaction.ExecuteAsync();

            return true;
        }

        public async Task<RateLimitInfo> GetInfoAsync(string tenantId, string channel)
        {
            var key = $"ratelimit:{tenantId}:{channel}";
            var current = await _db.StringGetAsync(key);
            var ttl = await _db.KeyTimeToLiveAsync(key);
            var limit = _requestsPerMinute[channel.ToLower()];

            return new RateLimitInfo
            {
                CurrentRequests = current.IsNullOrEmpty ? 0 : int.Parse(current.ToString()),
                Limit = limit,
                TimeToResetSeconds = (int?)ttl?.TotalSeconds ?? 0
            };
        }
    }
}
