using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging.RabbitMQ
{
    public class Consumer : IDisposable
    {
        private readonly string _queueName;

        private readonly IConnection _connection;
        private readonly IModel _channel;

        public Consumer(string queueName, ConnectionOptions options)
        {
            _queueName = queueName;

            var factory = new ConnectionFactory()
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void Receive(EventHandler<BasicDeliverEventArgs> receiveCallback)
        {
            _channel.ExchangeDeclare("direct_exchange", "direct");

            _channel.QueueDeclare(_queueName, false, false, false, null);

            _channel.QueueBind(_queueName, "direct_exchange", _queueName);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += receiveCallback;

            _channel.BasicConsume(_queueName, true, consumer);
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}