using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProgOdev.Data;
using WebProgOdev.Models;
using System.Linq;

namespace WebProgOdev.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }
        private bool IsLoggedIn()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            return userId.HasValue && userId.Value > 0;
        }

        private int GetCurrentUserId()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return 0;
            }

            return userId.Value;
        }


        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }


            var services = _context.Services
                .Where(s => s.IsActive)
                .ToList();

            var trainers = _context.Trainers
                .Where(t => t.IsActive)
                .ToList();

            ViewBag.Services = services;
            ViewBag.Trainers = trainers;

            ViewBag.MinHour = 8;
            ViewBag.MaxHour = 22;

            var model = new AppointmentCreateViewModel();
            var now = DateTime.Now;

            model.Date = now.Date;

            int startHour = now.Hour;
            if (startHour < 8) startHour = 8;
            if (startHour > 21) startHour = 21;

            model.StartHour = startHour;
            model.StartMinute = 0;

            int endHour = startHour + 1;
            if (endHour > 22) endHour = 22;

            model.EndHour = endHour;
            model.EndMinute = 0;

            return View(model);
        }


        [HttpPost]
        public IActionResult Create(AppointmentCreateViewModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var services = _context.Services.Where(s => s.IsActive).ToList();
            var trainers = _context.Trainers.Where(t => t.IsActive).ToList();

            ViewBag.Services = services;
            ViewBag.Trainers = trainers;
            ViewBag.MinHour = 8;
            ViewBag.MaxHour = 22;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var service = services.FirstOrDefault(s => s.Id == model.ServiceId);
            if (service == null)
            {
                ModelState.AddModelError("", "Geçerli bir servis seçilmelidir.");
                return View(model);
            }

            var trainer = trainers.FirstOrDefault(t => t.Id == model.TrainerId);
            if (trainer == null)
            {
                ModelState.AddModelError("", "Geçerli bir eğitmen seçilmelidir.");
                return View(model);
            }

            // TARİH KONTROLÜ
            if (model.Date == DateTime.MinValue)
            {
                ModelState.AddModelError("", "Geçerli bir tarih seçilmelidir.");
                return View(model);
            }

            // Dakika kontrolü (sunucu tarafı)
            if (model.StartMinute != 0 && model.StartMinute != 15 &&
                model.StartMinute != 30 && model.StartMinute != 45)
            {
                ModelState.AddModelError("", "Başlangıç dakikası 00, 15, 30 veya 45 olmalıdır.");
                return View(model);
            }

            if (model.EndMinute != 0 && model.EndMinute != 15 &&
                model.EndMinute != 30 && model.EndMinute != 45)
            {
                ModelState.AddModelError("", "Bitiş dakikası 00, 15, 30 veya 45 olmalıdır.");
                return View(model);
            }

            // TARİH + SAAT birleştir
            var startTime = new DateTime(
                model.Date.Year,
                model.Date.Month,
                model.Date.Day,
                model.StartHour,
                model.StartMinute,
                0
            );

            var endTime = new DateTime(
                model.Date.Year,
                model.Date.Month,
                model.Date.Day,
                model.EndHour,
                model.EndMinute,
                0
            );

            // 1) Geçmiş gün olmasın
            if (model.Date.Date < DateTime.Today)
            {
                ModelState.AddModelError("", "Randevu tarihi bugünden önce olamaz.");
                return View(model);
            }

            // 2) Eğer tarih bugünün tarihi ise, saat de şu andan sonra olmalı
            if (model.Date.Date == DateTime.Today &&
                startTime.TimeOfDay <= DateTime.Now.TimeOfDay)
            {
                ModelState.AddModelError("", "Bugün için randevu saati şu andan sonra olmalıdır.");
                return View(model);
            }


            // Eğitmen çalışma saatleri
            if (startTime.Hour < trainer.StartHour || endTime.Hour > trainer.EndHour)
            {
                ModelState.AddModelError("",
                    $"Bu eğitmen {trainer.StartHour}:00 - {trainer.EndHour}:00 saatleri arasında çalışıyor.");
                return View(model);
            }

            // Eğitmen randevu çakışma kontrolü
            bool trainerOverlap = _context.Appointments
                .Where(a => a.TrainerId == trainer.Id)
                .Any(a =>
                    startTime < a.EndTime &&
                    endTime > a.StartTime
                );

            if (trainerOverlap)
            {
                ModelState.AddModelError("", "Bu eğitmenin bu saatlerde başka bir randevusu bulunmaktadır.");
                return View(model);
            }

            // Kullanıcı randevu çakışma kontrolü
            bool userOverlap = _context.Appointments
                .Where(a => a.UserId == userId)
                .Any(a =>
                    startTime < a.EndTime &&
                    endTime > a.StartTime
                );

            if (userOverlap)
            {
                ModelState.AddModelError("", "Bu kullanıcının bu saatlerde başka bir randevusu bulunmaktadır.");
                return View(model);
            }

            var appointment = new Appointment();
            appointment.ServiceId = service.Id;
            appointment.TrainerId = trainer.Id;
            appointment.UserId = userId;
            appointment.StartTime = startTime;
            appointment.EndTime = endTime;
            appointment.Price = service.Price;
            appointment.Status = AppointmentStatus.Pending;

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("MyAppointments");
        }


        [HttpGet]
        public IActionResult MyAppointments()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var appointments = _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .OrderBy(a => a.StartTime)
                .ToList();

            return View(appointments);
        }


        private bool IsSlotAvailable(Trainer trainer, Service service, DateTime startTime, out DateTime endTime, out string errorMessage)
        {
            errorMessage = "";
            endTime = startTime.AddMinutes(service.DurationMinutes);

            var localEndTime = endTime;

            if (startTime.Hour < trainer.StartHour || localEndTime.Hour > trainer.EndHour)
            {
                errorMessage = $"Bu eğitmen {trainer.StartHour}:00 - {trainer.EndHour}:00 saatleri arasında çalışıyor.";
                return false;
            }

            var overlapping = _context.Appointments
                .Where(a => a.TrainerId == trainer.Id)
                .Where(a =>
                    startTime < a.EndTime &&
                    localEndTime > a.StartTime  
                )
                .Any();

            if (overlapping)
            {
                errorMessage = "Bu eğitmenin bu saatlerde başka bir randevusu bulunmaktadır.";
                return false;
            }

            return true;
        }



        public ActionResult ViewOne(int id = 1)
        {
            var appointment = _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.User)
                .FirstOrDefault(a => a.Id == id);

            if (appointment == null)
            {
                return Content("No appointment found.");
            }

            return View(appointment);
        }
    }
}
