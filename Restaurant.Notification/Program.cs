using MassTransit;
using Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Notification.Consumers;

namespace Restaurant.Notification
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseEnvironment("Development")
                .ConfigureServices((context, services) =>
                {
                    ConnectionOptions options = new(context.Configuration);

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<NotifierTableBookedConsumer>();
                        x.AddConsumer<KitchenReadyConsumer>();

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.UseMessageRetry(r =>
                            {
                                r.Exponential(5,
                                    TimeSpan.FromSeconds(1),
                                    TimeSpan.FromSeconds(100),
                                    TimeSpan.FromSeconds(5));
                                r.Ignore<StackOverflowException>();
                                r.Ignore<ArgumentNullException>(x => x.Message.Contains("Consumer"));
                            });

                            cfg.ConfigureEndpoints(context);
                            cfg.Host(options.HostName, options.Port, "/", h =>
                            {
                                h.Username(options.UserName);
                                h.Password(options.Password);
                            });
                        });
                    });
                    services.AddSingleton<Notifier>();
                });
    }
}