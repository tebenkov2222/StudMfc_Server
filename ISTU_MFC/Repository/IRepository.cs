using System.Threading.Tasks;

namespace Repository
{
    public interface IRepository
    {
        public int UserId { get; set; }
        void WriteMessage(string message);
        bool CheckByStudent(int idUser);
        bool CheckByEmployees(int idUser);
        public string[][] GetStudentInfo(int userId);
    }
    
    
}