using System.Threading.Tasks;

namespace Repository
{
    public interface IRepository
    {
        void WriteMessage(string message);
        bool CheckByStudent(int idUser);
        bool CheckByEmployees(int idUser);
    }
    
    
}