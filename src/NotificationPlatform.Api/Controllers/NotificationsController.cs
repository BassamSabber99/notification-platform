using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NotificationPlatform.Core.Interfaces;
using NotificationPlatform.Core.Models;

namespace NotificationPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IRateLimiter _rateLimiter;
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(
            INotificationService notificationService,
            IRateLimiter rateLimiter,
            IAuditLogService auditLogService,
            ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _rateLimiter = rateLimiter;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<ActionResult<NotificationResponse>> SendNotification(SendNotificationRequest request)
        {
            try
            {
                var tenantId = User.FindFirst("tenant_id")?.Value;
                if (string.IsNullOrEmpty(tenantId))
                    return Unauthorized(new { message = "Tenant ID not found in token" });

                foreach (var channel in request.Channels)
                {
                    var allowed = await _rateLimiter.AllowRequestAsync(tenantId, channel);
                    if (!allowed)
                    {
                        var rateLimitInfo = await _rateLimiter.GetInfoAsync(tenantId, channel);
                        return StatusCode(429, new { message = "Rate limit exceeded", info = rateLimitInfo });
                    }
                }

                var result = await _notificationService.SendAsync(request, tenantId);

                await _auditLogService.LogAsync(new AuditLog
                {
                    TenantId = tenantId,
                    MessageId = result.MessageId,
                    Action = "SEND_NOTIFICATION",
                    Status = "SUCCESS",
                    Details = $"Sent via channels: {string.Join(",", request.Channels)}",
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notification: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{messageId}")]
        public async Task<ActionResult<NotificationStatusResponse>> GetNotificationStatus(Guid messageId)
        {
            var tenantId = User.FindFirst("tenant_id")?.Value;
            var result = await _notificationService.GetStatusAsync(messageId, tenantId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("audit-logs")]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var tenantId = User.FindFirst("tenant_id")?.Value;
            var logs = await _auditLogService.GetLogsAsync(tenantId, pageNumber, pageSize);
            return Ok(logs);
        }
    }
}
