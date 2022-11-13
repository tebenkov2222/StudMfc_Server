using System;
using System.Collections.Generic;
using ModelsData;

namespace Repository
{
    public class TestRepositoryController: IRepository
    {
        public int UserId { get; set; }
        private bool _isStudent = false;
        public void WriteMessage(string message)
        {
            Console.WriteLine($"Test Repository Controller {message}");
        }

        public bool CheckByStudent(int idUser)
        {
            return _isStudent;
        }

        public bool CheckByEmployees(int idUser)
        {
            return !_isStudent;
        }

        public StudentProfileModel GetStudentProfileModel(int userId)
        {
            return new StudentProfileModel()
            {
                Family = "Test",
                Group = "Test",
                Name = "Test",
                SecondName = "Test",
                StudId = "Test"
            };
        }

        public string[][] GetStudentInfo(int userId)
        {
            throw new System.NotImplementedException();
        }

        private Dictionary<string, string> _testValueFields = new Dictionary<string, string>()
        {
            {"NameStudentField", "Иван"},
            {"SurnameStudentField", "Иванов"},
            {"PatronymicStudentField", "Иванович"},
            {"GroupStudentField", "Нет нахой"}
        };
        public IDictionary<string, string> GetValueFields(IDictionary<string, string[]> nameFieldByPath)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            foreach (var field in nameFieldByPath)   
            {
                result.Add(field.Key,_testValueFields[field.Key]);
            }
            return result;
        }

        public List<RequestModel> GetRequests(int userId)
        {
            throw new NotImplementedException();
        }

        public List<FieldsModel> GetRequestFeelds(int requestId)
        {
            throw new NotImplementedException();
        }

        public StudentProfileModel GetStudentByRequest(int requestId)
        {
            throw new NotImplementedException();
        }

        public void ChangeRequestState(int requestId, int user_id, string state)
        {
            throw new NotImplementedException();
        }

        public void CreateMessage(int requestId, int employee_id, string message)
        {
            throw new NotImplementedException();
        }

        public List<MessageModel> GetTableMessages(int userId)
        {
            throw new NotImplementedException();
        }

        public List<SubdivisionModel> GetDevisionsList(int userId)
        {
            throw new NotImplementedException();
        }

        public List<Servises> GetSubdivisionInfo(int sun_id)
        {
            throw new NotImplementedException();
        }
    }
}