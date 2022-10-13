using System.Diagnostics;

namespace RestaurantApi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            INotification notification = new Notification();
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