using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModelsData;

namespace Repository
{
    public class RepositoryController: IRepository
    {
        private Database _db;
        public int UserId { get; set; }

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
                StudId = studentInfo[1][1]
            };
        }

        public string[][] GetStudentInfo(int userId)
        {
            return _db.GetStudentInfo(userId);
        }
        public IDictionary<string, string> GetValueFields(IDictionary<string, string[]> nameFieldByPath)
        {
            throw new NotImplementedException();
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
                    Id = res[i][0]
                });
            }
            return answ;
        }

        public List<FieldsModel> GetRequestFeelds(int requestId)
        {
            return _db.GetRequestFeelds(requestId);
        }

        public StudentProfileModel GetStudentByRequest(int requestId)
        {
            var user_id = _db.GetStudentByRequest(requestId);
            return GetStudentProfileModel(user_id);
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

        public List<Servises> GetSubdivisionInfo(int sun_id)
        {
            var res = _db.GetServisesBySubdevision(sun_id);
            var answ = new List<Servises>();
            for (int i = 1; i < res.Length; i++)
            {
                answ.Add(new Servises()
                {
                    Name = res[i][1],
                    Id = res[i][0]
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

        public void CreateRequestWithFields(int servId, List<FieldsModel> fields)
        {
            var res = _db.InsertAndGetRequestId(UserId, servId);
            foreach (var field in fields)
            {
                _db.InsertField(Int32.Parse(res[1][0]), field.Name, field.Value, field.Malually_fiiled);
            }
        }
        
        public void ChangeRequestStateByFirst(int requestId, int user_id)
        {
            _db.ChangeRequestStateByFirst(requestId, user_id);
        }
    }
}