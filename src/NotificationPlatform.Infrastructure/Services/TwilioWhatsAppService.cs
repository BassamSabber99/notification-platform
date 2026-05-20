using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationPlatform.Core.Interfaces;

namespace NotificationPlatform.Infrastructure.Services
{
    public class TwilioWhatsAppService : IWhatsAppService
    {
        private readonly string _fromNumber;
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TwilioWhatsAppService> _logger;

        public TwilioWhatsAppService(IConfiguration config, HttpClient httpClient, ILogger<TwilioWhatsAppService> logger)
        {
            _accountSid = config["Twilio:AccountSid"];
            _authToken = config["Twilio:AuthToken"];
            _fromNumber = config["Twilio:WhatsAppNumber"];
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> SendAsync(string phoneNumber, string body, Dictionary<string, object> metadata)
        {
            try
            {
                var url = $"https://api.twilio.com/2010-04-01/Accounts/{_accountSid}/Messages.json";
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                var credentials = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{_accountSid}:{_authToken}")
                );
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("From", $"whatsapp:{_fromNumber}"),
                    new KeyValuePair<string, string>("To", $"whatsapp:{phoneNumber}"),
                    new KeyValuePair<string, string>("Body", body),
                });

                request.Content = content;
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"WhatsApp message sent successfully to {phoneNumber}");
                    return true;
                }

                _logger.LogError($"Failed to send WhatsApp message. Status: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception sending WhatsApp message: {ex.Message}");
                return false;
            }
        }
    }
}
