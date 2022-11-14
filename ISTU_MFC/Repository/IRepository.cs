using System.Collections.Generic;
using System.Threading.Tasks;
using ModelsData;

namespace Repository
{
    public interface IRepository
    {
        public int UserId { get; set; }
        void WriteMessage(string message);
        bool CheckByStudent(int idUser);
        bool CheckByEmployees(int idUser);
        public StudentProfileModel GetStudentProfileModel(int userId);
        public IDictionary<string, string> GetValueFields(IDictionary<string, string[]> nameFieldByPath);
        public List<RequestModel> GetRequests(int userId);
        public List<FieldsModel> GetRequestFeelds(int requestId);
        public StudentProfileModel GetStudentByRequest(int requestId);
        public void ChangeRequestState(int requestId, int user_id, string state);
        public void CreateMessage(int requestId, int employee_id, string message);
        public List<MessageModel> GetTableMessages(int userId);
        public List<SubdivisionModel> GetDevisionsList(int userId);
        public List<Servises> GetSubdivisionInfo(int sun_id);
        public ServiseModel GetServisesInfo(int servId);
    }
    
    
}