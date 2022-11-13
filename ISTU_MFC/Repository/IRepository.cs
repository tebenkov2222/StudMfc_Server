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
        public string[][] GetStudentInfo(int userId);
        public IDictionary<string, string> GetValueFields(IDictionary<string, string[]> nameFieldByPath);
    }
    
    
}