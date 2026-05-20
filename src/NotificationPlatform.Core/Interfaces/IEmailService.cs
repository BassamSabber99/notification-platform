using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationPlatform.Core.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendAsync(string to, string subject, string body, Dictionary<string, object> metadata);
    }
}
