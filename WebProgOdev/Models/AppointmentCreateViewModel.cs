using System.ComponentModel.DataAnnotations;

namespace WebProgOdev.Models
{
    public class AppointmentCreateViewModel
    {
        [Required(ErrorMessage = "Servis seçimi zorunludur.")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Eğitmen seçimi zorunludur.")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Başlangıç zamanı zorunludur.")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }
    }
}
