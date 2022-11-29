using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModelsData;

namespace Repository
{
    public class RepositoryController: IRepository
    {
        private Database _db;

        public Database Database => _db;

        public RepositoryController()
        {
            _db = new Database();
            _db.Connect(DatabaseConnectData.NiardanDefaultData);
        }

        public void GetDataByFieldsName(IEnumerable<string> fieldsName)
        {
            
        }

        public int Id { get; set; }

        public void WriteMessage(string message)
        {
            Console.WriteLine($"Test message {message}");
        }
        
        public bool CheckByStudent(int idUser)
        {
            return _db.CheckByStudent(idUser);
        }

        public bool CheckByEmployees(int idUser)
        {
            return _db.CheckByEmployee(idUser);
        }

        public StudentProfileModel GetStudentProfileModel(int userId)
        {
            var studentInfo = _db.GetStudentInfo(userId);
            return new StudentProfileModel()
            {
                Family = studentInfo[1][2],
                Name = studentInfo[1][3],
                SecondName = studentInfo[1][4],
                Group = studentInfo[1][5],
                StudId = studentInfo[1][1],
                Department = studentInfo[1][7],
                Faculty = studentInfo[1][6]
            };
        }

        public void SetValueFieldsOnRequest(int requestId, List<FieldsModel> fields)
        {
            foreach (var field in fields)
            {
                _db.InsertField(requestId, field.Name, field.Value, field.Malually_fiiled);
            }
        }

        public string[][] GetStudentInfo(int userId)
        {
            return _db.GetStudentInfo(userId);
        }
        public IDictionary<string, string> GetValueFieldsByPath(IDictionary<string, string[]> nameFieldByPath, int studentUserId)
        {
            IDictionary<string, string> result = new Dictionary<string, string>(); // fieldName, fieldValue
            var packets = new Dictionary<string, Dictionary<string, string>>(); //packetName, <fieldName, packetFieldPath>)

            foreach (var field in nameFieldByPath)
            {
                var packetName = field.Value[0];
                var containsKey = packets.ContainsKey(packetName);
                if (!containsKey)
                {
                    packets.Add(packetName,new Dictionary<string,string>());
                }
                packets[packetName].Add(field.Key, field.Value[1]);
            }

            foreach (var keyValuePair in packets.Select(packet => GetFieldValuesByPacket(packet.Key, packet.Value, studentUserId)).SelectMany(fieldValuesByPacket => fieldValuesByPacket))
            {
                result.Add(keyValuePair);
            }
            return result;
        }

        private Dictionary<string, string> GetFieldValuesByPacket(string packetName, Dictionary<string, string> fieldNameByPacketFieldPath, int studentId)
        {
            var result = new Dictionary<string, string>();
            switch (packetName)
            {
                case "StudentData":
                    var studentModel = GetStudentProfileModel(studentId);
                    foreach (var field in fieldNameByPacketFieldPath)
                    {
                        var valueByName = studentModel.GetValueByName(field.Value);
                        result[field.Key] = valueByName;
                    }
                    break;
                case "DirectorInstitute":
                    
                    var informationAboutRequest = GetDirectorInstituteByStudent(studentId);
                    foreach (var field in fieldNameByPacketFieldPath)
                    {
                        var valueByName = informationAboutRequest.GetValueByName(field.Value);
                        result[field.Key] = valueByName;
                    }
                    break;
            }

            return result;
        }

        public IDictionary<string, string> GetValueFieldsByIdRequest(int requestId)
        {
            var result = new Dictionary<string, string>();
            var tableFields = GetRequestFeelds(requestId);
            foreach (var field in tableFields)
            {
                result[field.Name] = field.Value;
            }

            return result;
        }

        public List<RequestModel> GetRequests(int userId)
        {
            var res = _db.GetTableAvailableRequestsForEmployees(userId);
            var answ = new List<RequestModel>();
            for (int i = 1; i < res.Length; i++)
            {
                answ.Add(new RequestModel()
                {
                    Caption = res[i][1],
                    Id = res[i][0],
                    FamylyNS = res[i][2] + ". " + res[i][3][0] + "." + res[i][4][0],
                    CreationDate = res[i][5].Split()[0]
                });
            }
            return answ;
        }

        public string GetLinkToDocumentByRequestId(int requestId)
        {
            return _db.GetLinkToDocumentByRequestId(requestId);
        }

        public string GetLinkToDocumentByServiceId(int serviceId)
        {
            return _db.GetLinkToDocumentByServiceId(serviceId);
        }

        public List<FieldsModel> GetRequestFeelds(int requestId)
        {
            var res = _db.GetRequestFeelds(requestId);
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

        public StudentProfileModel GetStudentByRequest(int requestId)
        {
            var user_id = _db.GetStudentByRequest(requestId);
            return GetStudentProfileModel(user_id);
        }

        public InformationAboutRequestModel GetInformationAboutRequestByStudent(int studentUserId)
        {   
            var informationAboutRequest = _db.GetInformationAboutStudent(studentUserId);
            return new InformationAboutRequestModel()
            {
                //RequestId = Int32.Parse(informationAboutRequest["request_id"]),
                //StudentUserId = Int32.Parse(informationAboutRequest["student_user_id"]),
                //StudentId = Int32.Parse(informationAboutRequest["stud_id"]),
                StudentFamily = informationAboutRequest["student_family"],
                StudentName = informationAboutRequest["student_name"],
                StudentSecondName = informationAboutRequest["student_secondname"],
                GroupNumber = informationAboutRequest["group_number"],
                //EmployeeUserId = Int32.Parse(informationAboutRequest["employee_user_id"]),
                EmployeeFamily = informationAboutRequest["employee_family"],
                EmployeeName = informationAboutRequest["employee_name"],
                EmployeeSecondName = informationAboutRequest["employee_secondname"],
                NameService = informationAboutRequest["name_service"],
                SubdivisionName = informationAboutRequest["subdivision_name"],
                InstituteName = informationAboutRequest["institute_name"],
                DirectorFamily = informationAboutRequest["director_family"],
                DirectorName = informationAboutRequest["director_name"],
                DirectorSecondName = informationAboutRequest["director_secondname"],
                ChiefFamily = informationAboutRequest["chief_family"],
                ChiefName = informationAboutRequest["chief_name"],
                ChiefSecondName = informationAboutRequest["chief_secondname"],
                CreateDate = informationAboutRequest["create_date"],
            };
        }

        public InformationAboutRequestModel GetInformationAboutRequestByRequest(int requestId)
        {
            var informationAboutRequest = _db.GetInformationAboutRequest(requestId);
            return new InformationAboutRequestModel()
            {
                //RequestId = Int32.Parse(informationAboutRequest["request_id"]),
                //StudentUserId = Int32.Parse(informationAboutRequest["student_user_id"]),
                //StudentId = Int32.Parse(informationAboutRequest["stud_id"]),
                StudentFamily = informationAboutRequest["student_family"],
                StudentName = informationAboutRequest["student_name"],
                StudentSecondName = informationAboutRequest["student_secondname"],
                GroupNumber = informationAboutRequest["group_number"],
                //EmployeeUserId = Int32.Parse(informationAboutRequest["employee_user_id"]),
                EmployeeFamily = informationAboutRequest["employee_family"],
                EmployeeName = informationAboutRequest["employee_name"],
                EmployeeSecondName = informationAboutRequest["employee_secondname"],
                NameService = informationAboutRequest["name_service"],
                SubdivisionName = informationAboutRequest["subdivision_name"],
                InstituteName = informationAboutRequest["institute_name"],
                DirectorFamily = informationAboutRequest["director_family"],
                DirectorName = informationAboutRequest["director_name"],
                DirectorSecondName = informationAboutRequest["director_secondname"],
                ChiefFamily = informationAboutRequest["chief_family"],
                ChiefName = informationAboutRequest["chief_name"],
                ChiefSecondName = informationAboutRequest["chief_secondname"],
                CreateDate = informationAboutRequest["create_date"],
            };
        }

        public DirectorInstituteModel GetDirectorInstituteByStudent(int studentUserId)
        {
            var directorInstituteByStudent = _db.GetDirectorInstituteByStudent(studentUserId);

            return new DirectorInstituteModel()
            {
                Name = directorInstituteByStudent["name"],
                Family = directorInstituteByStudent["family"],
                SecondName = directorInstituteByStudent["secondName"],
            };
        }

        public void ChangeRequestState(int requestId, int user_id, string state)
        {
            _db.ChangeRequestState(requestId, user_id, state);
        }

        public void CreateMessage(int requestId, int employee_id, string message)
        {
            _db.CreateMessage(requestId, employee_id, message);
        }

        public List<MessageModel> GetTableMessages(int userId)
        {
            var res = _db.GetTableMessages(userId);
            var answ = new List<MessageModel>();
            for (int i = 1; i < res.Length; i++)
            {
                answ.Add(new MessageModel()
                {
                    Date = res[i][1],
                    Text = res[i][2],
                    RequestId = res[i][3],
                    Status = res[i][4]
                });
            }
            return answ;
        }

        public List<SubdivisionModel> GetDevisionsList(int userId)
        {
            var res = _db.GetTableSubdivisionsInfoForStudent(userId);
            var answ = new List<SubdivisionModel>();
            for (int i = 1; i < res.Length; i++)
            {
                answ.Add(new SubdivisionModel()
                {
                    Id = res[i][1],
                    Information = res[i][3],
                    Name = res[i][2]
                });
            }

            return answ;
        }

        public List<ServiseModel> GetSubdivisionInfo(int sun_id)
        {
            var res = _db.GetServisesBySubdevision(sun_id);
            var answ = new List<ServiseModel>();
            for (int i = 1; i < res.Length; i++)
            {
                answ.Add(new ServiseModel()
                {
                    Name = res[i][1],
                    Id = Int32.Parse(res[i][0])
                });
            }
            return answ;
        }

        public ServiseModel GetServisesInfo(int servId)
        {
            var res = _db.GetServisesInfo(servId);
            return new ServiseModel()
            {
                Name = res[1][0],
                Info = res[1][1]
            };
        }

        public void CreateRequestWithFields(int servId, List<FieldsModel> fields, int userId)
        {
            var res = _db.InsertAndGetRequestId(userId, servId);
            var requestId = Int32.Parse(res[1][0]);
            SetValueFieldsOnRequest(requestId, fields);
        }
        
        public void ChangeRequestStateByFirst(int requestId, int user_id)
        {
            _db.ChangeRequestStateByFirst(requestId, user_id);
        }

        public void ChangeMessagesStatus(int user_id)
        {
            _db.ChangeMessagesStatus(user_id);
        }
    }
}