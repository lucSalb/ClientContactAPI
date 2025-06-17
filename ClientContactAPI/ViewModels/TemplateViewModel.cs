using System.ComponentModel.DataAnnotations;

namespace ClientContactAPI.ViewModels
{
    public class TemplateViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
