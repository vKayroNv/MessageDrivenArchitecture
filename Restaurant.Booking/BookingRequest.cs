namespace Restaurant.Booking
{
    public class BookingRequest
    {
        public byte CountOfPerson { get; set; }

        public BookingType Type { get; set; }
    }

    public enum BookingType
    {
        Sync,
        Async,
    }
}
