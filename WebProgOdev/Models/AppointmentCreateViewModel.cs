using System;
using System.ComponentModel.DataAnnotations;

namespace WebProgOdev.Models
{
    public class AppointmentCreateViewModel
    {
        [Required(ErrorMessage = "Servis seçimi zorunludur.")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Eğitmen seçimi zorunludur.")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Tarih zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        public int StartHour { get; set; }

        [Required(ErrorMessage = "Başlangıç dakikası zorunludur.")]
        public int StartMinute { get; set; }

        [Required(ErrorMessage = "Bitiş saati zorunludur.")]
        public int EndHour { get; set; }

        [Required(ErrorMessage = "Bitiş dakikası zorunludur.")]
        public int EndMinute { get; set; }
    }
}
