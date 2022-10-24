using RabbitMQ.Client;
using System.Text;

namespace Messaging.RabbitMQ
{
    public class Producer : INotification
    {
        private readonly string _queueName;
        private readonly ConnectionOptions _options;

        public Producer(string queueName, ConnectionOptions options)
        {
            _queueName = queueName;
            _options = options;
        }

        public void SendAsync(string message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("direct_exchange", "direct", false, false, null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish("direct_exchange", _queueName, null, body);
        }
    }
}