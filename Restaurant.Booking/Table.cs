namespace Restaurant.Booking
{
    public class Table
    {
        private readonly object _lock = new();

        public TableState State { get; private set; }
        public int SeatsCount { get; }
        public int Id { get; }

        public Table(int id)
        {
            Id = id;
            State = TableState.Free;
            SeatsCount = Random.Shared.Next(2, 5);
        }

        public bool SetState(TableState state)
        {
            lock (_lock)
            {
                if (state == State)
                {
                    return false;
                }

                State = state;
                return true;
            }
        }
    }
}