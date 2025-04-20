using System.ComponentModel.DataAnnotations;

namespace AI_Sorter.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Введите имя пользователя")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; } = string.Empty;
    }
}
