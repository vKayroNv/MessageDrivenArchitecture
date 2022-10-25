namespace Restaurant.Booking
{
    public class Restaurant
    {
        private readonly List<Table> _tables = new();

        public Restaurant()
        {
            for (ushort i = 1; i <= 10; i++)
            {
                _tables.Add(new Table(i));
            }
        }

        public async Task<bool?> BookFreeTableAsync(int countOfPersons)
        {
            Console.WriteLine("Ожидайте уведомления, сейчас подберем вам стол");
            var table = _tables.FirstOrDefault(t => t.SeatsCount > countOfPersons && t.State == TableState.Free);
            await Task.Delay(1000 * 5);
            return table?.SetState(TableState.Booked);
        }
    }
}