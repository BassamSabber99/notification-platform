using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationPlatform.Core.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace NotificationPlatform.Infrastructure.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly string _fromNumber;
        private readonly ILogger<TwilioSmsService> _logger;

        public TwilioSmsService(IConfiguration config, ILogger<TwilioSmsService> logger)
        {
            var accountSid = config["Twilio:AccountSid"];
            var authToken = config["Twilio:AuthToken"];
            _fromNumber = config["Twilio:FromNumber"];

            TwilioClient.Init(accountSid, authToken);
            _logger = logger;
        }

        public async Task<bool> SendAsync(string phoneNumber, string body, Dictionary<string, object> metadata)
        {
            try
            {
                var message = await MessageResource.CreateAsync(
                    from: new Twilio.Types.PhoneNumber(_fromNumber),
                    to: new Twilio.Types.PhoneNumber(phoneNumber),
                    body: body
                );

                _logger.LogInformation($"SMS sent successfully to {phoneNumber}. SID: {message.Sid}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception sending SMS: {ex.Message}");
                return false;
            }
        }
    }
}
