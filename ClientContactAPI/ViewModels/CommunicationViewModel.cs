using System.ComponentModel.DataAnnotations;

namespace ClientContactAPI.ViewModels
{
    public class CommunicationViewModel
    {
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public string TemplateId { get; set; }
    }
    public class CommunicationAllViewModel
    {
        [Required]
        public string TemplateId { get; set; }
    }
}
