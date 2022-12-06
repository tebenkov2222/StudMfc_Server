using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Office2013.Excel;

namespace ISTU_MFC.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        public string email { get; set; }
         
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        public string message { get; set; }
        public int user_id { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
    }
}