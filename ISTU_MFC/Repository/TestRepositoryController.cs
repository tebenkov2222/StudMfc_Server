using System;
using System.Collections.Generic;
using ModelsData;

namespace Repository
{
    public class TestRepositoryController: IRepository
    {
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

        public void SetValueFieldsOnRequest(int requestId, List<FieldsModel> fields)
        {
            throw new NotImplementedException();
        }

        public void SetValueFieldsOnRequest(int requestId)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> GetValueFieldsByPath(IDictionary<string, string[]> nameFieldByPath, int studentUserId)
        {
            throw new NotImplementedException();
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
            {"GroupStudentField", "Нет нахой"},
            {"NPSurnameDean", "В.В.Путин"},
            {"DepartamentStudent", "Программная Инженерия"},
        };
        public IDictionary<string, string> GetValueFieldsByPath(IDictionary<string, string[]> nameFieldByPath)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            foreach (var field in nameFieldByPath)   
            {
                result.Add(field.Key,_testValueFields[field.Key]);
            }
            return result;
        }

        public IDictionary<string, string> GetValueFieldsByIdRequest(int requestId)
        {
            throw new NotImplementedException();
        }

        public List<RequestModel> GetRequests(int userId)
        {
            throw new NotImplementedException();
        }

        public string GetLinkToDocumentByRequestId(int requestId)
        {
            throw new NotImplementedException();
        }

        public string GetLinkToDocumentByServiceId(int serviceId)
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

        public InformationAboutRequestModel GetInformationAboutRequestByStudent(int studentUserId)
        {
            throw new NotImplementedException();
        }

        public InformationAboutRequestModel GetInformationAboutRequestByRequest(int requestId)
        {
            throw new NotImplementedException();
        }

        public DirectorInstituteModel GetDirectorInstituteByStudent(int studentUserId)
        {
            throw new NotImplementedException();
        }

        public InformationAboutRequestModel GetInformationAboutRequest(int requestId)
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

        public List<ServiseModel> GetSubdivisionInfo(int sun_id)
        {
            throw new NotImplementedException();
        }

        public ServiseModel GetServisesInfo(int servId)
        {
            throw new NotImplementedException();
        }

        public void ChangeRequestStateByFirst(int requestId, int user_id)
        {
            throw new NotImplementedException();
        }

        public void CreateRequestWithFields(int servId, List<FieldsModel> fields, int userId)
        {
            throw new NotImplementedException();
        }

        public void ChangeMessagesStatus(int user_id)
        {
            throw new NotImplementedException();
        }

        public List<RequestModel> GetFiltredRequests(int userId, string status)
        {
            throw new NotImplementedException();
        }

        public List<RequestModel> GetNamedRequests(int userId, string family)
        {
            throw new NotImplementedException();
        }

        public List<RequestModel> GetNumberedRequests(int userId, int number)
        {
            throw new NotImplementedException();
        }

        public void CreateRequestWithFields(int servId, List<FieldsModel> fields)
        {
            throw new NotImplementedException();
        }
    }
}