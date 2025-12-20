using System.ComponentModel.DataAnnotations;

namespace WebProgOdev.Models
{
    public class AiViewModel
    {
        [Required(ErrorMessage = "Hedef zorunludur.")]
        [MaxLength(200)]
        public string Goal { get; set; } = "";

        [MaxLength(500)]
        public string Notes { get; set; } = "";

        public string ImageUrl { get; set; } = "";

        public string Result { get; set; } = "";
    }
}
