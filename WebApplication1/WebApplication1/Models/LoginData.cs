using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class LoginData
    {
        [Required(ErrorMessage = "Не указан логин")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}