using System;
using System.Collections.Generic;
using ModelsData;
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

        public List<FieldsModel> GetRequestFeelds(int requestId)
        {
            using var query = new QueryTool(_db);
            var res = query.QueryWithTable($"SELECT name, value, manually_filled FROM fields WHERE request_id = {requestId}");
            var list = new List<FieldsModel>();
            for (int i = 1; i < res.Length; i++)
            {
                list.Add(new FieldsModel()
                {
                    Name = res[i][0],
                    Value = res[i][1],
                    Malually_fiiled = bool.Parse(res[i][2])
                });
            }
            return list;
        }

        public int GetStudentByRequest(int requestId)
        {
            using var query = new QueryTool(_db);
            var res = query.QueryWithTable($"SELECT user_id FROM students WHERE stud_id = (SELECT stud_id FROM requests WHERE id = {requestId})");
            return Int32.Parse(res[1][0]);
        }
        
        public void Disconnect()
        {
            
        }
    }
}