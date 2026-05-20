using System;
using System.Threading.Tasks;

namespace NotificationPlatform.Core.Interfaces
{
    public interface IMessageQueueService
    {
        Task PublishAsync<T>(T message, string channel) where T : class;
        Task SubscribeAsync<T>(string channel, Func<T, Task> handler) where T : class;
    }
}
