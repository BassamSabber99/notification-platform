using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationPlatform.Core.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace NotificationPlatform.Infrastructure.Services
{
    public class FirebasePushService : IPushNotificationService
    {
        private readonly FirebaseMessaging _messaging;
        private readonly ILogger<FirebasePushService> _logger;

        public FirebasePushService(IConfiguration config, ILogger<FirebasePushService> logger)
        {
            var credentialsPath = config["Firebase:CredentialsPath"];
            
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(credentialsPath)
                });
            }

            _messaging = FirebaseMessaging.DefaultInstance;
            _logger = logger;
        }

        public async Task<bool> SendAsync(string deviceToken, string title, string body, Dictionary<string, object> metadata)
        {
            try
            {
                var message = new Message()
                {
                    Token = deviceToken,
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body,
                    },
                    Data = metadata?.ToDictionary(x => x.Key, x => x.Value?.ToString()) ?? new Dictionary<string, string>(),
                };

                var response = await _messaging.SendAsync(message);

                _logger.LogInformation($"Push notification sent successfully. Message ID: {response}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception sending push notification: {ex.Message}");
                return false;
            }
        }
    }
}
