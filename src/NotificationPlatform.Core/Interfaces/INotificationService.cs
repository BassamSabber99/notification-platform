using System;
using System.Threading.Tasks;
using NotificationPlatform.Core.Models;

namespace NotificationPlatform.Core.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResponse> SendAsync(SendNotificationRequest request, string tenantId);
        Task<NotificationStatusResponse> GetStatusAsync(Guid messageId, string tenantId);
        Task<bool> RetryFailedMessagesAsync();
    }

    public class SendNotificationRequest
    {
        public string[] Channels { get; set; }
        public RecipientInfo Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class RecipientInfo
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class NotificationResponse
    {
        public Guid MessageId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class NotificationStatusResponse
    {
        public Guid MessageId { get; set; }
        public string Status { get; set; }
        public int RetryCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
    }
}
