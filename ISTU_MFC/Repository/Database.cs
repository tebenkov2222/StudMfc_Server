using System;
using System.Collections.Generic;
using System.Globalization;
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

        #region Select

        #region User

        public string[][] CheckUserExistence(int userId) // тут нет проверки
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
                ($"SELECT count(*) FROM users WHERE Id = '{userId}';");
        }

        #endregion

        #region Employee

        public bool CheckByEmployee(int userId)
        {
            using var query = new QueryTool(_db);
            return bool.Parse(query.QueryWithTable
                ($"SELECT (SELECT Count(*) FROM employees WHERE user_id = {userId}) = 1")[1][0]);
        }
        
        public int GetSubdivisonByEmployee(int userId)
        {
            using var query = new QueryTool(_db);
            return Int32.Parse(query.QueryWithTable
                ($"SELECT subdivision_id FROM employees WHERE user_id = {userId}")[1][0]);
        }

        public string[][] GetTableAvailableRequestsForEmployees(int userId, string status = "closed")
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
            ("SELECT request_id, name_service, stud_family, stud_name, stud_secondname, create_date " +
             " FROM list_of_requests_for_employees " + $"WHERE status <> '{status}' AND user_id = {userId};");
        }

        public string[][] GetTableFilteredRequestsForEmployees(int userId, string status)
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

        public string[][] GetTableNumberedRequestsForEmployees(int userId, int number)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
            ("SELECT request_id, name_service, stud_family, stud_name, stud_secondname, create_date " +
             " FROM list_of_requests_for_employees " + $"WHERE request_id = {number} AND user_id = {userId};");
        }

        #endregion

        #region Student

        public bool CheckByStudent(int userId)
        {
            using var query = new QueryTool(_db);
            return bool.Parse(query.QueryWithTable
                ($"SELECT (SELECT Count(*) FROM students WHERE user_id = {userId}) = 1")[1][0]);
        }

        public string[][] GetStudentInfo(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT * FROM students_info WHERE user_id = {userId}");
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

        public string[][] GetTableSubdivisionsInfoForStudent(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT * FROM subdivisions_info WHERE user_id =  {userId};");
        }

        public string[][] GetTableRequestsForStudent(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
            ("SELECT request_id, name_service, status, employee_family, employee_name, employee_secondname, create_date" +
             " FROM information_about_requests " + $"WHERE student_user_id = '{userId}';");
        }

        #endregion

        #region Request

        public string GetLinkToDocumentByRequestId(int requestId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithDictionary
                ($"SELECT document_link FROM references_to_documents WHERE request_id = {requestId};")["document_link"];
        }

        public Dictionary<string, string> GetInformationAboutRequest(int requestId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithDictionary
                ($"SELECT * FROM information_about_requests WHERE request_id = {requestId}");
        }

        public string[][] GetRequestFields(int requestId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
                ($"SELECT name, value, manually_filled FROM fields WHERE request_id = {requestId}");
        }

        public int GetStudentByRequest(int requestId)
        {
            using var query = new QueryTool(_db);
            return Int32.Parse(query.QueryWithTable
            ($"SELECT user_id FROM students WHERE stud_id =" +
             $" (SELECT stud_id FROM requests WHERE id = {requestId})")[1][0]);
        }

        #endregion

        #region Field

        public string[][] GetTableFields(int requestId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
                ($"SELECT name, value, manually_filled FROM list_of_fields where request_id = {requestId}");
        }

        #endregion

        #region Message

        public string[][] GetTableMessages(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT * FROM messages_table_for_student WHERE user_id = {userId};");
        }

        #endregion

        #region Service

        public string[][] GetServicesBySubdivision(int subId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
                ($"SELECT services_id, name FROM services_info_by_subdivisions WHERE subdivision_id = {subId};");
        }
        
        public string[][] GetAllServicesBySubdivision(int subId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
                ("SELECT ser.id, ser.name, ser.information, ser.document_link, ss.subdivision_id, ss.status "+
                 " FROM services ser LEFT OUTER JOIN (SELECT * FROM subdivisions_services "+
                 $" WHERE subdivision_id = {subId})AS ss ON ser.id = ss.service_id ;");
        }

        public string[][] GetServicesInfo(int servId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
                ($"SELECT name, information FROM services_info_by_subdivisions WHERE services_id = {servId};");
        }

        public string GetLinkToDocumentByServiceId(int serviceId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithDictionary
                ($"SELECT document_link FROM services WHERE id = {serviceId};")["document_link"];
        }

        #endregion

        #endregion

        #region Update

        public void ChangeRequestState(int requestId, int userId, string status)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"CALL change_request_status({requestId},{userId},'{status}');");
        }

        public void ChangeRequestStateByFirst(int requestId, int userId)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"CALL change_request_state_by_first({requestId}, {userId});");
        }

        public void ChangeMessagesStatus(int userId)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"CALL change_status_message({userId});");
        }
        
        public void ChangeSubdivisonsServiseStatus(int serviceId, int subdivisonId, string status)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"UPDATE subdivisions_services SET status = '{status}' "+
                                    $" WHERE service_id = {serviceId} AND subdivision_id = {subdivisonId}");
        }

        public void RequestRejection(int requestId)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"CALL request_rejection({requestId});");
        }

        #endregion

        #region Insert

        public void CreateMessage(int requestId, int userId, string message)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"CALL create_message({requestId},{userId},'{message}');");
        }

        public string[][] InsertRequest(int userId, int subdivisionService)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
            ("INSERT INTO requests (stud_id, subdivision_service_id)" +
             $" VALUES ((SELECT stud_id FROM students WHERE user_id = {userId}), " +
             $" {subdivisionService}) returning id AS request_id");
        }

        public void InsertField(int requestId, string name, string value, bool manuallyFilled = false)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"CALL insert_field({requestId}, '{name}', '{value}', {manuallyFilled});");
        }

        public void CreateStudent(StudentModelForAddToDB student)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable
            ($"CALL create_student({student.id},{student.student[0].id}," +
             $"'{student.fio_full.family}','{student.fio_full.name}'," +
             $"'{student.fio_full.patronymic}','{student.student[0].department}'," +
             $"'{student.student[0].group}');");
        }
        
        public void InsertSubdivisonsServise(int serviceId, int subdivisonId)
        {
            using var query = new QueryTool(_db);
            var res = query.QueryWithTable
            ("INSERT INTO subdivisions_services (service_id, subdivision_id)" +
             $" VALUES ({serviceId}, {subdivisonId})");
        }

        #endregion
        public void DeleteSubdivisonsServise(int serviceId, int subdivisonId)
        {
            using var query = new QueryTool(_db);
            var res = query.QueryWithTable
            ($"DELETE FROM subdivisions_services WHERE service_id = {serviceId} AND subdivision_id = {subdivisonId};");
        }
    }
}