namespace Repository
{
    
    public class Database
    {
        public Database()
        {
            
        }

        public void Connect(DatabaseConnectData databaseConnectData)
        {
            var db = new NpgsqlConnection("Host=localhost;Port=5432;Database=test;Username=postgres;Password=1");
        }

        public void Disconnect()
        {
            
        }
    }
}