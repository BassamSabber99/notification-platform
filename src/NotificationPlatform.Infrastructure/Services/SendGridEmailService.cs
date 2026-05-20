using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationPlatform.Core.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NotificationPlatform.Infrastructure.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly SendGridClient _client;
        private readonly string _fromEmail;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(IConfiguration config, ILogger<SendGridEmailService> logger)
        {
            var apiKey = config["SendGrid:ApiKey"];
            _fromEmail = config["SendGrid:FromEmail"];
            _client = new SendGridClient(apiKey);
            _logger = logger;
        }

        public async Task<bool> SendAsync(string to, string subject, string body, Dictionary<string, object> metadata)
        {
            try
            {
                var from = new EmailAddress(_fromEmail);
                var toAddress = new EmailAddress(to);
                var msg = new SendGridMessage()
                {
                    From = from,
                    Subject = subject,
                    HtmlContent = body,
                };
                msg.AddTo(toAddress);

                var response = await _client.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted ||
                    response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"Email sent successfully to {to}");
                    return true;
                }

                _logger.LogError($"Failed to send email to {to}. Status: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception sending email: {ex.Message}");
                return false;
            }
        }
    }
}
