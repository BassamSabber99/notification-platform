using System.Threading.Tasks;

namespace NotificationPlatform.Core.Interfaces
{
    public interface IRateLimiter
    {
        Task<bool> AllowRequestAsync(string tenantId, string channel);
        Task<RateLimitInfo> GetInfoAsync(string tenantId, string channel);
    }

    public class RateLimitInfo
    {
        public int CurrentRequests { get; set; }
        public int Limit { get; set; }
        public int TimeToResetSeconds { get; set; }
    }
}
