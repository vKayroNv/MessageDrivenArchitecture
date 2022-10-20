using Messaging;
using Messaging.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text;

namespace Restaurant.Notification
{
    public class Worker : BackgroundService
    {
        private readonly Consumer _consumer;

        public Worker(IConfiguration configuration)
        {
            ConnectionOptions options = new() { ConnectionString = configuration["ConnectionString"] };

            _consumer = new Consumer("BookingNotification", options);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Receive((sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            });
        }
    }
}
