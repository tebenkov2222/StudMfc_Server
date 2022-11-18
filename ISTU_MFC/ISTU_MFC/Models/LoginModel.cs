using System.ComponentModel.DataAnnotations;

namespace ISTU_MFC.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        public string login { get; set; }
         
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}