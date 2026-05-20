using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NotificationPlatform.Core.Interfaces;
using NotificationPlatform.Core.Models;

namespace NotificationPlatform.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IRepository<AuditLog> _repository;

        public AuditLogService(IRepository<AuditLog> repository)
        {
            _repository = repository;
        }

        public async Task LogAsync(AuditLog log)
        {
            log.Timestamp = DateTime.UtcNow;
            await _repository.AddAsync(log);
            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetLogsAsync(string tenantId, int pageNumber, int pageSize)
        {
            return await _repository.GetPagedAsync(
                predicate: x => x.TenantId == tenantId,
                pageNumber: pageNumber,
                pageSize: pageSize,
                orderBy: x => x.OrderByDescending(l => l.Timestamp)
            );
        }
    }
}
