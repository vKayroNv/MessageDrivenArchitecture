namespace Messaging
{
    public class ConnectionOptions
    {
        public string HostName { get; private set; } = "localhost";
        public ushort Port { get; private set; } = 5672;
        public string UserName { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;

        public string ConnectionString
        {
            set
            {
                var data = value.Split(';');
                foreach (var item in data)
                {
                    var pair = item.Split('=');
                    if (pair.Length != 2)
                    {
                        continue;
                    }

                    if (pair[0] == "HostName")
                    {
                        HostName = pair[1];
                    }
                    if (pair[0] == "Port")
                    {
                        if (ushort.TryParse(pair[1], out var port))
                        {
                            Port = port;
                        }
                    }
                    if (pair[0] == "UserName")
                    {
                        UserName = pair[1];
                    }
                    if (pair[0] == "Password")
                    {
                        Password = pair[1];
                    }
                }
            }
        }
    }
}
