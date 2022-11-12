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

        public bool CheckByStudent(int userId)
        {
            using var query = new QueryTool(_db);
            return bool.Parse(query.QueryWithTable($"SELECT (SELECT Count(*) FROM students WHERE user_id = {userId}) = 1")[1][0]);
        }

        public bool CheckByEmployee(int userId)
        {
            using var query = new QueryTool(_db);
            return bool.Parse(query.QueryWithTable($"SELECT (SELECT Count(*) FROM employees WHERE user_id = {userId}) = 1")[1][0]);
        }
        
        public string[][] GetStudentInfo(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT * FROM students_info WHERE user_id = {userId}");
        }

        public void Disconnect()
        {
            
        }
    }
}