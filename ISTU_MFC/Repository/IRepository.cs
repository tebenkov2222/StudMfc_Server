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
        public List<FieldsModel> GetRequestFields(int requestId);
        public StudentProfileModel GetStudentByRequest(int requestId);
        InformationAboutRequestModel GetInformationAboutRequestByStudent(int studentUserId);
        InformationAboutRequestModel GetInformationAboutRequestByRequest(int requestId);
        public DirectorInstituteModel GetDirectorInstituteByStudent(int studentUserId);
        public void ChangeRequestStatus(int requestId, int userId, string state);
        public void CreateMessage(int requestId, int userId, string message);
        public List<MessageModel> GetTableMessages(int userId);
        public List<SubdivisionModel> GetDivisionsList(int userId);
        public List<ServiseModel> GetSubdivisionInfo(int subId);
        public ServiseModel GetServicesInfo(int servId);
        public void ChangeRequestStateByFirst(int requestId, int userId);
        public void CreateRequestWithFields(int servId, List<FieldsModel> fields, int userId);
        public void ChangeMessagesStatus(int userId);
        public List<RequestModel> GetFilteredRequests(int userId, string status);
        public List<RequestModel> GetNamedRequests(int userId, string family);
        public List<RequestModel> GetNumberedRequests(int userId, int number);
        public bool CheckUserExistence(int userId);
        public void CreateStudent(StudentModelForAddToDB student);
        public ServisesSubdivisonModel GetSubdivisionServices(int userid);
        public void ChangeSubdivisionsServiceStatus(int serviceId, int subdivisionId, string status);
        public void InsertSubdivisionsService(int serviceId, int subdivisonId);
        public void DeleteSubdivisionsService(int serviceId, int subdivisonId);
    }
    
    
}