using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationPlatform.Core.Interfaces
{
    public interface IWhatsAppService
    {
        Task<bool> SendAsync(string phoneNumber, string body, Dictionary<string, object> metadata);
    }
}
