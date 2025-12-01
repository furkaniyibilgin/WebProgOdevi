    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProgOdev.Models
{
    public enum AppointmentStatus
    {
        Pending = 0,
        Approved = 1,
        Cancelled = 2
    }

    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public int TrainerId { get; set; }

        [Required]
        public int UserId { get; set; } 

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Range(0, 10000)]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public Service Service { get; set; } = null!;
        public Trainer Trainer { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
