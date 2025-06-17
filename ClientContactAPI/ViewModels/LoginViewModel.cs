using System.ComponentModel.DataAnnotations;

namespace ClientContactAPI.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string HashPassword { get; set; }
    }
}
