using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NotificationPlatform.Core.Models;

namespace NotificationPlatform.Core.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(AuditLog log);
        Task<IEnumerable<AuditLog>> GetLogsAsync(string tenantId, int pageNumber, int pageSize);
    }
}
