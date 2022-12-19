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

        // Проверка существования пользователя в таблице пользователей
        public bool CheckUserExistence(int userId)
        {
            using var query = new QueryTool(_db);
            return bool.Parse(query.QueryWithTable
                ($"SELECT (SELECT count(*) FROM users WHERE Id = '{userId}') = 1;")[1][0]);
        }

        public string[][] GetUserFullName(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
                ($"SELECT family, name, secondname FROM users WHERE Id = '{userId}';");
        }

        #endregion

        #region Employee

        // проверка существования сотрудника в таблице сотрудников
        public bool CheckEmployeeExistence(int userId)
        {
            using var query = new QueryTool(_db);
            return bool.Parse(query.QueryWithTable
                ($"SELECT (SELECT Count(*) FROM employees WHERE user_id = {userId}) = 1")[1][0]);
        }
        
        // получить номер подразделения, в котором работает сотрудник
        public int GetSubdivisionByEmployee(int userId)
        {
            using var query = new QueryTool(_db);
            return Int32.Parse(query.QueryWithTable
                ($"SELECT subdivision_id FROM employees WHERE user_id = {userId}")[1][0]);
        }

        // получить таблицу доступных заявок для сотрудника
        // (отображать для сотрудника взятые им заявки и новые заявки в подразделении, к которому он относится)
        public string[][] GetTableAvailableRequestsForEmployees(int userId, string status = "closed")
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
            ("SELECT request_id, name_service, stud_family, stud_name, stud_secondname, create_date " +
             " FROM list_of_requests_for_employees " + $"WHERE status <> '{status}' AND user_id = {userId};");
        }
        
        public string[][] GetFiltredRequestsForEmployees(int userId, string status)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
            ("SELECT request_id, name_service, stud_family, stud_name, stud_secondname, create_date " +
             " FROM list_of_requests_for_employees " + $"WHERE status = '{status}' AND user_id = {userId};");
        }
        
        // получить таблицу заявок по фамилии студента для сотрудника
        // (метод для поиска сотрудником всех заявок по фамилии студента)
        public string[][] GetTableNamedRequestsForEmployees(int userId, string family)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
            ("SELECT request_id, name_service, stud_family, stud_name, stud_secondname, create_date " +
             " FROM list_of_requests_for_employees " + $"WHERE stud_family = '{family}' AND user_id = {userId};");
        }

        // получить таблицу запросов по номеру заявки для сотрудника
        // (метод для поиска сотрудником всех заявок по номеру заявки)
        public string[][] GetTableNumberedRequestsForEmployees(int userId, int number)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
            ("SELECT request_id, name_service, stud_family, stud_name, stud_secondname, create_date " +
             " FROM list_of_requests_for_employees " + $"WHERE request_id = {number} AND user_id = {userId};");
        }

        #endregion

        #region Student

        // проверить существование студента в таблице студентов
        public bool CheckStudentExistence(int userId)
        {
            using var query = new QueryTool(_db);
            return bool.Parse(query.QueryWithTable
                ($"SELECT (SELECT Count(*) FROM students WHERE user_id = {userId}) = 1")[1][0]);
        }
        
        // получить информацию о студенте по его id пользователя
        public string[][] GetStudentInfo(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT * FROM students_info WHERE user_id = {userId}");
        }

        // получить информацию о заявках для конкретного студента
        public Dictionary<string, string> GetInformationAboutStudent(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithDictionary
                ($"SELECT * FROM information_about_requests WHERE student_user_id = {userId}");
        }

        // получить ФИО директора института для конкретного студента
        public Dictionary<string, string> GetDirectorInstituteByStudent(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithDictionary
                ($"SELECT * FROM directors_of_institutes_for_each_user WHERE user_id = {userId}");
        }

        // получить информацию о подразделении для конкретного студента 
        public string[][] GetTableSubdivisionsInfoForStudent(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT * FROM subdivisions_info WHERE user_id =  {userId};");
        }

        // получить информацию о заяках для конкретного студента
        public string[][] GetTableRequestsForStudent(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
            ("SELECT request_id, name_service, status, employee_family, employee_name, employee_secondname, create_date" +
             " FROM information_about_requests " + $"WHERE student_user_id = '{userId}';");
        }

        #endregion

        #region Request

        // получить ссылку на документ по номеру заявки
        public string GetLinkToDocumentByRequestId(int requestId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithDictionary
                ($"SELECT document_link FROM references_to_documents WHERE request_id = {requestId};")["document_link"];
        }

        // получить информацию о заявке по её номеру
        public Dictionary<string, string> GetInformationAboutRequest(int requestId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithDictionary
                ($"SELECT * FROM information_about_requests WHERE request_id = {requestId}");
        }

        // получить поля для документа в заявке по её номеру.
        public string[][] GetRequestFields(int requestId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
                ($"SELECT name, value, manually_filled FROM fields WHERE request_id = {requestId}");
        }

        // получить номер конкретного студента по номеру заявки
        public int GetStudentByRequest(int requestId)
        {
            using var query = new QueryTool(_db);
            return Int32.Parse(query.QueryWithTable
            ($"SELECT user_id FROM students WHERE stud_id =" +
             $" (SELECT stud_id FROM requests WHERE id = {requestId})")[1][0]);
        }

        #endregion

        #region Message

        // получить таблицу сообщений для определённого пользователя
        public string[][] GetTableMessages(int userId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT * FROM messages_table_for_student WHERE user_id = {userId};");
        }

        #endregion

        #region Service

        // получить таблицу с номерами услуг и их информацией в конкретном подразделении
        // (в этом вариатне упор делается на таблицу подразделений)
        public string[][] GetServicesBySubdivision(int subId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
                ($"SELECT * FROM services_info_by_subdivisions WHERE subdivision_id = {subId}"+
                 " AND status = 'Enabled';");
        }
        
        // получить все услуги, которые созданы и отсутствуют в конкретном подразделении
        // (в этом варианте упор делается на таблицу услуг)
        public string[][] GetAllServicesBySubdivision(int subId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
                ("SELECT ser.id, ser.name, ser.information, ser.document_link, ss.subdivision_id, ss.status "+
                 " FROM services ser LEFT OUTER JOIN (SELECT * FROM subdivisions_services "+
                 $" WHERE subdivision_id = {subId}) AS ss ON ser.id = ss.service_id ;");
        }

        // Получить информацию об услуге по её номеру
        public string[][] GetServicesInfo(int servId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable
                ($"SELECT name, information FROM services_info_by_subdivisions WHERE services_id = {servId};");
        }
        
        // получить ссылку на документ (шаблон) для услуги по номеру услуги
        public string GetLinkToDocumentByServiceId(int serviceId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithDictionary
                ($"SELECT document_link FROM services WHERE id = {serviceId};")["document_link"];
        }

        #endregion

        #endregion

        #region Update

        // Изменить статус заявки и указать номер сотрудника обрабатывающего заявку
        public void ChangeRequestState(int requestId, int userId, string status)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"CALL change_request_status({requestId}, {userId},'{status}');");
        }
        
        // отличие от вышеописанного метода в том, что тут присваивается статус 'processing'
        // только для той заявки, которая имеет статус 'not processed',
        // и данное действие описано внутри процедуры.
        // Это специальный метод. Например: два сотрудника обновили страницу и оба нажимают на одну и ту же заявку.
        // Этот метод запишет данные того сотрудника, который первый нажал на заявку,
        // а второму уже не даст перезаписать данные из-за условия.
        public void ChangeRequestStateByFirst(int requestId, int userId)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"CALL change_request_state_by_first({requestId}, {userId});");
        }

        // изменить статус сообщения для конкретного студента
        public void ChangeMessagesStatus(int userId)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"CALL change_status_message({userId});");
        }
        
        // изменить статус услуги для конкретного подразделения
        public void ChangeSubdivisionsServiceStatus(int serviceId, int subdivisonId, string status)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"UPDATE subdivisions_services SET status = '{status}' "+
                                    $" WHERE service_id = {serviceId} AND subdivision_id = {subdivisonId}");
        }

        // отказаться от выполнения определённой заявки. (метод для сотрудника)
        public void RequestRejection(int requestId)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"CALL request_rejection({requestId});");
        }

        #endregion

        #region Insert

        // Создать сообщение на заявку для студента
        public void CreateMessage(int requestId, int userId, string message)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"CALL create_message({requestId},{userId},'{message}');");
        }
        
        // Создать заявку для студента
        public string[][] InsertRequest(int userId, int subdivisionService)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT insert_request({userId}, {subdivisionService})");
        }

        // создать поле для конкретной заявки
        public void InsertField(int requestId, string name, string value, bool manuallyFilled = false)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable($"CALL insert_field({requestId}, '{name}', '{value}', {manuallyFilled});");
        }

        // создать студента (заполняются данные таблицы пользователей и студентов)
        public void CreateStudent(StudentModelForAddToDB student)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable
            ($"CALL create_student({student.id},{student.student[0].id}," +
             $"'{student.fio_full.family}','{student.fio_full.name}'," +
             $"'{student.fio_full.patronymic}','{student.student[0].department}'," +
             $"'{student.student[0].group}');");
        }
        
        // создать конкретную услугу в конкретном подразделении
        public void InsertSubdivisionsService(int serviceId, int subdivisonId)
        {
            using var query = new QueryTool(_db);
            query.QueryWithTable
            ("INSERT INTO subdivisions_services (service_id, subdivision_id)" +
             $" VALUES ({serviceId}, {subdivisonId})");
        }

        public string[][] CreateService(string name, string information = "", string documentLink = "", string formLink = "")
        {
            using var query = new QueryTool(_db);
            return query.QueryWithTable($"SELECT create_services('{name}' , '{information}', '{documentLink}', '{formLink}');");
        }

        #endregion

        #region Delete

        // удалить конкретную сулугу из конкретного подразделения
        public void DeleteSubdivisionsService(int serviceId, int subdivisionId)
        {
            using var query = new QueryTool(_db);
            query.QueryWithoutTable
            ("DELETE FROM subdivisions_services " +
             $"WHERE service_id = {serviceId} AND subdivision_id = {subdivisionId};");
        }

        public int DeleteServices(int servicesId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithoutTable($"DELETE FROM services WHERE id = {servicesId}");
        }

        public int DeleteMessages(int messagesId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithoutTable($"DELETE FROM messages WHERE id = {messagesId}");
        }

        public int DeleteRequest(int requestId)
        {
            using var query = new QueryTool(_db);
            return query.QueryWithoutTable($"DELETE FROM requests WHERE id = {requestId}");
        }
        
        #endregion
    }
}