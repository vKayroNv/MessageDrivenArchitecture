namespace Restaurant.Booking
{
    public class Table
    {
        public const byte MAXSEATSCOUNT = 5;

        private readonly int _id;
        private readonly byte _seatsCount;
        private TableState _state;

        public int Id { get => _id; }
        public byte SeatsCount { get => _seatsCount; }
        public TableState State { get => _state; }

        public Table(int id)
        {
            _id = id;
            _seatsCount = (byte)Random.Shared.Next(2, MAXSEATSCOUNT);
            _state = TableState.Free;
        }

        public bool SetTableState(TableState state)
        {
            if (_state == state)
            {
                return false;
            }

            _state = state;
            return true;
        }
    }
}
