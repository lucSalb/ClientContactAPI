using System.ComponentModel.DataAnnotations;

namespace ClientContactAPI.ViewModels
{
    public class CustomerViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
