using System;
using System.Text.Json;
using System.Threading.Tasks;
using NotificationPlatform.Core.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationPlatform.Infrastructure.Services
{
    public class RabbitMQService : IMessageQueueService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService(string connectionString)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(connectionString) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public async Task PublishAsync<T>(T message, string queueName) where T : class
        {
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var json = JsonSerializer.Serialize(message);
            var body = System.Text.Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: properties,
                body: body
            );

            await Task.CompletedTask;
        }

        public async Task SubscribeAsync<T>(string queueName, Func<T, Task> handler) where T : class
        {
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageText = System.Text.Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<T>(messageText);

                try
                {
                    await handler(message);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            await Task.CompletedTask;
        }
    }
}
