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

        public string[][] StudentGroup(string variable, string field1, string field2) // например, группу студента по фамилии
        {
            string[][] result;
            using var query = new QueryTool(_db);
            result = query.QueryWithTable($"Select g.group_number AS {field1} FROM Groups As g" + 
                                          $" JOIN students As s ON s.group_number = g.id " + 
                                          $" JOIN Users As u ON s.user_id = u.id WHERE u.surname = '{variable}';");

            return result;
        }

        public bool CheckByStudent(int idUser)
        {
            using (var query = new QueryTool(_db))
            {
                var result = query.QueryWithTable($"SELECT (SELECT Count(*) FROM students WHERE user_id = {idUser}) = 1");
                return bool.Parse(query.QueryWithTable($"SELECT (SELECT Count(*) FROM students WHERE user_id = {idUser}) = 1")[0][1]);
            }

            return false;
        }

        public bool CheckByEmployee(int idUser)
        {
            using var query = new QueryTool(_db);
            return bool.Parse(query.QueryWithTable($"SELECT (SELECT Count(*) FROM employees WHERE user_id = {idUser}) = 1")[0][1]);
        }

        public void Disconnect()
        {
            
        }
    }
}