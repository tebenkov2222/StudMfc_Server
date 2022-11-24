namespace ISTU_MFC.Repository
{
    public class DatabaseConnectData
    {
        public string Server;
        public string Port;
        public string UserId;
        public string Password;
        public string Database;
        public int Timeout;
        public int CommandTimeout;
        public int KeepAlive;

        public static DatabaseConnectData NiardanDefaultData => new DatabaseConnectData()
        {
            Server = "niardan.ru",
            Port = "54334",
            UserId = "tebenkov2222",
            Password = "TP_Linkenew",
            Database = "tebenkov2222",
            Timeout = 300,
            CommandTimeout = 300,
            KeepAlive = 300
        };

        public override string ToString() => 
            $"Host={Server};Port={Port};Database={Database};Username={UserId};Password={Password}";
    }
}