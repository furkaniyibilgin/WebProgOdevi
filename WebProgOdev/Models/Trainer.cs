using System.ComponentModel.DataAnnotations;

namespace WebProgOdev.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Specialty { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Bio { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        
        [Range(0, 23, ErrorMessage = "Başlangıç saati 0 ile 23 arasında olmalıdır.")]
        public int StartHour { get; set; } = 9;

        [Range(0, 23, ErrorMessage = "Bitiş saati 0 ile 23 arasında olmalıdır.")]
        public int EndHour { get; set; } = 18;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public string FullName => $"{FirstName} {LastName}";
    }
}
