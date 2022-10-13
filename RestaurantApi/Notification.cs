namespace RestaurantApi
{
    public class Notification : INotification
    {
        private object _locker = new();

        public Notification()
        {

        }

        public void SendAsync(string text)
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(5000);

                lock (_locker)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("УВЕДОМЛЕНИЕ ");
                    Console.ResetColor();
                    Console.WriteLine(text);
                }
            });
        }
    }
}
