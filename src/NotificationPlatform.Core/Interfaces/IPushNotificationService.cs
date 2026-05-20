using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationPlatform.Core.Interfaces
{
    public interface IPushNotificationService
    {
        Task<bool> SendAsync(string deviceToken, string title, string body, Dictionary<string, object> metadata);
    }
}
