﻿using MassTransit;
using Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Restaurant.Booking
{
    public static class Program
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
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.ConfigureEndpoints(context);
                            cfg.Host(options.HostName, options.Port, "/", h =>
                            {
                                h.Username(options.UserName);
                                h.Password(options.Password);
                            });
                        });
                    });

                    services.AddTransient<Restaurant>();

                    services.AddHostedService<Worker>();
                });
    }
}