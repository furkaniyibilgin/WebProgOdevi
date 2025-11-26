using System.ComponentModel.DataAnnotations;

namespace WebProgOdev.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(10, 300)]
        public int DurationMinutes { get; set; } = 60;

        [Range(0, 10000)]
        public decimal Price { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
