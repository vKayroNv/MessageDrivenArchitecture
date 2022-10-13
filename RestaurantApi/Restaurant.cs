using System.Collections.Concurrent;

namespace RestaurantApi
{
    public class Restaurant
    {
        private const int TABLESCOUNT = 20;

        private readonly INotification _notification;

        private readonly List<Table> _tables = new();

        private readonly ConcurrentQueue<BookingRequest> _requests = new();

        public Restaurant(INotification notification)
        {
            _notification = notification;

            for (int i = 1; i <= TABLESCOUNT; i++)
            {
                _tables.Add(new(i));
            }

            UnbookRandomly();
            ProcessRequests();
        }

        public void CreateBookRequest(BookingRequest request)
        {
            _requests.Enqueue(request);
        }

        private void BookTable(int countOfPerson)
        {
            Console.WriteLine("Подождите на линии, сейчас подберем вам стол");

            var table = _tables.FirstOrDefault(s => s.SeatsCount >= countOfPerson && s.State == TableState.Free);

            Thread.Sleep(5000);

            if (table != null)
            {
                table.SetTableState(TableState.Booked);
                Console.WriteLine($"Готово! Ваш столик номер {table.Id}");
            }
            else
            {
                Console.WriteLine("К сожалению все столики заняты");
            }
        }

        private void BookTableAsync(int countOfPerson)
        {
            Console.WriteLine("Ожидайте уведомления, сейчас подберем вам стол");

            Task.Factory.StartNew(() =>
            {
                var table = _tables.FirstOrDefault(s => s.SeatsCount >= countOfPerson && s.State == TableState.Free);

                if (table != null)
                {
                    table.SetTableState(TableState.Booked);
                    _notification.SendAsync($"Готово! Ваш столик номер {table.Id}");
                }
                else
                {
                    _notification.SendAsync("К сожалению все столики заняты");
                }
            });
        }

        private void UnbookTable(int id)
        {
            var table = _tables.FirstOrDefault(s => s.Id >= id && s.State == TableState.Booked);

            Thread.Sleep(5000);

            if (table != null)
            {
                table.SetTableState(TableState.Free);
                Console.WriteLine($"Бронь столика {id} снята");
            }
            else
            {
                Console.WriteLine($"Столик {id} не был забронирован");
            }
        }

        private void UnbookTableAsync(int id)
        {
            Task.Factory.StartNew(() =>
            {
                var table = _tables.FirstOrDefault(s => s.Id >= id && s.State == TableState.Booked);

                if (table != null)
                {
                    table.SetTableState(TableState.Free);
                    _notification.SendAsync($"Бронь столика {id} снята");
                }
                else
                {
                    _notification.SendAsync($"Столик {id} не был забронирован");
                }
            });
        }

        private void UnbookRandomly()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000 * Random.Shared.Next(20, 31));

                    var tables = _tables.Where(s => s.State == TableState.Booked);

                    if (tables.Any())
                    {
                        var table = tables.ElementAt(Random.Shared.Next(tables.Count()));
                        UnbookTableAsync(table.Id);
                    }
                }
            });
        }

        private void ProcessRequests()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000);

                    while (!_requests.IsEmpty)
                    {
                        if (_requests.TryDequeue(out var request))
                        {
                            switch (request.Type)
                            {
                                case BookingType.Sync:
                                    BookTable(request.CountOfPerson);
                                    break;
                                case BookingType.Async:
                                    BookTableAsync(request.CountOfPerson);
                                    break;
                            }
                        }

                        await Task.Delay(100);
                    }
                }
            });
        }
    }
}
