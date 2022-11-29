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
        public string[][] GetStudentInfo(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT * FROM students_info WHERE user_id = {userId}");
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
        public string[][] GetTableAvailableRequestsForEmployees(int userId, string status = "closed") 
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
            ("SELECT request_id, name_service, stud_family, stud_name, stud_secondname, create_date " + 
             " FROM list_of_requests_for_employees " + $"WHERE status <> '{status}' AND user_id = {userId};");
        }

        //   document_link
        // "ссылка на документ"
        public string GetLinkToDocumentByRequestId(int requestId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithDictionary
                ($"SELECT document_link FROM references_to_documents WHERE request_id = {requestId};")["document_link"];
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
        public Dictionary<string, string> GetInformationAboutStudent(int studentUserId)
        {
            using var query = new QueryTool(_db);   
            return query.QueryWithDictionary
                ($"SELECT * FROM information_about_requests WHERE student_user_id = {studentUserId}");
        }
        public Dictionary<string, string> GetDirectorInstituteByStudent(int studentUserId)
        {
            using var query = new QueryTool(_db);   
            return query.QueryWithDictionary
                ($"SELECT * FROM directors_of_institutes_for_each_user WHERE user_id = {studentUserId}");
        }
        
        public string[][] GetRequestFeelds(int requestId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT name, value, manually_filled FROM fields WHERE request_id = {requestId}");
        }
        
        public int GetStudentByRequest(int requestId)
        {
            using var query = new QueryTool(_db);
            var res = query.QueryWithTable($"SELECT user_id FROM students WHERE stud_id = (SELECT stud_id FROM requests WHERE id = {requestId})");
            return Int32.Parse(res[1][0]);
        }

        public void ChangeRequestState(int requestId, int user_id, string status)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"UPDATE requests SET employee_id = (SELECT employee_id FROM employees WHERE user_id = {user_id}), status = '{status}' WHERE id = {requestId}");
        }

        public void CreateMessage(int requestId, int employee_id, string message)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"INSERT INTO messages (employee_id, stud_id, text_message, request_id) VALUES " +
                                    $" ((SELECT employee_id FROM employees WHERE user_id = {employee_id}),"+
                                    $"(SELECT stud_id FROM requests WHERE id = {requestId}), '{message}', {requestId})");
        }
        
        // user_id, dispatch_date, text_message, request_id, status
        public string[][] GetTableMessages(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT * FROM messages_table_for_student WHERE user_id = {userId}");
        }
        public string[][] GetTableSubdivisionsInfoForStudent(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT * FROM subdivisions_info WHERE user_id =  {userId}");
        }

        public string[][] GetServisesBySubdevision(int subId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT services_id, name FROM services_info_by_subdivisions WHERE subdivision_id = {subId}");
        }

        public string[][] GetServisesInfo(int servId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT name, information FROM services_info_by_subdivisions WHERE services_id = {servId}");
        }
        
        public string[][] InsertAndGetRequestId(int userId, int subdivisionService)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
            ("INSERT INTO requests (stud_id, subdivision_service_id)"+
             $" VALUES ((SELECT stud_id FROM students WHERE user_id = {userId}), "+
             $" {subdivisionService}) returning id AS request_id");
        }

        public int InsertField(int requestId, string name, string value, bool manuallyFilled = false)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithoutTable
            ("INSERT INTO fields (request_id, name, value, manually_filled) " +
             $"Values ({requestId}, '{name}', '{value}', {manuallyFilled});");
        }
        
        public void ChangeRequestStateByFirst(int requestId, int user_id)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"UPDATE requests SET employee_id = (SELECT employee_id FROM employees WHERE user_id = {user_id}), status = 'processing' WHERE id = {requestId} AND status = 'not processed'");
        }

        public string GetLinkToDocumentByServiceId(int serviceId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithDictionary
                ($"SELECT document_link FROM services WHERE id = {serviceId};")["document_link"];
        }
        
        public void ChangeMessagesStatus(int user_id)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable("UPDATE messages SET status = 'viewed'"+
                                    $" WHERE stud_id = (SELECT stud_id FROM students WHERE user_id = {user_id})");
        }
        
        public string[][] GetTableFiltredRequestsForEmployees(int userId, string status) 
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
            ("SELECT request_id, name_service, stud_family, stud_name, stud_secondname, create_date " + 
             " FROM list_of_requests_for_employees " + $"WHERE status = '{status}' AND user_id = {userId};");
        }
        
        public string[][] GetTableNamedRequestsForEmployees(int userId, string family) 
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
            ("SELECT request_id, name_service, stud_family, stud_name, stud_secondname, create_date " + 
             " FROM list_of_requests_for_employees " + $"WHERE stud_family = '{family}' AND user_id = {userId};");
        }
    }
}