using Messaging;
using Messaging.RabbitMQ;
using Microsoft.Extensions.Configuration;

namespace Restaurant.Booking
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
            var config = builder.Build();

            // dotnet user-secrets set "ConnectionString" "HostName=<hostname>;Port=<port>;UserName=<username>;Password=<password>"
            ConnectionOptions options = new() { ConnectionString = config["ConnectionString"] };

            INotification notification = new Producer("BookingNotification", options);

            Restaurant restaurant = new(notification);

            while (true)
            {
                Console.WriteLine("Забронировать стол");
                Console.WriteLine("1 - Ожидание на линии");
                Console.WriteLine("2 - Уведомление по СМС");

                if (!int.TryParse(Console.ReadLine(), out int result) || (result != 1 && result != 2))
                {
                    Console.WriteLine("Введите 1 или 2");
                    continue;
                }

                Console.Write("Количество персон: ");
                if (!byte.TryParse(Console.ReadLine(), out byte count) && count <= 0)
                {
                    Console.WriteLine("Введите количество людей");
                    continue;
                }

                switch (result)
                {
                    case 1:
                        restaurant.CreateBookRequest(new() { CountOfPerson = count, Type = BookingType.Sync });
                        break;
                    case 2:
                        restaurant.CreateBookRequest(new() { CountOfPerson = count, Type = BookingType.Async });
                        break;
                }
            }
        }
    }
}