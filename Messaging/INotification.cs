namespace Messaging
{
    public interface INotification
    {
        void SendAsync(string text);
    }
}