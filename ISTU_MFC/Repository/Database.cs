using System.Collections.Generic;
using Npgsql;

namespace Repository
{
    public class Database
    {
        private NpgsqlConnection _db;
        
        public Database()
        {
        }

        public void Connect(DatabaseConnectData databaseConnectData)
        {
            _db = new NpgsqlConnection(databaseConnectData.ToString());
        }

        public string[][] StudentGroup(string variable) // например, группу студента по фамилии
        {
            string[][] result;
            using var query = new QueryTool(_db);
            result = query.QueryWithTable("Select g.group_number FROM Groups As g" + 
                                          " JOIN students As s ON s.group_number = g.id " + 
                                          $" JOIN Users As u ON s.user_id = u.id WHERE u.surname = '{variable}';");

            return result;
        }

        public void Disconnect()
        {
            
        }
    }
}