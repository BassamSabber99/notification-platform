using System;

namespace NotificationPlatform.Core.Models
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string TenantId { get; set; }
        public Guid MessageId { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
    }
}
