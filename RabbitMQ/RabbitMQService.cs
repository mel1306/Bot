using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using MessageQueue;

namespace RabbitMQ
{
    public class RabbitMQService
    {
        private const string QueueName = "FinancialChatQueue";
        private readonly IModel _channel;

        public RabbitMQService(RabbitMQInfo rabbitMq)
        {
            var rabbitMqInfo = rabbitMq;
            var connectionFactory = new ConnectionFactory
            {
                UserName = rabbitMqInfo.Username,
                Password = rabbitMqInfo.Password,
                VirtualHost = rabbitMqInfo.VirtualHost,
                HostName = rabbitMqInfo.HostName,
                Uri = new Uri(rabbitMqInfo.Uri)
            };
            var connection = connectionFactory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        public void PublishMessage(string message)
        {
            var jsonPayload = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonPayload);

            _channel.BasicPublish(exchange: "",
                routingKey: QueueName,
                basicProperties: null,
                body: body
            );
        }
    }
}
