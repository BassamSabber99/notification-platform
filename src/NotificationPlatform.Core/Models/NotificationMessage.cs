using System;
using System.Collections.Generic;

namespace NotificationPlatform.Core.Models
{
    public class NotificationMessage
    {
        public Guid Id { get; set; }
        public string TenantId { get; set; }
        public NotificationChannel Channel { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientPhoneNumber { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
        public MessageStatus Status { get; set; }
        public int RetryCount { get; set; }
        public int MaxRetries { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? FailedAt { get; set; }
        public string FailureReason { get; set; }
        public string ProviderMessageId { get; set; }
        public Dictionary<string, string> ProviderResponse { get; set; }
    }

    public enum NotificationChannel
    {
        Email = 1,
        SMS = 2,
        PushNotification = 3,
        WhatsApp = 4
    }

    public enum MessageStatus
    {
        Pending = 1,
        Queued = 2,
        Sent = 3,
        Failed = 4,
        RetryScheduled = 5,
        Delivered = 6
    }
}
