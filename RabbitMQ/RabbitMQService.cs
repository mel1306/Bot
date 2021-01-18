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
        //private readonly EventingBasicConsumer _consumer;

        //public event EventHandler MessagePublished;

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
            //_consumer = new EventingBasicConsumer(_channel);
            //_consumer.Received += (sender, e) =>
            //{
            //    var body = e.Body.ToArray();
            //    var jsonMessage = Encoding.UTF8.GetString(body);
            //    var message = JsonConvert.DeserializeObject<string>(jsonMessage);
            //    ConsumeMessage(message);
            //};
            //_channel.BasicConsume(QueueName, true, _consumer);
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

        //protected virtual void OnMessagePublished(EventArgs e)
        //{
        //    EventHandler handler = MessagePublished;
        //    //handler?.Invoke(this, e);
        //    if (handler != null)
        //    {
        //        handler(this, e);
        //    }
        //}

        //private void ConsumeMessage(string message)
        //{
        //    OnMessagePublished(EventArgs.Empty);
        //}
    }
}
