namespace RestaurantApi
{
    public interface INotification
    {
        void SendAsync(string text);
    }
}