using Messaging;
using System.Collections.Concurrent;

namespace Restaurant.Booking
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
            if (request.Type == BookingType.Async)
            {
                Console.WriteLine("Ожидайте уведомления, сейчас подберем вам стол");
            }
            else
            {
                Console.WriteLine("Подождите на линии, сейчас подберем вам стол");
            }

            _requests.Enqueue(request);
        }

        private void BookTableBase(bool isAsync, byte countOfPerson)
        {
            var table = _tables.FirstOrDefault(s => s.SeatsCount >= countOfPerson && s.State == TableState.Free);

            if (!isAsync)
            {
                Thread.Sleep(5000);
            }

            if (table != null)
            {
                table.SetTableState(TableState.Booked);

                if (isAsync)
                {
                    _notification.SendAsync($"Готово! Ваш столик номер {table.Id}");
                }
                else
                {
                    Console.WriteLine($"Готово! Ваш столик номер {table.Id}");
                }
            }
            else
            {
                var tables = _tables
                    .Where(s => s.State == TableState.Free)
                    .ToList();

                if (tables.Any())
                {
                    tables.Sort((a, b) => b.SeatsCount.CompareTo(a.SeatsCount));

                    byte seatsCount = 0;
                    List<int> tableIds = new();

                    foreach (var _table in tables)
                    {
                        seatsCount += _table.SeatsCount;
                        tableIds.Add(_table.Id);

                        if (seatsCount >= countOfPerson)
                        {
                            break;
                        }
                    }

                    if (seatsCount >= countOfPerson)
                    {
                        foreach (var id in tableIds)
                        {
                            _tables.First(s => s.Id == id).SetTableState(TableState.Booked);
                        }

                        if (isAsync)
                        {
                            _notification.SendAsync($"Готово! Номера ваших столиков: {string.Join(", ", tableIds)}");
                        }
                        else
                        {
                            Console.WriteLine($"Готово! Номера ваших столиков: {string.Join(", ", tableIds)}");
                        }
                    }
                    else
                    {
                        if (isAsync)
                        {
                            _notification.SendAsync("К сожалению все столики заняты");
                        }
                        else
                        {
                            Console.WriteLine("К сожалению все столики заняты");
                        }
                    }
                }
            }
        }

        private void BookTable(byte countOfPerson)
        {
            BookTableBase(false, countOfPerson);
        }

        private void BookTableAsync(byte countOfPerson)
        {
            BookTableBase(true, countOfPerson);
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
