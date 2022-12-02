using System.Collections.Generic;
using System.Threading.Tasks;
using ModelsData;

namespace Repository
{
    public interface IRepository
    {
        void WriteMessage(string message);
        bool CheckByStudent(int idUser);
        bool CheckByEmployees(int idUser);
        public StudentProfileModel GetStudentProfileModel(int userId);
        public void SetValueFieldsOnRequest(int requestId, List<FieldsModel> fields);
        public IDictionary<string, string> GetValueFieldsByPath(IDictionary<string, string[]> nameFieldByPath, int studentUserId);
        public IDictionary<string, string> GetValueFieldsByIdRequest(int requestId);
        public List<RequestModel> GetRequests(int userId);
        public string GetLinkToDocumentByRequestId(int requestId);
        public string GetLinkToDocumentByServiceId(int serviceId);
        public List<FieldsModel> GetRequestFeelds(int requestId);
        public StudentProfileModel GetStudentByRequest(int requestId);
        InformationAboutRequestModel GetInformationAboutRequestByStudent(int studentUserId);
        InformationAboutRequestModel GetInformationAboutRequestByRequest(int requestId);
        public DirectorInstituteModel GetDirectorInstituteByStudent(int studentUserId);
        public void ChangeRequestState(int requestId, int user_id, string state);
        public void CreateMessage(int requestId, int employee_id, string message);
        public List<MessageModel> GetTableMessages(int userId);
        public List<SubdivisionModel> GetDevisionsList(int userId);
        public List<ServiseModel> GetSubdivisionInfo(int sub_id);
        public ServiseModel GetServisesInfo(int servId);
        public void ChangeRequestStateByFirst(int requestId, int user_id);
        public void CreateRequestWithFields(int servId, List<FieldsModel> fields, int userId);
        public void ChangeMessagesStatus(int user_id);
        public List<RequestModel> GetFiltredRequests(int userId, string status);
        public List<RequestModel> GetNamedRequests(int userId, string family);
        public List<RequestModel> GetNumberedRequests(int userId, int number);
        public bool CheckUserExistence(int userId);
        public void CreateStudent(StudentModelForAddToDB student);
    }
    
    
}