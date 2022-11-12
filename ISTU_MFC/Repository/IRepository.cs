using System.Threading.Tasks;
using ModelsData;

namespace Repository
{
    public interface IRepository
    {
        int Id { get; set; }
        void WriteMessage(string message);
        bool CheckByStudent(int idUser);
        bool CheckByEmployees(int idUser);

        StudentProfileModel GetStudentProfileModel();
    }
    
    
}