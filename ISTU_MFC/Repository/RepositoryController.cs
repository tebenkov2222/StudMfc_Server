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
                Group = studentInfo[1][1],
                StudId = studentInfo[1][5]
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
    }
}