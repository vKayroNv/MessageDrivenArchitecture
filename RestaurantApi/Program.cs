using System.Diagnostics;

namespace RestaurantApi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Restaurant restaurant = new();

            while (true)
            {
                Console.WriteLine("Забронировать стол");
                Console.WriteLine("1 - Ожидание на линии");
                Console.WriteLine("2 - Уведомление по СМС");

                if (!int.TryParse(Console.ReadLine(), out int result) && result is not 1 or 2)
                {
                    Console.WriteLine("Введите 1 или 2");
                    continue;
                }

                Console.Write("Количество персон: ");
                if (!int.TryParse(Console.ReadLine(), out int count) && count <= 0)
                {
                    Console.WriteLine("Введите количество людей");
                    continue;
                }

                var stopwatch = Stopwatch.StartNew();

                switch (result)
                {
                    case 1:
                        restaurant.BookTable(count);
                        break;
                    case 2:
                        restaurant.BookTableAsync(count);
                        break;
                }

                stopwatch.Stop();
                Console.WriteLine($"Затрачено времени: {stopwatch.ElapsedMilliseconds} мс");
            }
        }
    }
}