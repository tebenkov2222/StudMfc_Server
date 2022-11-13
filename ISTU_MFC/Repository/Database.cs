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

        public void Disconnect()
        {
        }
        
        public void Connect(DatabaseConnectData databaseConnectData) =>
            _db = new NpgsqlConnection(databaseConnectData.ToString());
        
        public bool CheckByStudent(int userId)
        {
            using var query = new QueryTool(_db);
            return bool.Parse(query.QueryWithTable
                ($"SELECT (SELECT Count(*) FROM students WHERE user_id = {userId}) = 1")[1][0]);
        }

        public bool CheckByEmployee(int userId)
        {
            using var query = new QueryTool(_db);
            return bool.Parse(query.QueryWithTable
                ($"SELECT (SELECT Count(*) FROM employees WHERE user_id = {userId}) = 1")[1][0]);
        }
        
        //user_id   student_id  family     name    secondName      Group            faculty                     istituty
        // 19	    20043561	"Test"	"Student"	"Test"	    "Б20-191-1"	"Программное обеспечение"	"Информатика и Вычислительная Техника"
        public Dictionary<string, string> GetStudentInfo(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithDictionary($"SELECT * FROM students_info WHERE user_id = {userId}");
        }
        
        // user_id  family     name         secondName          post                 subdivision
        // 20	    "Test"	 "Employers"	    "Test"	    "Человек-бензопила"	    "Деканат ИМОП"
        public Dictionary<string, string> GetEmployeeInfo(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithDictionary($"SELECT * FROM employees_info WHERE user_id = {userId}");
        }
        
        // request_id,      name_service
        //   1223	    "Материальная помошь"
        //  443355	    "Материальная помошь"
        //  335544	    "Академический отпуск"
        public string[][] GetTableAvailableRequestsForEmployees(int userId) 
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
                ("SELECT request_id, name_service FROM list_of_requests_for_employees " + 
                 $"WHERE employee_id IS null AND user_id = {userId};");
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
        //   document_link
        // "ссылка на документ"
        public Dictionary<string, string> GetLinkToDocument(int requestId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithDictionary
                ($"SELECT document_link FROM references_to_documents WHERE request_id = {requestId};");
        }
        
        //   name,    value,   manually_filled
        // "field7"	 "test7"	false
        // "field8"	 "test8"	false
        // "field9"	 "test9"	true
        public string[][] GetTableFields(int requestId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
                ($"SELECT name, value, manually_filled FROM list_of_fields where request_id = {requestId}");
        }
        
        // Переписал только имена столбцов! Очень большая таблица получилась.
        // request_id   student_family  student_name    student_secondName  employee_family  employee_name
        // employee_secondName name_service subdivision_name create_date execution_date closing_date status
        public Dictionary<string, string> GetInformationAboutRequest(int requestId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithDictionary
                ($"SELECT * FROM information_about_requests WHERE request_id = {requestId}");
        }
    }
}