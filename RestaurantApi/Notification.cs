namespace RestaurantApi
{
    public class Notification : INotification
    {
        public Notification()
        {

        }

        public void SendAsync(string text)
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(5000);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("УВЕДОМЛЕНИЕ ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(text);
            });
        }
    }
}
